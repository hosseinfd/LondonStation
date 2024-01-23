using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Implementing_A_star_algorithm.Algorithm;
using Implementing_A_star_algorithm.ExcelServices;
using Implementing_A_star_algorithm.RetriveData;
using Microsoft.Extensions.DependencyInjection;

namespace Implementing_A_star_algorithm
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sessionManager = new StationManager();
            var tubeGraph = await sessionManager.PopulateGraphAsync();
            

            var stationTest = tubeGraph
                .Where(q => q.Name == "Baker Street Underground Station")
                .Select(q => q);
            var json = JsonSerializer.Serialize(stationTest);

            Console.WriteLine(json);
            // run the algorithm
            // var pathFinder = new PathFinder();
            // var startStation =
            //     stationTest.First(s => s.Name == "Baker Street Underground Station"); // replace with your actual station name
            // var goalStation =
            //     tubeStations.First(s => s.Name == "Seven Sisters"); // replace with your actual station name
            // var path = pathFinder.FindPathWithHeuristic(startStation, goalStation);
            //
            // foreach (var station in path)
            // {
            //     Console.WriteLine(station.Name);
            // }
        }
    }
}