using System.Collections.Generic;

namespace Implementing_A_star_algorithm.Models;

public class StationModel
{
    public List<LineModel> Lines { get;private set; }
    public string Name { get; private set; }
    public string Id { get; private set; }
    public double Lat { get; private set; }
    public double Lon { get; private set; }
    public string Zone { get; private set; }
    public List<ConnectionStationModel> Connections { get; private set; }

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
}