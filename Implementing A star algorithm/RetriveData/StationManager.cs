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
        var tubeGraph = new TubeGraph();
        var tubeLines = await GetLines();
        var stationModels = new List<StationModel>();
        var disruptionOfLines = await FetchDisruptionOfLines();
        foreach (var tubeLine in tubeLines)
        {
            var line = new LineModel(tubeLine.id, tubeLine.name);
            line.AddDisruption(disruptionOfLines, tubeLine.id);

            var stationDetails = await GetStationsOfLines(tubeLine.id);
            // var connectionsModel = new List<ConnectionStationModel>();
            foreach (var stationDetail in stationDetails)
            {
                var stationModel = new StationModel(stationDetail.name,
                    stationDetail.stationId,
                    stationDetail.lat,
                    stationDetail.lon,
                    stationDetail.zone);

                var connectionStationModel = new ConnectionStationModel(stationDetail.name,
                    stationDetail.stationId,
                    stationDetail.lat,
                    stationDetail.lon,
                    stationDetail.zone);
                // connectionsModel.Add(connectionStationModel);

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

        var stopPoints = routeSequenceJsonElement
            .GetProperty("stopPointSequences")
            .EnumerateArray()
            .Where(q => q.GetProperty("direction").GetString() == "inbound")
            .Select(q => q
                .GetProperty("stopPoint").EnumerateArray()
                .Select(stopPoint => new TubeGraph.StationDetail
                {
                    lat = stopPoint.GetProperty("lat").GetDouble(),
                    lon = stopPoint.GetProperty("lon").GetDouble(),
                    name = stopPoint.GetProperty("name").GetString(),
                    stationId = stopPoint.GetProperty("stationId").GetString(),
                    zone = stopPoint.GetProperty("zone").GetString()
                }).ToList()).FirstOrDefault();


        return stopPoints;
    }

    public async Task FetchTimeTableForStation(string fromStopPointId, string toStopPointId, string lineId)
    {
        var httpClient = new HttpClient();
        var url = $"https://api.tfl.gov.uk/Line/{lineId}/Timetable/{fromStopPointId}/to/{toStopPointId}";
        var response = await httpClient.GetStringAsync(url);
        var timetable = JsonSerializer.Deserialize<TubeGraph.Timetable>(response);
        // return timetable;
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