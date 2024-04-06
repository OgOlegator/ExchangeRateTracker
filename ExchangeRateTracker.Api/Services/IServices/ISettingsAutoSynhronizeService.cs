namespace ExchangeRateTracker.Api.Services.IServices
{
    /// <summary>
    /// Сервис работы с настройками запуска автоматической синхронизации
    /// </summary>
    public interface ISettingsAutoSynhronizeService
    {
        /// <summary>
        /// Изменить настройки
        /// </summary>
        /// <param name="dayInterval">новый интервал в днях</param>
        /// <param name="time">новое время запуска</param>
        /// <returns></returns>
        Task ChangeAsync(int dayInterval, TimeOnly time);

    }
}
