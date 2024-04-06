

// See https://aka.ms/new-console-template for more information
using ExchangeRateTracker.ConsoleApp;
using System.Text.Json;

Settings settings;

using (var file = new StreamReader("C:\\Users\\olegp\\source\\repos\\ExchangeRateTracker\\ExchangeRateTracker.AutoSynhronize\\Settings.json"))
{
    var text = await file.ReadToEndAsync();

    settings = JsonSerializer.Deserialize<Settings>(text);
}

var time = DateTime.Now.ToString("HH:mm");

if (time == settings.StartTime)
{
    var t = 2;
}
