using System;
using System.Collections.Generic;

namespace Implementing_A_star_algorithm.ExcelServices;

public class TubeStation
{
    public string Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<TubeStation> ConnectedTubeStations { get; set; } = new List<TubeStation>();
    public List<string> TubeLines { get; set; } = new List<string>();
    public List<int> ZoneNumbers { get; set; } = new List<int>();
    
    public double DistanceTo(TubeStation other)
    {
        var dLat = (other.Latitude - this.Latitude) * Math.PI / 180.0;
        var dLon = (other.Longitude - this.Longitude) * Math.PI / 180.0;

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(this.Latitude * Math.PI / 180.0) * Math.Cos(other.Latitude * Math.PI / 180.0) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var distance = 6371 * c;  // Radius of earth in km

        return distance;
    }
}