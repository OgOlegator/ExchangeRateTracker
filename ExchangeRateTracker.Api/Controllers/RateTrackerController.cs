using ExchangeRateTracker.Api.Exceptions;
using ExchangeRateTracker.Api.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRateTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RateTrackerController : ControllerBase
    {
        private readonly ISynchronizeRatesService _synchronizeService;
        private readonly IReportService _reportService;
        private readonly ISettingsAutoSynhronizeService _settingsService;

        public RateTrackerController(ISynchronizeRatesService synchronizeService, IReportService reportService, ISettingsAutoSynhronizeService settingsService)
        {
            _synchronizeService = synchronizeService;
            _reportService = reportService;
            _settingsService = settingsService;
        }

        /// <summary>
        /// Настройка запуска авто синхронизации курсов
        /// </summary>
        /// <param name="dayInterval">Интервал запуска в днях</param>
        /// <param name="time">Время запуска</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Synchronize/Setting/{dayInterval} {time}")]
        public async Task<IActionResult> ChangeAutoSynchronizeSettings(int dayInterval, string time)
        {
            try
            {
                await _settingsService.ChangeAsync(dayInterval, TimeOnly.Parse(time));

                return Ok();
            }
            catch (FormatException)
            {
                return BadRequest("Некорректный формат входных данных");
            }
            catch (ChangeAutoSynhronizeSettingsException ex)
            {
                return Conflict(ex.Message);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Синхронизировать курсы на сегодняшний день
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Synchronize/ByDate/{date}")]
        public async Task<IActionResult> SynhronizeByDay(string date)
        {
            try
            {
                await _synchronizeService.SynhronizeByDayAsync(DateOnly.Parse(date));

                return Ok();
            }
            catch (FormatException)
            {
                return BadRequest("Некорректный формат входных данных");
            }
            catch (DbUpdateException)
            {
                return Conflict();
            }
            catch (SynchronizeException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Синхронизировать курсы за указанный период
        /// </summary>
        /// <param name="startDate">Дата с</param>
        /// <param name="endDate">Дата по</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Synchronize/ByPeriod/{startDate} {endDate}")]
        public async Task<IActionResult> SynchronizeByPeriod(string startDate, string endDate)
        {
            try
            {
                var dateFrom = DateOnly.Parse(startDate);
                var dateTo = DateOnly.Parse(endDate);

                if (dateFrom > dateTo)
                    throw new FormatException();

                await _synchronizeService.SynchronizeByPeriodAsync(dateFrom, dateTo);

                return Ok();
            }
            catch (FormatException)
            {
                return BadRequest("Некорректный формат входных данных");
            }
            catch (DbUpdateException)
            {
                return Conflict();
            }
            catch (SynchronizeException ex) 
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Получить отчет по валютам. Валюты перечисляются через &
        /// </summary>
        /// <param name="currencies"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        [HttpGet]
        [Route("Report/{currencies} {startDate} {endDate}")]
        public async Task<IActionResult> GetReport(string currencies, string startDate, string endDate)
        {
            try
            { 
                var dateFrom = DateOnly.Parse(startDate);
                var dateTo = DateOnly.Parse(endDate);

                if (dateFrom > dateTo)
                    throw new FormatException();

                var currenciesList = currencies.Split('&').ToList();

                return Ok(_reportService.BuildByCurrencies(currenciesList, dateFrom, dateTo));
            }
            catch (FormatException)
            {
                return BadRequest("Некорректный формат входных данных");
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
