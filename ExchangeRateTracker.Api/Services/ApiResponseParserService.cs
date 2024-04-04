using ExchangeRateTracker.Api.Models;
using System.Globalization;

namespace ExchangeRateTracker.Api.Services
{
    /// <summary>
    /// Парсеры ответов API Чешского национального банка
    /// </summary>
    public class ApiResponseParserService
    {
        /// <summary>
        /// Парсинг курсов на сегодняшний день
        /// </summary>
        /// <param name="ratesDoc">Документ с курсами валют за сегодняшний день</param>
        /// <returns></returns>
        public static List<ExchangeRate> TodayRates(string ratesDoc)
        {
            var result = new List<ExchangeRate>();
            var rows = ratesDoc.Split('\n');

            var today = DateTime.Now.Date;

            foreach (var row in rows.Skip(2).SkipLast(1))    //В первых дувх строках Дата и заголовок
            {
                var rowVals = row.Split("|");

                result.Add(new ExchangeRate
                {
                    CurrencyCode = rowVals[3],
                    Date = today,
                    Amount = ConvertStringToDecimal(rowVals[2]),
                    Rate = ConvertStringToDecimal(rowVals[4]),
                });
            }

            return result;
        }

        /// <summary>
        /// Парсинг курсов по датам на определенный период
        /// </summary>
        /// <param name="ratesDoc">Документ с курсами валют по датам</param>
        /// <param name="dateFrom">Дата с</param>
        /// <param name="dateTo">Дата по</param>
        /// <param name="allowedCurrencies">Нужные валюты</param>
        /// <returns></returns>
        public static List<ExchangeRate> RatesByPeriod(string ratesDoc, DateOnly dateFrom, DateOnly dateTo, List<string> allowedCurrencies)
        {
            var result = new List<ExchangeRate>();
            var rows = ratesDoc.Split('\n');

            var header = ParseHeader(rows.First());

            foreach (var row in rows.Skip(1).SkipLast(1))
            {
                var rowVals = row.Split("|");
                var dateRate = DateOnly.Parse(rowVals[0]);

                if (dateRate <= dateFrom && dateRate >= dateTo)
                    foreach (var columnHeader in header.Where(column => allowedCurrencies.Contains(column.Value[1])))
                        result.Add(new ExchangeRate
                        {
                            CurrencyCode = columnHeader.Value[1],
                            Date = dateRate.ToDateTime(new TimeOnly()),
                            Amount = ConvertStringToDecimal(columnHeader.Value[0]),
                            Rate = ConvertStringToDecimal(rowVals[columnHeader.Key]),
                        });
            }

            return result;

            Dictionary<int, string[]> ParseHeader(string headerRow)
            {
                var result = new Dictionary<int, string[]>();
                var columns = headerRow.Split("|");

                for (var i = 1; i < columns.Count(); i++)
                    result[i] = columns[i].Split(" ");

                return result;
            }
        }

        private static decimal ConvertStringToDecimal(string value)
            => decimal.Parse(value.Replace(".", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator), CultureInfo.InvariantCulture);
    }
}
