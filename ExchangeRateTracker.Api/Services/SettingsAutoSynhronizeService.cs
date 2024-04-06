using ExchangeRateTracker.Api.Exceptions;
using ExchangeRateTracker.Api.Models.Dtos;
using ExchangeRateTracker.Api.Services.IServices;
using System.Text.Json;

namespace ExchangeRateTracker.Api.Services
{
    public class SettingsAutoSynhronizeService : ISettingsAutoSynhronizeService
    {
        private const string _settingsFilePath = "C:\\Users\\olegp\\source\\repos\\ExchangeRateTracker\\ExchangeRateTracker.AutoSynhronize\\Settings.json";

        public async Task ChangeAsync(int dayInterval, TimeOnly time)
        {
            SettingsDto settings;

            try
            {
                using (var file = new StreamReader(_settingsFilePath))
                {
                    var text = await file.ReadToEndAsync();

                    settings = JsonSerializer.Deserialize<SettingsDto>(text);
                }
            }
            catch
            {
                throw new ChangeAutoSynhronizeSettingsException("Ошибка при чтении файла Settings.json");
            }

            settings.IntervalDays = dayInterval;
            settings.StartTime = time.ToString("HH:mm");

            try
            {
                using (var file = new StreamWriter(new FileStream(
                    _settingsFilePath,
                    FileMode.Create)))
                    await file.WriteAsync(JsonSerializer.Serialize(settings));
            }
            catch 
            {
                throw new ChangeAutoSynhronizeSettingsException("Ошибка при перезаписи файла Settings.json");
            }
        }
    }
}
