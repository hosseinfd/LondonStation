using System;
using System.Collections.Generic;

namespace Implementing_A_star_algorithm.Models;

public class StationModel: IComparable<StationModel>
{
    public List<LineModel> Lines { get; set; }
    public string Name { get;  set; }
    public string Id { get;  set; }
    public double Lat { get;  set; }
    public double Lon { get;  set; }
    public string Zone { get;  set; }
    public List<ConnectionStationModel> Connections { get;  set; }
    public double Priority { get; set; }
    public StationModel CameFrom { get; set; }
    public double GScore { get; set; } = double.MaxValue;
    public double FScore { get; set; } = double.MaxValue;
    public StationModel()
    {
        
    }
    public StationModel( string name, string id, double lat, double lon, string zone)
    {
        Name = name;
        Id = id;
        Lat = lat;
        Lon = lon;
        Zone = zone;
    }

    public void AddConnection(ConnectionStationModel connectionStationModel)
    {
        Connections ??= new List<ConnectionStationModel>();
        Connections.Add(connectionStationModel);
    }
    
    public void AddLine(LineModel lines)
    {
        Lines ??= new List<LineModel>();
        Lines.Add(lines);
    }
    
    public int CompareTo(StationModel other)
    {
        // Smaller Priority means higher in the PriorityQueue
        return this.Priority.CompareTo(other.Priority);
    }
}