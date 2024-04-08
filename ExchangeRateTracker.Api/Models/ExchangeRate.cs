
namespace ExchangeRateTracker.Api.Models
{
    public class ExchangeRate
    {
        public string CurrencyCode { get; set; }

        public DateTime Date {  get; set; }

        public decimal Amount { get; set; }

        public decimal Rate { get; set; }
    }
}
