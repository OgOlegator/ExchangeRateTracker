using ExchangeRateTracker.Api.Data;
using ExchangeRateTracker.Api.Exceptions;
using ExchangeRateTracker.Api.Models;
using ExchangeRateTracker.Api.Models.Dtos;
using ExchangeRateTracker.Api.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ExchangeRateTracker.Api.Services
{
    public class SynchronizeRatesService : ISynchronizeRatesService
    {
        private readonly AppDbContext _context;
        private readonly IBankApiService _bankApiService;
        private readonly List<string> _allowedCurrencies;

        public SynchronizeRatesService(AppDbContext context, IConfiguration configuration, IBankApiService bankApiService)
        {
            _context = context;
            _bankApiService = bankApiService;
            _allowedCurrencies = configuration.GetSection("AllowedCurrencies").Get<List<string>>();
        }

        public async Task SynchronizeByPeriod(DateOnly dateFrom, DateOnly dateTo)
        {
            var ratesByYearDocs = await GetRatesByYearsAsync(GetYearsBetweenDates(dateFrom, dateTo).ToList());

            var rates = await ParseRatesFromManyDocsByPeriodAsync(ratesByYearDocs, dateFrom, dateTo);

            await UpdateRates(rates);
        }

        public async Task SynhronizeForToday()
        {
            var result = await _bankApiService.GetRatesTodayAsync();

            if (!result.IsSuccess)
                throw new SynchronizeException(result.Message);

            await UpdateRates(ApiResponseParserService.TodayRates(result.Result));
        }

        /// <summary>
        /// Обновить курсы валют в БД
        /// </summary>
        /// <param name="rates"></param>
        /// <returns></returns>
        /// <exception cref="DbUpdateException"></exception>
        private async Task UpdateRates(List<ExchangeRate> rates)
        {
            try
            {
                _context.ExchangeRates.UpdateRange(rates);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new DbUpdateException("Ошибка при обновлении курсов в БД");
            }
        }

        /// <summary>
        /// Получить список годов между двумя датами
        /// </summary>
        /// <param name="dateFrom">Дата с</param>
        /// <param name="dateTo">Дата по</param>
        /// <returns></returns>
        private IEnumerable<int> GetYearsBetweenDates(DateOnly dateFrom, DateOnly dateTo)
        {
            var currentYear = dateFrom.Year;

            while (currentYear <= dateTo.Year) 
            {
                yield return currentYear;
                currentYear++;
            }
        }

        /// <summary>
        /// Получить курсы валют по нескольким годам
        /// </summary>
        /// <param name="years">Список годов</param>
        /// <returns></returns>
        /// <exception cref="SynchronizeException"></exception>
        private async Task<List<string>> GetRatesByYearsAsync(List<int> years)
        {
            var listTasks = new List<Task<ResultDto>>();

            foreach (var year in years)
                listTasks.Add(_bankApiService.GetRatesByYearAsync(year));

            await Task.WhenAll(listTasks);

            if (listTasks.Any(task => task.Result.IsSuccess == false))
                throw new SynchronizeException("Ошибка при получении данных банка");

            return listTasks.Select(task => task.Result.Result).ToList();
        }

        /// <summary>
        /// Парсинг курсов валют из нескольких документов по курсам за год для получения курсов по определенному периоду времени
        /// </summary>
        /// <param name="ratesDocs">Документы с курсами валют</param>
        /// <param name="dateFrom">Дата с</param>
        /// <param name="dateTo">Дата по</param>
        /// <returns></returns>
        private async Task<List<ExchangeRate>> ParseRatesFromManyDocsByPeriodAsync(List<string> ratesDocs, DateOnly dateFrom, DateOnly dateTo)
        {
            var listParseTasks = new List<Task<List<ExchangeRate>>>();

            foreach (var ratesDoc in ratesDocs)
            {
                var task = Task.Run(() => ApiResponseParserService.RatesByPeriod(ratesDoc, dateFrom, dateTo, _allowedCurrencies));
                listParseTasks.Add(task);
            };
            
            await Task.WhenAll(listParseTasks);

            var rates = new List<ExchangeRate>();

            foreach (var task in listParseTasks)
                rates.AddRange(task.Result);

            return rates;
        }
    }
}
