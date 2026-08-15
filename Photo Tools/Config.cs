using System;
using System.IO;
using System.Text.Json;

namespace Photo_Tools
{
    public class AppConfig
    {
        public string? DbLocation { get; set; }
        public string? SqlScript { get; set; }
        public string? SampleDataPath { get; set; }

        public static AppConfig Load(string path = "appsettings.json")
        {
            if (!File.Exists(path))
            {
                // Return defaults if no config file is present
                return new AppConfig
                {
                    DbLocation = "D:/Scratch/PhotoTools.db",
                    SqlScript = "D:/Scratch/Create_PhotoTools DB.sql",
                    SampleDataPath = "D:/Scratch/PhotoTools Samples/PhotoSampleData"
                };
            }

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
        }
    }
}
