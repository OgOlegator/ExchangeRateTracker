using ExchangeRateTracker.Api.Data;
using ExchangeRateTracker.Api.Models;
using ExchangeRateTracker.Api.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ExchangeRateTracker.Tests.Services
{
    public class ReportServiceTests
    {
        [Fact]
        public void BuildByCurrencies_ManyCurrencies_Complete()
        {
            var reportBuilder = new ReportService(GetMockContext());

            var currencies = new List<string>
            {
                "USD",
                "RUB"
            };

            var dateFrom = DateOnly.Parse(DateTime.Now.AddDays(-1).ToString("dd.MM.yyyy"));
            var dateTo = DateOnly.Parse(DateTime.Now.ToString("dd.MM.yyyy"));

            var actualReport = reportBuilder.BuildByCurrencies(currencies, dateFrom, dateTo);

            Assert.Equal(2, actualReport.Currencies.Count);

            Assert.Equal("USD", actualReport.Currencies[0].Code);
            Assert.Equal(100, actualReport.Currencies[0].MaxRate);
            Assert.Equal(95, actualReport.Currencies[0].MinRate);
            Assert.Equal((decimal)195 / 2, actualReport.Currencies[0].AvgRate);

            Assert.Equal("RUB", actualReport.Currencies[1].Code);
            Assert.Equal((decimal)12 / 11, actualReport.Currencies[1].MaxRate);
            Assert.Equal((decimal)9 / 10, actualReport.Currencies[1].MinRate);
            Assert.Equal(((decimal)12 / 11 + (decimal)9 / 10) / 2, actualReport.Currencies[1].AvgRate);
        }

        [Fact]
        public void BuildByCurrencies_IncorrectDates_Complete()
        {
            var reportBuilder = new ReportService(GetMockContext());

            var currencies = new List<string>
            {
                "USD",
                "RUB"
            };

            var dateTo = DateOnly.Parse(DateTime.Now.AddDays(-1).ToString("dd.MM.yyyy"));
            var dateFrom = DateOnly.Parse(DateTime.Now.ToString("dd.MM.yyyy"));

            var actualReport = reportBuilder.BuildByCurrencies(currencies, dateFrom, dateTo);

            Assert.Equal(0, actualReport.Currencies.Count);
        }

        [Fact]
        public void BuildByCurrencies_WithoutCurrencies_Complete()
        {
            var reportBuilder = new ReportService(GetMockContext());

            var currencies = new List<string>();

            var dateFrom = DateOnly.Parse(DateTime.Now.AddDays(-1).ToString("dd.MM.yyyy"));
            var dateTo = DateOnly.Parse(DateTime.Now.ToString("dd.MM.yyyy"));

            var actualReport = reportBuilder.BuildByCurrencies(currencies, dateFrom, dateTo);

            Assert.Equal(0, actualReport.Currencies.Count);
        }

        private AppDbContext GetMockContext()
        {
            var testData = GetTestData().AsQueryable();

            var mockSet = new Mock<DbSet<ExchangeRate>>();
            mockSet.As<IQueryable<ExchangeRate>>().Setup(m => m.Provider).Returns(testData.Provider);
            mockSet.As<IQueryable<ExchangeRate>>().Setup(m => m.Expression).Returns(testData.Expression);
            mockSet.As<IQueryable<ExchangeRate>>().Setup(m => m.ElementType).Returns(testData.ElementType);
            mockSet.As<IQueryable<ExchangeRate>>().Setup(m => m.GetEnumerator()).Returns(() => testData.GetEnumerator());

            var options = new DbContextOptionsBuilder<AppDbContext>().Options;

            var mockContext = new Mock<AppDbContext>(options);
            mockContext.Setup(c => c.ExchangeRates).Returns(mockSet.Object);

            return mockContext.Object;
        }

        private List<ExchangeRate> GetTestData()
        {
            return new List<ExchangeRate>
            {
                new ExchangeRate
                {
                    Amount = 1,
                    CurrencyCode = "USD",
                    Date = DateTime.Now.Date,
                    Rate = 100
                },
                new ExchangeRate
                {
                    Amount = 1,
                    CurrencyCode = "USD",
                    Date = DateTime.Now.Date.AddDays(-1),
                    Rate = 95
                },
                new ExchangeRate
                {
                    Amount = 10,
                    CurrencyCode = "RUB",
                    Date = DateTime.Now.Date,
                    Rate = 9
                },
                new ExchangeRate
                {
                    Amount = 11,
                    CurrencyCode = "RUB",
                    Date = DateTime.Now.Date.AddDays(-1),
                    Rate = 12
                },

            };
        }
    }
}
