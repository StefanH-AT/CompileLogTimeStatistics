using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

IEnumerable<string> logFiles = Directory.GetFiles(".").Where(f => f.EndsWith(".log"));

Dictionary<string, TimeSpan> stats = new Dictionary<string, TimeSpan>();

string style = @"* {
  margin: 0;
  padding: 0;
  font-family: sans-serif;
}

.compile-stats {
  width: 50%;
}

.compile-stats-item {
  display: flex;
  flex-direction: row;
  justify-content: flex-start;
  
  margin: 10px;
}

.compile-stats-item-data {
  position: relative;
  width: 100%;
}

.compile-stats-item-name {
  width: 600px;
}

.compile-stats-item-time {
  width: 200px;
}

.compile-stats-item-bar {
  position: absolute;
  
  display: inline-block;
  background-color: rgba(200, 0, 0, 100);
  height: 20px;
  top: 25%;
}";

string webTable = $"<html><head><style>{style}</style></head><body><div class=\"compile-stats\">";

foreach (string file in logFiles)
{
    string mapName = file.Replace(".log", "");
    Console.WriteLine($"Processing {mapName}");

    if ((File.GetAttributes(file) & FileAttributes.ReadOnly) != 0)
    {
        Console.WriteLine(" > Read only. Skipping");
        continue;
    }
    
    string[] lines = File.ReadAllLines(file);
    string elapsedLine = lines.Last(s => s.EndsWith("elapsed"));

    string[] splits = elapsedLine.Split(",");

    string minuteString, secondString;
    int minutes, seconds;

    if (splits.Length >= 2)
    {
        minuteString = splits[0];
        secondString = splits[1];
        minutes = int.Parse(minuteString.Remove(minuteString.IndexOf("minute", StringComparison.Ordinal)).Trim());
        seconds = int.Parse(secondString.Remove(secondString.IndexOf("second", StringComparison.Ordinal)).Trim());
    }
    else
    {
        secondString = splits[0];
        minutes = 0;
        seconds = int.Parse(secondString.Remove(secondString.IndexOf("second", StringComparison.Ordinal)).Trim());
    }

    
    stats.Add(mapName.Trim().Replace(@".\", ""), new TimeSpan(0, minutes, seconds));
}

double maxTime = stats.Values.Max(t => t.TotalSeconds);

foreach (var data in stats)
{
    string mapName = data.Key;
    var time = data.Value;
    string width = Convert.ToString(time.TotalSeconds / maxTime * 100, CultureInfo.InvariantCulture);
    webTable += @"<div class=""compile-stats-item"">";
    webTable += $"<p class=\"compile-stats-item-name\">{mapName}</p>";
    webTable += $"<p class=\"compile-stats-item-time\">{time.Minutes}m {time.Seconds}s</p>";
    webTable += @"<div class=""compile-stats-item-data"">";
    webTable += $"<span class=\"compile-stats-item-bar\" style=\"width: {width}%\"></span>";
    webTable += "</div></div>";
}

webTable += "</div></body></html>";

File.WriteAllText("compile_stats.json", JsonSerializer.Serialize(stats));
File.WriteAllText("compile_stats.html", webTable);
