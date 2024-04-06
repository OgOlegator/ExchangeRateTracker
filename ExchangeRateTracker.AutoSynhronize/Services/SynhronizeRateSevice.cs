using ExchangeRateTracker.AutoSynhronize.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExchangeRateTracker.AutoSynhronize.Services
{
    public class SynhronizeRateSevice
    {
        private async Task<bool> StartOff()
        {
            Settings settings;

            using (var file = new StreamReader(
                "C:\\Users\\olegp\\source\\repos\\ExchangeRateTracker\\ExchangeRateTracker.AutoSynhronize\\Settings.json"))
            {
                var text = await file.ReadToEndAsync();

                settings = JsonSerializer.Deserialize<Settings>(text);
            }

            var lastStartDate = DateTime.Parse(settings.LastStart);

            if (lastStartDate == DateTime.Now.Date)
                return false;

            if (lastStartDate.AddDays(settings.IntervalDays) != DateTime.Now.Date)
                return false;

            if (DateTime.Now.ToString("HH:mm") != settings.StartTime)
                return false;

            return true;
        }

        public async Task Execute()
        {
            if (!await StartOff())
                return;


        }
    }
}
