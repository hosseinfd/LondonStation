using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using Implementing_A_star_algorithm.RetriveData;

namespace Implementing_A_star_algorithm.ExcelServices
{
    public class ExcelService
    {
        public void SaveToExcel(TubeGraph tubeGraph, string filePath)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Tube Data");

            // Headers
            worksheet.Cell(1, 1).Value = "Station Name";
            worksheet.Cell(1, 2).Value = "Latitude";
            worksheet.Cell(1, 3).Value = "Longitude";
            worksheet.Cell(1, 4).Value = "Connected Stations";
            worksheet.Cell(1, 5).Value = "Tube Lines"; 
            worksheet.Cell(1, 6).Value = "Zone number"; 

            int row = 2;

            foreach (var stationEntry in tubeGraph.Stations)
            {
                var station = stationEntry.Value;
                worksheet.Cell(row, 1).Value = station.name;
                worksheet.Cell(row, 2).Value = station.lat;
                worksheet.Cell(row, 3).Value = station.lon;
                worksheet.Cell(row, 4).Value = string.Join(", ", station.connections.Select(s => s.name));
                worksheet.Cell(row, 5).Value = string.Join(", ", station.lines.Select(l => l.name));
                worksheet.Cell(row, 6).Value = string.Join(", ", station.zone);
                // worksheet.Cell(row, 5).Value = string.Join(", ", station.naptanId);
                row++;
            }

            // AutoFit columns for better view
            worksheet.Columns().AdjustToContents();

            workbook.SaveAs(filePath);
        }

        public List<TubeStation> ImportFromExcel(string filePath)
        {
            var tubeStations = new List<TubeStation>();
            var tubeStationsDictionary = new Dictionary<string, TubeStation>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1); // Assuming the data is in the first sheet
                var rows = worksheet.RowsUsed().Skip(1); // Skip header row

                // First pass: create all TubeStation objects and populate dictionary
                foreach (var row in rows)
                {
                    var stationName = row.Cell(1).GetString();
                    var latitude = row.Cell(2).GetDouble();
                    var longitude = row.Cell(3).GetDouble();
                    var tubeLines = row.Cell(5).GetString().Split(new[] { ", " }, StringSplitOptions.None);
                    var zoneCellValue = row.Cell(6).GetString();
                    
                    List<int> zoneNumbers;

                    if (zoneCellValue.Contains("+") || zoneCellValue.Contains("/"))
                    {
                        zoneNumbers = zoneCellValue.Split(new[] { '+', '/' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToList();
                    }
                    else
                    {
                        zoneNumbers = new List<int> { int.Parse(zoneCellValue) };
                    }

                    var tubeStation = new TubeStation
                    {
                        Name = stationName,
                        Latitude = latitude,
                        Longitude = longitude,
                        TubeLines = new List<string>(tubeLines),
                        ZoneNumbers = zoneNumbers
                    };

                    tubeStationsDictionary[stationName] = tubeStation;
                    tubeStations.Add(tubeStation);
                }

                // Second pass: link the connected stations
                foreach (var row in rows)
                {
                    var stationName = row.Cell(1).GetString();
                    var connectedStationsNames = row.Cell(4).GetString().Split(new[] { ", " }, StringSplitOptions.None);

                    var tubeStation = tubeStationsDictionary[stationName];
                    foreach (var connectedStationName in connectedStationsNames)
                    {
                        if (tubeStationsDictionary.TryGetValue(connectedStationName.Trim(),
                                out var connectedTubeStation))
                        {
                            tubeStation.ConnectedTubeStations.Add(connectedTubeStation);
                        }
                    }
                }
            }

            return tubeStations;
        }
    }
}