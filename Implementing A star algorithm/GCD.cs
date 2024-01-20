using System;

namespace Implementing_A_star_algorithm
{
    class GreatCircleDistance
    {
        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private double GreatCircleDistanceCalc(double lat1, double lon1, double lat2, double lon2) //Calculates the great circle distance between two locations
        {
            const double radius = 6371; //Radius of the Earth in km
            lat1 = DegreesToRadians(lat1);
            lon1 = DegreesToRadians(lon1);
            lat2 = DegreesToRadians(lat2);
            lon2 = DegreesToRadians(lon2);
            double d_lat = lat2 - lat1;
            double d_lon = lon2 - lon1;
            double h = Math.Sin(d_lat / 2) * Math.Sin(d_lat / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(d_lon / 2) * Math.Sin(d_lon / 2); //Haversine formula
            return Math.Asin(Math.Sqrt(h)) * radius * 2;
        }
       
        private double ParseLatLon(string str)
        {
            const string Degree = "°";
            str = str.ToUpper().Replace(Degree, " ").Replace("'", " ").Replace("\"", " ");
            str = str.Replace("S", " S").Replace("N", " N");
            str = str.Replace("E", " E").Replace("W", " W");
            char[] separators = { ' ' };
            string[] fields = str.Split(separators, StringSplitOptions.RemoveEmptyEntries);
             
            double result = double.Parse(fields[0]); //Degrees
            if (fields.Length > 2)      
                result += double.Parse(fields[1]) / 60; //Minutes
            if (fields.Length > 3)      
                result += double.Parse(fields[2]) / 3600; //Seconds
            if (str.Contains('S') || str.Contains('W')) result *= -1;
            return result;
        }
    }
}
