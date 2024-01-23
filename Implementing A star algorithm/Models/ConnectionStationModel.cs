using System.Collections.Generic;

namespace Implementing_A_star_algorithm.Models;

public class ConnectionStationModel
{
    public string Name { get; private set; }
    public string Id { get; private set; }
    public double Lat { get; private set; }
    public double Lon { get; private set; }
    public string Zone { get; private set; }
    public List<LineModel> Lines { get; private set; }

    public ConnectionStationModel(string name, string id, double lat, double lon, string zone)
    {
        Name = name;
        Id = id;
        Lat = lat;
        Lon = lon;
        Zone = zone;
    }

    public void AddLines(LineModel lineModel)
    {
        Lines ??= new List<LineModel>();
        Lines.Add(lineModel);
    }
}