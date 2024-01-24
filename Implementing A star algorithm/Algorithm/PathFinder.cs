using System;
using System.Collections.Generic;
using System.Linq;
using Implementing_A_star_algorithm.Algorithm;
using Implementing_A_star_algorithm.Models;

public class PathFinder
{
    private Dictionary<StationLineKey, StationModel> stations;

    public PathFinder(IEnumerable<StationModel> stationList)
    {
        stations = new Dictionary<StationLineKey, StationModel>();

        foreach (var station in stationList)
        {
            foreach (var line in station.Lines)
            {
                var key = new StationLineKey(station.Id, line.LineId);
                // Assumes StationModel objects can represent the same station on different lines
                stations[key] = station;
            }
        }
    }

    public List<PathStep> FindBestPath(StationLineKey startKey, StationLineKey endKey)
    {
        // Initialization
        var openSet = new PriorityQueue<StationLineKey>();
        var cameFrom = new Dictionary<StationLineKey, StationLineKey>();
        var gScore = new Dictionary<StationLineKey, double>();

        foreach (var key in stations.Keys)
        {
            gScore[key] = double.MaxValue;
        }

        gScore[startKey] = 0;
        openSet.Enqueue(startKey, 0);

        while (!openSet.IsEmpty)
        {
            var currentKey = openSet.Dequeue();
            var currentStation = stations[currentKey];

            if (currentKey.Equals(endKey))
            {
                return ReconstructPath(cameFrom, currentKey);
            }

            foreach (var connection in currentStation.Connections)
            {
                foreach (var line in currentStation.Lines)
                {
                    var neighborKey = new StationLineKey(connection.Id, line.LineId);
                    if (!stations.ContainsKey(neighborKey)) continue;

                    var neighborStation = stations[neighborKey];
                    double travelCost = CalculateTravelCost(currentStation, neighborStation);
                    double tentativeGScore = gScore[currentKey] + travelCost;

                    if (tentativeGScore < gScore[neighborKey])
                    {
                        cameFrom[neighborKey] = currentKey;
                        gScore[neighborKey] = tentativeGScore;

                        if (!openSet.Contains(neighborKey))
                        {
                            openSet.Enqueue(neighborKey, tentativeGScore);
                        }
                    }
                }
            }
        }

        return null; // Path not found
    }

    private List<PathStep> ReconstructPath(Dictionary<StationLineKey, StationLineKey> cameFrom, StationLineKey endKey)
    {
        var totalPath = new List<PathStep>();
        var currentKey = endKey;

        while (cameFrom.ContainsKey(currentKey))
        {
            var currentStation = stations[currentKey];
            var previousKey = cameFrom[currentKey];
            var previousStation = stations[previousKey];

            var step = new PathStep
            {
                StationName = currentStation.Name,
                LineName = currentStation.Lines.FirstOrDefault(ln => ln.LineId == currentKey.LineId)?.Name ?? "Unknown"
            };
            totalPath.Insert(0, step);

            currentKey = previousKey;
        }

        // Add the start step at the beginning of the path
        var startStation = stations[currentKey];
        totalPath.Insert(0, new PathStep
        {
            StationName = startStation.Name,
            LineName = startStation.Lines.FirstOrDefault(ln => ln.LineId == currentKey.LineId)?.Name ?? "Unknown"
        });

        return totalPath;
    }

    private double CalculateTravelCost(StationModel fromStation, StationModel toStation)
    {
        // Calculate geographical distance, or use a predefined travel time.
        // Add additional cost if switching lines.
        return CalculateGeographicalDistance(fromStation.Lat, fromStation.Lon, toStation.Lat, toStation.Lon);
    }

    private double CalculateGeographicalDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var earthRadiusKm = 6371;

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        lat1 = ToRadians(lat1);
        lat2 = ToRadians(lat2);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKm * c;
    }

    private double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}

public class StationLineKey : IComparable<StationLineKey>
{
    public string StationId { get; }
    public string LineId { get; }

    public StationLineKey(string stationId, string lineId)
    {
        StationId = stationId;
        LineId = lineId;
    }

    public int CompareTo(StationLineKey other)
    {
        if (other == null) return 1;

        // You can adjust the comparison logic to fit your needs
        int stationIdComparison = StationId.CompareTo(other.StationId);
        if (stationIdComparison != 0) return stationIdComparison;

        return LineId.CompareTo(other.LineId);
    }

    // Override Equals and GetHashCode to use StationLineKey as Dictionary keys
    public override bool Equals(object obj)
    {
        return obj is StationLineKey other &&
               StationId == other.StationId &&
               LineId == other.LineId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StationId, LineId);
    }
}

public class PathStep
{
    public string StationName { get; set; }

    public string LineName { get; set; }
    // Additional properties like TravelTime can be added if needed
}