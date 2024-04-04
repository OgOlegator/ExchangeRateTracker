namespace ExchangeRateTracker.Api.Models.Dtos
{
    public class ResultDto
    {
        public bool IsSuccess { get; set; } = true;

        public string Message { get; set; }

        public string Result {  get; set; }
    }
}
