using System;
using System.Collections.Generic;

namespace Implementing_A_star_algorithm.RetriveData;

public class TubeGraph
{
    public Dictionary<string, Station> Stations { get; } = new Dictionary<string, Station>();

    public void AddOrUpdateStation(Station station)
    {
        if (Stations.ContainsKey(station.name))
        {
            Stations[station.name] = station;
        }
        else
        {
            Stations.Add(station.name, station);
        }
    }

    public class Station
    {
        public string name { get; set; }
        public double lat { get; set; }

        public double lon { get; set; }

        public string naptanId { get; set;}
        public string stationId { get; set; }
        public string zone { get; set; }

        public List<Station> connections { get; set; } = new();
        public List<TubeLine> lines { get; set; } = new();


        public Station(string name, double latitude, double longitude, string stationId, string zone,string naptanId)
        {
            this.name = name;
            lon = longitude;
            this.stationId = stationId;
            this.zone = zone;
            lat = latitude;
            this.naptanId = naptanId;
        }

        public Station()
        {
        }

        public void Connect(Station station)
        {
            if (station is null) throw new ArgumentNullException(nameof(station));
            if (!connections.Contains(station))
            {
                connections.Add(station);
                station.connections.Add(this); // Bidirectional connection
            }
        }
    }

    public class TubeLine
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class RouteSequence
    {
        public List<StationDetail> stations { get; set; }
    }

    public class StationDetail
    {
        public string name { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public string stationId { get; set; }
        public string zone { get; set; }
    }
}