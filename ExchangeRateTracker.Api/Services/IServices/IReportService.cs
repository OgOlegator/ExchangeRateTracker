using ExchangeRateTracker.Api.Models.Dtos;

namespace ExchangeRateTracker.Api.Services.IServices
{
    public interface IReportService
    {
        Task<ReportDto> GetReportByCurrenciesAsync(List<string> currencies, DateOnly dateFrom, DateOnly dateTo);
    }
}
