using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.ExcelAc;

namespace Implementing_A_star_algorithm.RetriveData;

public class StationManager
{
    private readonly TubeGraph _tubeGraph;
    private static readonly string baseUrl = "https://api.tfl.gov.uk/Line";

    public StationManager(TubeGraph tubeGraph)
    {
        _tubeGraph = tubeGraph;
    }

    private async Task<JsonElement> GetStationInfoAsync(string stationId)
    {
        var url = $"https://api.tfl.gov.uk/StopPoint/{stationId}";
        using var httpClient = new HttpClient();
        var responseString = await httpClient.GetStringAsync(url);
        var responseJson = JsonSerializer.Deserialize<JsonElement>(responseString);
        return responseJson;
    }
    public async Task PopulateGraphAsync()
    {

        var tubeLines = await GetLines();
        foreach (var tubeLine in tubeLines)
        {
            var stationDetails = await GetStationsOfLines(tubeLine.id);
            TubeGraph.Station previousStation = null;
            foreach (var stationDetail in stationDetails)
            {
                var currentStation = _tubeGraph.Stations.GetValueOrDefault(stationDetail.name) ??
                                     new TubeGraph.Station(stationDetail.name, stationDetail.lat,
                                         stationDetail.lon,stationDetail.stationId,stationDetail.zone,
                                         "");

                if (!currentStation.lines.Contains(tubeLine))
                {
                    currentStation.lines.Add(tubeLine); // Add current tube line to station
                }

                if (previousStation != null)
                {
                    previousStation.Connect(currentStation);
                }

                _tubeGraph.AddOrUpdateStation(currentStation);
                previousStation = currentStation;
            }
        }
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

        var stationsResponse = await httpClient.GetStringAsync($"{baseUrl}/{lineId}/Route/Sequence/all");
        var routeSequence = JsonSerializer.Deserialize<TubeGraph.RouteSequence>(stationsResponse);
        return routeSequence.stations;
    }
}