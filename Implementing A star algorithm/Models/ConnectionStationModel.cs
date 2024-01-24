using System;
using System.Collections.Generic;

namespace Implementing_A_star_algorithm.Models;

public class ConnectionStationModel
{
    public string Name { get; set; }
    public string Id { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
    public string Zone { get; set; }
    public List<LineModel> Lines { get; set; }
    public List<TimeTable> TimeTables { get; set; }
    
    public ConnectionStationModel(string name, string id, double lat, double lon, string zone)
    {
        Name = name;
        Id = id;
        Lat = lat;
        Lon = lon;
        Zone = zone;
    }

    public ConnectionStationModel()
    {
    }

    public void AddLines(LineModel lineModel)
    {
        Lines ??= new List<LineModel>();
        Lines.Add(lineModel);
    }

    public class TimeTable
    {
        public TimeTableEnum IntervalEnum { get; set; }
        public TimeSpan FirstJourney { get; set; }
        public TimeSpan LastJourney { get; set; }
        public List<KnownJourney> KnownJourneys { get; set; }
        public List<Period> Periods { get; set; }
    }

    public class IntervalTime
    {
        public int IntervalId { get; set; }
        public string StopId { get; set; }
        public int TimeToArrival { get; set; }
    }

    public class KnownJourney
    {
        public TimeSpan Time { get; set; }
        public int IntervalId { get; set; }
    }

    public class Period
    {
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
    }

    public enum TimeTableEnum
    {
        Monday_Friday = 1,
        Saturdays_Holidays = 2,
        Sunday = 3,
        Ivalid = 0
    }
}