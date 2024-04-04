namespace ExchangeRateTracker.Api.Services.IServices
{
    /// <summary>
    /// Сервис синхронизации данных по курсам
    /// </summary>
    public interface ISynchronizeRatesService
    {
        /// <summary>
        /// Синхронизировать курсы за сегодня
        /// </summary>
        /// <returns></returns>
        Task SynhronizeForToday();

        /// <summary>
        /// Синхронизировать курсы за период времени
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        Task SynchronizeByPeriod(DateOnly dateFrom, DateOnly dateTo);
    }
}
