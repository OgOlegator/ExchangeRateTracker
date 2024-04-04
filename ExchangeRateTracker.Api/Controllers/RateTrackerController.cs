using ExchangeRateTracker.Api.Exceptions;
using ExchangeRateTracker.Api.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRateTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RateTrackerController : ControllerBase
    {
        private readonly ISynchronizeRatesService _synchronizeService;

        public RateTrackerController(ISynchronizeRatesService synchronizeService)
        {
            _synchronizeService = synchronizeService;
        }

        /// <summary>
        /// Настройка запуска авто синхронизации курсов
        /// </summary>
        /// <param name="dayInterval"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Synchronize/Setting/{dayInterval} {time}")]
        public async Task<IActionResult> ChangeAutoSynchronizeSettings(int dayInterval, string time)
        {

            return Ok();
        }

        /// <summary>
        /// Синхронизировать курсы на сегодняшний день
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Synchronize/Today")]
        public async Task<IActionResult> SynhronizeToday()
        {
            try
            {
                await _synchronizeService.SynhronizeForToday();

                return Ok();
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
        [Route("Synchronize/period/{startDate} {endDate}")]
        public async Task<IActionResult> SynchronizeByPeriod(string startDate, string endDate)
        {
            try
            {
                var dateFrom = DateOnly.Parse(startDate);
                var dateTo = DateOnly.Parse(endDate);

                if (dateFrom > dateTo)
                    throw new FormatException();

                await _synchronizeService.SynchronizeByPeriod(dateFrom, dateTo);

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
        /// <returns></returns>
        [HttpGet]
        [Route("Report/{currencies}")]
        public async Task<IActionResult> GetReport(string currencies)
        {
            var currenciesList = currencies.Split('&');

            return Ok();
        }
    }
}
