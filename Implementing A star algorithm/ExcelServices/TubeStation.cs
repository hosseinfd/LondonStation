using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2010.ExcelAc;

namespace Implementing_A_star_algorithm.ExcelServices;

public class TubeStation
{
    public string Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<TubeStation> ConnectedTubeStations { get; set; } = new();
    public List<string> TubeLines { get; set; } = new();
    public List<int> ZoneNumbers { get; set; } = new();
}