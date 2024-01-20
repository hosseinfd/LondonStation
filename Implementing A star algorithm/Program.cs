using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Implementing_A_star_algorithm.Algorithm;
using Implementing_A_star_algorithm.ExcelServices;
using Microsoft.Extensions.DependencyInjection;

namespace Implementing_A_star_algorithm
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var tubeGraph = new Graph();
            var kingsCross = tubeGraph.AddNode("Kings Cross", 51.531724f, -0.124606f);
            var euston = tubeGraph.AddNode("Euston", 51.528099f, -0.133176f);
            var russellSquare = tubeGraph.AddNode("Russell Square", 51.52175f, -0.12589f);
            var pretendStation = tubeGraph.AddNode("Pretend", 51.52175f, -0.12589f);
            kingsCross.AddEdge(euston, 2);
            kingsCross.AddEdge(russellSquare, 3);
            pretendStation.AddEdge(euston, 1);
            pretendStation.AddEdge(russellSquare, 1);

            var result = tubeGraph.AStar(russellSquare, euston);
            foreach (var node in result)
            {
                Console.WriteLine(node.GetName());
            }
            
            //define service here
            var excelService = new ExcelService();
            
            
            // fetch data from tfl and save them into excel file
            
            // var _tubeGraph = new TubeGraph();
            // var sessionManager = new StationManager(_tubeGraph);
            // await sessionManager.PopulateGraphAsync();
            // excelService.SaveToExcel(_tubeGraph, "D:\\Implementing-A-star-algorithm\\TubeData.xlsx");
            
            
            // run the algorithm
            var pathFinder = new AStarPathFinder();
            var tubeStations = excelService.ImportFromExcel("D:\\Implementing-A-star-algorithm\\TubeData.xlsx");
            var startStation =
                tubeStations.First(s => s.Name == "Baker Street Underground Station"); // replace with your actual station name
            var goalStation =
                tubeStations.First(s => s.Name == "Seven Sisters"); // replace with your actual station name
            var path = pathFinder.FindPath(startStation, goalStation);

            foreach (var station in path)
            {
                Console.WriteLine(station.Name);
            }
        }
    }
}