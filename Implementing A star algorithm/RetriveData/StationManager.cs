using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Implementing_A_star_algorithm.RetriveData;

public class StationManager
{
    private readonly TubeGraph _tubeGraph;
    private static readonly string baseUrl = "https://api.tfl.gov.uk/Line";

    public StationManager(TubeGraph tubeGraph)
    {
        _tubeGraph = tubeGraph;
    }

    public async Task PopulateGraphAsync()
    {
        using var httpClient = new HttpClient();

        var tubeLinesResponse = await httpClient.GetStringAsync($"{baseUrl}/Mode/tube");
        var tubeLines = JsonSerializer.Deserialize<List<TubeGraph.TubeLine>>(tubeLinesResponse);

        foreach (var tubeLine in tubeLines)
        {
            var stationsResponse = await httpClient.GetStringAsync($"{baseUrl}/{tubeLine.id}/Route/Sequence/all");
            var routeSequence = JsonSerializer.Deserialize<TubeGraph.RouteSequence>(stationsResponse);

            TubeGraph.Station previousStation = null;
            foreach (var stationDetail in routeSequence.stations)
            {
                var currentStation = _tubeGraph.Stations.GetValueOrDefault(stationDetail.name) ??
                                     new TubeGraph.Station(stationDetail.name, stationDetail.lat,
                                         stationDetail.lon,stationDetail.stationId,stationDetail.zone);

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
}