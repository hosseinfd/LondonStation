using System.IO;

namespace Implementing_A_star_algorithm.ExcelServices;

public class FileService
{
    static readonly string JsonFileName = "TFLStation.json";
    static readonly string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", JsonFileName);
    public void SaveJsonToFile(string json)
    {
        
        // Ensure the directory exists
        var directory = Path.GetDirectoryName(FilePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(FilePath, json);
    }

    public string ReadJsonFromFile()
    {
        if (File.Exists(FilePath))
        {
            var json = File.ReadAllText(FilePath);
            if (!string.IsNullOrWhiteSpace(json))
            {
                return json;
            }
            else
            {
                return string.Empty;
            }
        }
        else
        {
            return string.Empty;
        }
    }
}