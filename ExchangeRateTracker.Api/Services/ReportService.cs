using ExchangeRateTracker.Api.Data;
using ExchangeRateTracker.Api.Models.Dtos;
using ExchangeRateTracker.Api.Services.IServices;

namespace ExchangeRateTracker.Api.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public ReportDto BuildByCurrencies(List<string> currencies, DateOnly dateFrom, DateOnly dateTo)
        {
            var startDate = dateFrom.ToDateTime(new TimeOnly());
            var endDate = dateTo.ToDateTime(new TimeOnly());

            var reportData = _context.ExchangeRates
                .Where(rate 
                    => currencies.Contains(rate.CurrencyCode) 
                    && rate.Date >= startDate
                    && rate.Date <= endDate)
                .ToList();

            var result = new ReportDto();

            foreach(var groupByCurrency in reportData.GroupBy(rate => rate.CurrencyCode))
                result.Currencies.Add(new ReportCurrency
                {
                    Code = groupByCurrency.Key,
                    MaxRate = groupByCurrency.Max(rate => rate.Rate / rate.Amount),
                    MinRate = groupByCurrency.Min(rate => rate.Rate / rate.Amount),
                    AvgRate = groupByCurrency.Sum(rate => rate.Rate / rate.Amount) / groupByCurrency.Count()
                });

            return result;
        }
    }
}
