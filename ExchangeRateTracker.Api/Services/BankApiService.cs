using ExchangeRateTracker.Api.Exceptions;
using ExchangeRateTracker.Api.Models.Dtos;
using ExchangeRateTracker.Api.Services.IServices;

namespace ExchangeRateTracker.Api.Services
{
    public class BankApiService : IBankApiService
    {
        private readonly IHttpClientFactory _httpClient;

        public BankApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory;
        }

        public async Task<ResultDto> GetRatesByYearAsync(int year)
        {
            return await SendAsync(
                $"https://www.cnb.cz/en/financial_markets/foreign_exchange_market/exchange_rate_fixing/year.txt?year={year}");
        }

        public async Task<ResultDto> GetRatesTodayAsync()
        {
            return await SendAsync(
                $"https://www.cnb.cz/en/financial_markets/foreign_exchange_market/exchange_rate_fixing/daily.txt?date={DateTime.Now.ToString("dd.MM.yyyy")}");
        }

        public async Task<ResultDto> SendAsync(string uri)
        {
            var result = new ResultDto();

            var client = _httpClient.CreateClient("ExchangeRateTracker");

            var message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");

            //TODO Uri приходит на вход методу
            message.RequestUri = new Uri(uri);
            message.Method = HttpMethod.Get;

            var apiResponse = await client.SendAsync(message);

            if (!apiResponse.IsSuccessStatusCode)
            {
                result.IsSuccess = false;

                if (apiResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    result.Message = "Сервис получения данных о курсках не доступен";
                else
                    result.Message = "Не удалось получить данные о курсе";
            }
            else
                result.Result = await apiResponse.Content.ReadAsStringAsync();

            return result;
        }
    }
}
