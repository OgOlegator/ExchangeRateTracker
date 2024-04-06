using ExchangeRateTracker.Api.Models.Dtos;

namespace ExchangeRateTracker.Api.Services.IServices
{
    /// <summary>
    /// Сервис построения отчетов по курсам валют
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// Поистроить отчет по курсам нескольких валют за период времени
        /// </summary>
        /// <param name="currencies">Валюты отчета</param>
        /// <param name="dateFrom">Дата с</param>
        /// <param name="dateTo">Дата по</param>
        /// <returns></returns>
        Task<ReportDto> BuildByCurrenciesAsync(List<string> currencies, DateOnly dateFrom, DateOnly dateTo);
    }
}
