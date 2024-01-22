﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.ExcelAc;

namespace Implementing_A_star_algorithm.RetriveData;

public class StationManager
{
    private static readonly string baseUrl = "https://api.tfl.gov.uk/Line";

    public StationManager()
    {
    }

    private async Task<JsonElement> GetStationInfoAsync(string stationId)
    {
        var url = $"https://api.tfl.gov.uk/StopPoint/{stationId}";
        using var httpClient = new HttpClient();
        var responseString = await httpClient.GetStringAsync(url);
        var responseJson = JsonSerializer.Deserialize<JsonElement>(responseString);
        return responseJson;
    }

    public async Task<TubeGraph> PopulateGraphAsync()
    {
        var tubeGraph = new TubeGraph();
        var tubeLines = await GetLines();
        foreach (var tubeLine in tubeLines)
        {
            var stationDetails = await GetStationsOfLines(tubeLine.id);
            TubeGraph.Station previousStation = null;
            foreach (var stationDetail in stationDetails)
            {
                var currentStation = tubeGraph.Stations.GetValueOrDefault(stationDetail.name) ??
                                     new TubeGraph.Station(stationDetail.name,
                                         stationDetail.lat,
                                         stationDetail.lon,
                                         stationDetail.stationId,
                                         stationDetail.zone,
                                         null);

                if (!currentStation.lines.Contains(tubeLine))
                {
                    currentStation.lines.Add(tubeLine); // Add current tube line to station
                }

                if (previousStation != null)
                {
                    previousStation.Connect(currentStation);
                }

                var departureInfo = await FetchDepartureTimesForStation(stationDetail.stationId);
                tubeGraph.AddOrUpdateStation(currentStation);
                tubeGraph.AddDepartureTimesToStation(stationDetail.name,departureInfo);
                previousStation = currentStation;
            }
        }

        return tubeGraph;
    }

    private async Task<List<TubeGraph.TubeLine>> GetLines()
    {
        using var httpClient = new HttpClient();
        var tubeLinesResponse = await httpClient.GetStringAsync($"{baseUrl}/Mode/tube");
        var tubeLines = JsonSerializer.Deserialize<List<TubeGraph.TubeLine>>(tubeLinesResponse);
        return tubeLines;
    }

    private async Task<List<TubeGraph.StationDetail>> GetStationsOfLines(string lineId)
    {
        using var httpClient = new HttpClient();

        var stationsResponse =
            await httpClient.GetStringAsync($"{baseUrl}/{lineId}/Route/Sequence/all?serviceTypes=Regular");
        var routeSequence = JsonSerializer.Deserialize<TubeGraph.RouteSequence>(stationsResponse);
        return routeSequence.stations;
    }
    
    public async Task<List<TubeGraph.DepartureInfo>> FetchDepartureTimesForStation(string naptanId)
    {
        var httpClient = new HttpClient();
        var apiUrl = $"https://api.tfl.gov.uk/StopPoint/{naptanId}/Arrivals";
        var response = await httpClient.GetStringAsync(apiUrl);
        var departures = JsonSerializer.Deserialize<List<TubeGraph.DepartureInfo>>(response);

        return departures.Select(d => new TubeGraph.DepartureInfo
        {
            lineName = d.lineName,
            destinationName = d.destinationName,
            expectedArrival = DateTime.Parse(d.expectedArrival.ToString()),
            direction = d.direction,
            timeToStation = d.timeToStation,
            timeToStationInMiunute = TimeSpan.FromSeconds(Convert.ToInt32(d.timeToStation))
            // ... map other properties
        }).ToList();
    }
}