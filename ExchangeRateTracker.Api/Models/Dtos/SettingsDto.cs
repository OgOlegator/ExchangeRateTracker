namespace ExchangeRateTracker.Api.Models.Dtos
{
    public class SettingsDto
    {
        public string LastStart { get; set; }

        public string StartTime { get; set; }

        public int IntervalDays { get; set; }
    }
}
