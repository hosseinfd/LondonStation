using System.Collections.Generic;
using Implementing_A_star_algorithm.ExcelServices;

namespace Implementing_A_star_algorithm.Algorithm;

public class AStarPathFinder
{
    public List<TubeStation> FindPath(TubeStation start, TubeStation goal)
    {
        var closedSet = new HashSet<TubeStation>();
        var openSet = new HashSet<TubeStation> { start };
        var cameFrom = new Dictionary<TubeStation, TubeStation>();
        var gScore = new Dictionary<TubeStation, double> { { start, 0 } };
        var fScore = new Dictionary<TubeStation, double> { { start, start.DistanceTo(goal) } };

        var priorityQueue = new PriorityQueue<TubeStationNode>();
        priorityQueue.Enqueue(new TubeStationNode(start, start.DistanceTo(goal)));

        while (openSet.Count > 0)
        {
            var current = priorityQueue.Dequeue().TubeStation;
            if (current == goal)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (var neighbor in current.ConnectedTubeStations)
            {
                if (closedSet.Contains(neighbor)) continue;

                var tentativeGScore = gScore[current] + current.DistanceTo(neighbor);

                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
                else if (tentativeGScore >= gScore[neighbor])
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + neighbor.DistanceTo(goal);
                priorityQueue.Enqueue(new TubeStationNode(neighbor, fScore[neighbor]));
            }
        }

        return new List<TubeStation>(); // No path was found
    }

    private List<TubeStation> ReconstructPath(Dictionary<TubeStation, TubeStation> cameFrom, TubeStation current)
    {
        var path = new List<TubeStation> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }
}