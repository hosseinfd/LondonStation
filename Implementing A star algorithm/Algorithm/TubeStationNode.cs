using System;
using Implementing_A_star_algorithm.ExcelServices;

namespace Implementing_A_star_algorithm.Algorithm;

public class TubeStationNode: IComparable<TubeStationNode>
{
    public TubeStation TubeStation { get; }
    public double FScore { get; }

    public TubeStationNode(TubeStation tubeStation, double fScore)
    {
        TubeStation = tubeStation;
        FScore = fScore;
    }

    public int CompareTo(TubeStationNode other)
    {
        return FScore.CompareTo(other.FScore);
    }
}