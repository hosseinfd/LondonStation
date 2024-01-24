using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Implementing_A_star_algorithm.Algorithm;
using Implementing_A_star_algorithm.ExcelServices;
using Implementing_A_star_algorithm.Models;
using Implementing_A_star_algorithm.RetriveData;
using Microsoft.Extensions.DependencyInjection;

namespace Implementing_A_star_algorithm
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sessionManager = new StationManager();
            var fileService = new FileService();
            var jsonFromFile = fileService.ReadJsonFromFile();
            List<StationModel> stationModels;
            if (string.IsNullOrWhiteSpace(jsonFromFile))
            {
                stationModels = await sessionManager.PopulateGraphAsync();
                fileService.SaveJsonToFile(JsonSerializer.Serialize(stationModels));
            }
            else
            {
                stationModels = JsonSerializer.Deserialize<List<StationModel>>(jsonFromFile);
            }

            // run the algorithm
            var startStation =
                stationModels.Where(s => s.Name == "Preston Road Underground Station")
                    .Select(q => new StationLineKey(q.Id, q.Lines.FirstOrDefault().LineId))
                    .FirstOrDefault();
            var goalStation =
                stationModels.Where(s => s.Name == "North Harrow Underground Station")
                    .Select(q => new StationLineKey(q.Id, q.Lines.FirstOrDefault().LineId)).FirstOrDefault();

            var pathFinder = new PathFinder(stationModels);
            var result = pathFinder.FindBestPath(startStation, goalStation);

            var resultJson = JsonSerializer.Serialize(result);
            foreach (var station in result)
            {
                Console.WriteLine(station.LineName);
            }
        }
    }
}