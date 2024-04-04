namespace ExchangeRateTracker.Api.Models.Dtos
{
    public class ReportDto
    {
        public List<ReportCurrency> Currencies { get; set; } = new List<ReportCurrency>();
    }

    public class ReportCurrency
    {
        public string Code { get; set; }

        public decimal MinRate { get; set; }

        public decimal MaxRate { get; set; }

        public decimal AvgRate { get; set; }
    }
}
