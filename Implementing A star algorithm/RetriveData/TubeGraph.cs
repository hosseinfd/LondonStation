using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Implementing_A_star_algorithm.ExcelServices;

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

    public void AddDepartureTimesToStation(string key , List<DepartureInfo> departureInfo)
    {
        Stations[key].InboundDepartures = departureInfo.Where(d => d.direction == "inbound").ToList();
        Stations[key].OutboundDepartures = departureInfo.Where(d => d.direction == "outbound").ToList();
    }
    
    public class Station
    {
        public string name { get; set; }
        public double lat { get; set; }

        public double lon { get; set; }

        public string naptanId { get; set; }
        public string stationId { get; set; }
        public string zone { get; set; }

        public List<Station> connections { get; set; } = new();
        public List<TubeLine> lines { get; set; } = new();
        public List<DepartureInfo> InboundDepartures { get; set; } = new List<DepartureInfo>();
        public List<DepartureInfo> OutboundDepartures { get; set; } = new List<DepartureInfo>();

        public Station(string name, double latitude, double longitude, string stationId, string zone, string naptanId)
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
        public List<TubeLineService> serviceTypes { get; set; }
    }

    public class TubeLineService
    {
        public string name { get; set; }
        public string uri { get; set; }
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

    public class DepartureInfo
    {
        public string lineName { get; set; }
        public string destinationName { get; set; }
        public DateTime expectedArrival { get; set; }
        public string direction { get; set; }
        public TimeSpan timeToStationInMiunute { get; set; }
        public int timeToStation { get; set; }

    }
    
    public List<TubeStation> ConvertToTubeStations()
    {
        var tubeStations = new List<TubeStation>();

        foreach (var station in Stations.Values)
        {
            var tubeStation = new TubeStation
            {
                Name = station.name,
                Latitude = station.lat,
                Longitude = station.lon,
                ConnectedTubeStations = station.connections.Select(c => new TubeStation
                {
                    Name = c.name,
                    Latitude = c.lat,
                    Longitude = c.lon
                    // Note: For simplicity, only basic details are copied here
                }).ToList(),
                TubeLines = station.lines.Select(l => l.name).ToList(),
                ZoneNumbers = ParseZoneNumbers(station.zone)
            };

            tubeStations.Add(tubeStation);
        }

        return tubeStations;
    }
    
    private List<int> ParseZoneNumbers(string zoneString)
    {
        if (string.IsNullOrEmpty(zoneString))
        {
            return new List<int>();
        }

        var zones = new List<int>();
        var zoneParts = zoneString.Split('+');

        foreach (var part in zoneParts)
        {
            if (int.TryParse(part, out int zone))
            {
                zones.Add(zone);
            }
            else
            {
                // Handle the case where the part is not an integer
                // You might want to log this case or throw an exception
            }
        }

        return zones;
    }
    
}