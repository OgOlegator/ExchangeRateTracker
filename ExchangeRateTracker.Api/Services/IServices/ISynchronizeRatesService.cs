namespace ExchangeRateTracker.Api.Services.IServices
{
    /// <summary>
    /// Сервис синхронизации данных по курсам
    /// </summary>
    public interface ISynchronizeRatesService
    {
        /// <summary>
        /// Синхронизировать курсы на дату
        /// </summary>
        /// <param name="date">Дата синхронизации</param>
        Task SynhronizeByDayAsync(DateOnly date);

        /// <summary>
        /// Синхронизировать курсы за период времени
        /// </summary>
        /// <param name="dateFrom">Дата с</param>
        /// <param name="dateTo">Дата по</param>
        Task SynchronizeByPeriodAsync(DateOnly dateFrom, DateOnly dateTo);
    }
}
