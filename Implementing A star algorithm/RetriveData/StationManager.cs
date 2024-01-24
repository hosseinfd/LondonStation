using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Implementing_A_star_algorithm.Models;

namespace Implementing_A_star_algorithm.RetriveData;

public class StationManager
{
    public StationManager()
    {
    }

    public async Task<List<StationModel>> PopulateGraphAsync()
    {
        var tubeLines = await GetLines();
        var stationModels = new List<StationModel>();
        var disruptionOfLines = await FetchDisruptionOfLines();
        foreach (var tubeLine in tubeLines)
        {
            var line = new LineModel(tubeLine.id, tubeLine.name);
            line.AddDisruption(disruptionOfLines, tubeLine.id);

            var stationDetails = await GetStationsOfLines(tubeLine.id);
            foreach (var stationDetail in stationDetails)
            {
                var stationModel = new StationModel(stationDetail.name,
                    stationDetail.stationId,
                    stationDetail.lat,
                    stationDetail.lon,
                    stationDetail.zone);


                stationModel.AddLine(line);
                stationModels.Add(stationModel);
            }

            var stationsOnLine = stationModels.Where(s => s.Lines.Any(l => l.LineId == line.LineId)).ToList();

            for (var i = 0; i < stationsOnLine.Count; i++)
            {
                if (i < stationsOnLine.Count - 1)
                {
                    // Connect current station to the next station
                    stationsOnLine[i].AddConnection(new ConnectionStationModel(
                        stationsOnLine[i + 1].Name,
                        stationsOnLine[i + 1].Id,
                        stationsOnLine[i + 1].Lat,
                        stationsOnLine[i + 1].Lon,
                        stationsOnLine[i + 1].Zone
                    ));
                }

                if (i > 0)
                {
                    // Connect current station to the previous station
                    stationsOnLine[i].AddConnection(new ConnectionStationModel(
                        stationsOnLine[i - 1].Name,
                        stationsOnLine[i - 1].Id,
                        stationsOnLine[i - 1].Lat,
                        stationsOnLine[i - 1].Lon,
                        stationsOnLine[i - 1].Zone
                    ));
                }

                var stationsJson = JsonSerializer.Serialize(stationModels);
            }
        }

        // foreach (var stationModel in stationModels)
        // {
        //     await Task.Delay(200);
        //     foreach (var connection in stationModel.Connections)
        //     {
        //         var arrivalTime =
        //             await FetchTimeTableForStation(stationModel.Id, connection.Id, stationModel.Lines.First().LineId);
        //         connection.TimeTables = arrivalTime.Timetables;
        //     }
        // }

        return stationModels;
    }

    private async Task<List<TubeGraph.TubeLine>> GetLines()
    {
        using var httpClient = new HttpClient();
        var tubeLinesResponse = await httpClient.GetStringAsync("https://api.tfl.gov.uk/Line/Mode/tube");
        var tubeLines = JsonSerializer.Deserialize<List<TubeGraph.TubeLine>>(tubeLinesResponse);
        return tubeLines;
    }

    private async Task<List<TubeGraph.StationDetail>> GetStationsOfLines(string lineId)
    {
        using var httpClient = new HttpClient();

        var stationsResponse =
            await httpClient.GetStringAsync(
                $"https://api.tfl.gov.uk/Line/{lineId}/Route/Sequence/all?serviceTypes=Regular");
        var routeSequenceJsonElement = JsonSerializer.Deserialize<JsonElement>(stationsResponse);

        var a = new List<TubeGraph.StationDetail>();
        var stopPoints = routeSequenceJsonElement
            .GetProperty("stopPointSequences")
            .EnumerateArray()
            .Where(q => q.GetProperty("direction").GetString() == "inbound")
            .SelectMany(q => q
                .GetProperty("stopPoint").EnumerateArray()
                .Where(stopPoint => stopPoint
                    .GetProperty("modes")
                    .EnumerateArray()
                    .Any(mode => mode.GetString() == "tube" || mode.GetString() == "bus"))
                .Select(stopPoint => new TubeGraph.StationDetail
                {
                    lat = stopPoint.GetProperty("lat").GetDouble(),
                    lon = stopPoint.GetProperty("lon").GetDouble(),
                    name = stopPoint.GetProperty("name").GetString(),
                    stationId = stopPoint.GetProperty("stationId").GetString(),
                    zone = stopPoint.GetProperty("zone").GetString()
                }))
            .GroupBy(s => s.stationId)
            .Select(g => g.ToList())
            .ToList();

        foreach (var stopPoint in stopPoints)
        {
            a.AddRange(stopPoint);
        }

        return a;
    }

    public async
        Task<(List<ConnectionStationModel.IntervalTime> IntervalTimes, List<ConnectionStationModel.TimeTable> Timetables
            )> FetchTimeTableForStation(string fromStopPointId, string toStopPointId, string lineId)
    {
        var httpClient = new HttpClient();
        var url = $"https://api.tfl.gov.uk/Line/{lineId}/Timetable/{fromStopPointId}/to/{toStopPointId}";
        

        var intervalTimes = new List<ConnectionStationModel.IntervalTime>();
        var timeTables = new List<ConnectionStationModel.TimeTable>();
        try
        {
            var response = await httpClient.GetStringAsync(url);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(response);
            if (jsonElement.TryGetProperty("timetable", out var timetableElement) &&
                timetableElement.TryGetProperty("departureStopId", out var departureStopIdElement))
            {
                var departureStopId = jsonElement.GetProperty("timetable").GetProperty("departureStopId").GetString();

                if (departureStopId == fromStopPointId)
                {
                    var routes = jsonElement.GetProperty("timetable").GetProperty("routes").EnumerateArray();
                    foreach (var route in routes)
                    {
                        var stationIntervals = route.GetProperty("stationIntervals").EnumerateArray();
                        foreach (var stationInterval in stationIntervals)
                        {
                            var intervals = stationInterval.GetProperty("intervals").EnumerateArray();
                            foreach (var interval in intervals)
                            {
                                var stopId = interval.GetProperty("stopId").GetString();
                                if (stopId == toStopPointId)
                                {
                                    var intervalId = Convert.ToInt32(stationInterval.GetProperty("id").GetString());
                                    var timeToArrival = Convert.ToInt32(interval.GetProperty("timeToArrival").GetDouble());
                                    intervalTimes.Add(new ConnectionStationModel.IntervalTime
                                    {
                                        IntervalId = intervalId,
                                        StopId = stopId,
                                        TimeToArrival = timeToArrival
                                    });
                                }
                            }
                        }

                        // Accessing schedules within each route
                        if (route.TryGetProperty("schedules", out var schedules))
                        {
                            foreach (var scheduleElement in schedules.EnumerateArray())
                            {
                                var timetable = ParseTimeTable(scheduleElement);
                                timeTables.Add(timetable);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return (IntervalTimes: intervalTimes, Timetables: timeTables); 

    }

    public ConnectionStationModel.TimeTable ParseTimeTable(JsonElement scheduleElement)
    {
        var timeTable = new ConnectionStationModel.TimeTable
        {
            KnownJourneys = new List<ConnectionStationModel.KnownJourney>(),
            Periods = new List<ConnectionStationModel.Period>()
        };

        // Parse the schedule name to IntervalEnum
        var scheduleName = scheduleElement.GetProperty("name").GetString();
        timeTable.IntervalEnum = scheduleName switch
        {
            "Monday - Friday" => ConnectionStationModel.TimeTableEnum.Monday_Friday,
            "Saturdays and Public Holidays" => ConnectionStationModel.TimeTableEnum.Saturdays_Holidays,
            "Sunday" => ConnectionStationModel.TimeTableEnum.Sunday,
            _ => ConnectionStationModel.TimeTableEnum.Ivalid
        };

        // Parse first journey
        var firstJourney = scheduleElement.GetProperty("firstJourney");
        timeTable.FirstJourney = new TimeSpan(
            Convert.ToInt32(firstJourney.GetProperty("hour").GetString()),
            Convert.ToInt32(firstJourney.GetProperty("minute").GetString()),
            0);

        // Parse last journey
        var lastJourney = scheduleElement.GetProperty("lastJourney");
        timeTable.LastJourney = new TimeSpan(
            Convert.ToInt32(lastJourney.GetProperty("hour").GetString()),
            Convert.ToInt32(lastJourney.GetProperty("minute").GetString()),
            0);

        // Parse known journeys
        foreach (var journeyElement in scheduleElement.GetProperty("knownJourneys").EnumerateArray())
        {
            var knownJourney = new ConnectionStationModel.KnownJourney
            {
                Time = new TimeSpan(
                    Convert.ToInt32(journeyElement.GetProperty("hour").GetString()),
                    Convert.ToInt32(journeyElement.GetProperty("minute").GetString()),
                    0),
                IntervalId = journeyElement.GetProperty("intervalId").GetInt32()
            };
            timeTable.KnownJourneys.Add(knownJourney);
        }

        // Parse periods
        foreach (var periodElement in scheduleElement.GetProperty("periods").EnumerateArray())
        {
            var period = new ConnectionStationModel.Period
            {
                FromTime = new TimeSpan(
                    Convert.ToInt32(periodElement.GetProperty("fromTime").GetProperty("hour").GetString()),
                    Convert.ToInt32(periodElement.GetProperty("fromTime").GetProperty("minute").GetString()), 
                    0),
                ToTime = new TimeSpan(
                    Convert.ToInt32(periodElement.GetProperty("toTime").GetProperty("hour").GetString()),
                    Convert.ToInt32(periodElement.GetProperty("toTime").GetProperty("minute").GetString()),
                    0)
            };
            timeTable.Periods.Add(period);
        }

        return timeTable;
    }

    public async Task<List<TflDisruptionDto.Disruption>> FetchDisruptionOfLines()
    {
        var url = "https://api.tfl.gov.uk/Line/Mode/tube/Status";
        var httpClient = new HttpClient();
        var response = await httpClient.GetStringAsync(url);
        var disruption = JsonSerializer.Deserialize<List<TflDisruptionDto.Disruption>>(response);
        return disruption;
    }
}