using ExchangeRateTracker.Api.Models.Dtos;

namespace ExchangeRateTracker.Api.Services.IServices
{
    /// <summary>
    /// Сервис получения данных курса Чешской Кроны в Чешском национальном банке
    /// </summary>
    public interface IBankApiService
    {
        /// <summary>
        /// Отправить запрос к сервису 
        /// </summary>
        Task<ResultDto> SendAsync(string uri);

        /// <summary>
        /// Получить курсы на сегодняшний день
        /// </summary>
        Task<ResultDto> GetRatesTodayAsync();

        /// <summary>
        /// Получить курсы за определенный год
        /// </summary>
        Task<ResultDto> GetRatesByYearAsync(int year);
    }
}
