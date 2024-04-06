using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateTracker.AutoSynhronize.Models
{
    public class Settings
    {
        public string LastStart { get; set; }

        public string StartTime { get; set; }

        public int IntervalDays { get; set; }
    }
}
