using System;
using System.Collections.Generic;
using Implementing_A_star_algorithm.ExcelServices;

namespace Implementing_A_star_algorithm.Algorithm;

public class PathFinder
{
    public List<TubeStation> FindPathWithAStar(TubeStation start, TubeStation goal)
    {
        var openSet = new HashSet<TubeStation> { start };
        var cameFrom = new Dictionary<TubeStation, TubeStation>();
        var gScore = new Dictionary<TubeStation, double> { { start, 0 } };
        var fScore = new Dictionary<TubeStation, double> { { start, HaversineDistance(start, goal) } };

        var priorityQueue = new PriorityQueue<TubeStationNode>();
        priorityQueue.Enqueue(new TubeStationNode(start, HaversineDistance(start, goal)));

        while (priorityQueue.Count() > 0)
        {
            var current = priorityQueue.Dequeue().TubeStation;
            if (current == goal)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);

            foreach (var neighbor in current.ConnectedTubeStations)
            {
                var tentativeGScore = gScore[current] + HaversineDistance(current, neighbor);

                if (tentativeGScore < gScore.GetValueOrDefault(neighbor, double.MaxValue))
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + HaversineDistance(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                        priorityQueue.Enqueue(new TubeStationNode(neighbor, fScore[neighbor]));
                    }
                }
            }
        }

        return new List<TubeStation>(); // No path was found
    }

    public List<TubeStation> FindPathWithHeuristic(TubeStation start, TubeStation goal)
    {
        var openSet = new PriorityQueue<TubeStationNode>();
        var cameFrom = new Dictionary<TubeStation, TubeStation>();
        var closedSet = new HashSet<TubeStation>();

        openSet.Enqueue(new TubeStationNode(start, 0));

        while (openSet.Count() > 0)
        {
            var current = openSet.Dequeue().TubeStation;
            if (current == goal)
            {
                return ReconstructPath(cameFrom, current);
            }

            closedSet.Add(current);

            foreach (var neighbor in current.ConnectedTubeStations)
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                var estimatedDistance = HaversineDistance(neighbor, goal);
                if (!openSet.Any(n => n.TubeStation == neighbor))
                {
                    cameFrom[neighbor] = current;
                    openSet.Enqueue(new TubeStationNode(neighbor, estimatedDistance));
                }
            }
        }

        return new List<TubeStation>(); // No path was found
    }

    #region Private_methods

    private List<TubeStation> ReconstructPath(Dictionary<TubeStation, TubeStation> cameFrom, TubeStation current)
    {
        var totalPath = new List<TubeStation> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }

        return totalPath;
    }

    private double HaversineDistance(TubeStation station1, TubeStation station2)
    {
        double R = 6371; // Earth's radius in km
        var lat1 = station1.Latitude * Math.PI / 180;
        var lat2 = station2.Latitude * Math.PI / 180;
        var deltaLat = lat2 - lat1;
        var deltaLon = (station2.Longitude - station1.Longitude) * Math.PI / 180;

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c; // Distance in km
    }
    
    // private double CalculateAverageTravelTime(TubeStation current, TubeStation goal)
    // {
    //     // Example implementation, assuming direct connections
    //     // This needs to be adjusted based on your network's structure
    //     var departures = current == "inbound" ? current.InboundDepartures : current.OutboundDepartures;
    //     var totalTravelTime = departures
    //         .Where(d => d.destinationName == goal.name)
    //         .Average(d => d.timeToStationInMiunute.TotalHours);
    //
    //     return totalTravelTime;
    // }

    #endregion
}