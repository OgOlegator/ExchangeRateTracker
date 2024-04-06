using ExchangeRateTracker.AutoSynhronize.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace ExchangeRateTracker.AutoSynhronize.Services
{
    public class SynhronizeRateSevice
    {
        /// <summary>
        /// Проверка можно ли запускать синхронизацию 
        /// </summary>
        /// <returns></returns>
        private async Task<bool> StartOffAsync()
        {
            var settings = await ReadSettingsFileAsync();

            var lastStartDate = DateTime.Parse(settings.LastStart);

            if (lastStartDate == DateTime.Now.Date)
                return false;

            if (lastStartDate.AddDays(settings.IntervalDays) != DateTime.Now.Date)
                return false;

            if (DateTime.Now.ToString("HH:mm") != settings.StartTime)
                return false;

            return true;
        }

        /// <summary>
        /// Запуск синхронизации курса за текущий день
        /// </summary>
        /// <returns></returns>
        public async Task<ResultSynhronize> ExecuteAsync()
        {
            var result = new ResultSynhronize();

            if (!await StartOffAsync())
            {
                result.IsSuccess = false;
                result.Message = "Условия синхронизации не выполнены";
                return result;
            }

            var client = new HttpClient();

            var dateSynhronize = DateTime.Now.ToString("dd.MM.yyyy");

            var message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");

            //Для избежания ошибки проверки сертификата запрос идет по http
            message.RequestUri = new Uri($"http://localhost:5008/api/RateTracker/Synchronize/ByDate/{dateSynhronize}");
            message.Method = HttpMethod.Post;

            try
            {
                var apiResponse = await client.SendAsync(message);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    result.IsSuccess = false;
                    result.Message = await apiResponse.Content.ReadAsStringAsync();

                    return result;
                }
            }
            catch(HttpRequestException ex)
            {
                result.IsSuccess = false;
                result.Message = $"Ошибка при обращении к АПИ. {ex.Message}";
                return result;
            }

            result.Message = $"Синхронизация на дату {dateSynhronize} выполнена успешно";

            await ChangeLastDateInSettingsAsync(dateSynhronize);

            return result;
        }

        /// <summary>
        /// Обновления файла Settings.json. Установка новой даты последнего запуска
        /// </summary>
        /// <param name="newLastDate">Новая дата последнего запуска синхронизации</param>
        /// <returns></returns>
        /// <exception cref="Exception">Не удалось обновить файл</exception>
        private async Task ChangeLastDateInSettingsAsync(string newLastDate)
        {
            var settings = await ReadSettingsFileAsync();
            settings.LastStart = newLastDate;

            try
            {
                using (var file = new StreamWriter(new FileStream(
                    $"C:\\Users\\olegp\\source\\repos\\ExchangeRateTracker\\ExchangeRateTracker.AutoSynhronize\\Settings.json",
                    FileMode.Create)))
                {
                    await file.WriteAsync(JsonSerializer.Serialize(settings));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при перезаписи файла Settings.json", ex);
            }
        }

        /// <summary>
        /// Чтение настроек из файла Settings.json
        /// </summary>
        /// <returns>Настройки</returns>
        private async Task<Settings> ReadSettingsFileAsync()
        {
            Settings settings;

            using (var file = new StreamReader(
                "C:\\Users\\olegp\\source\\repos\\ExchangeRateTracker\\ExchangeRateTracker.AutoSynhronize\\Settings.json"))
            {
                var text = await file.ReadToEndAsync();

                settings = JsonSerializer.Deserialize<Settings>(text);
            }

            return settings;
        }
    }
}
