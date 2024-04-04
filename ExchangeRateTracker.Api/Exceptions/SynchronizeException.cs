namespace ExchangeRateTracker.Api.Exceptions
{
    public class SynchronizeException : Exception
    {
        public SynchronizeException(string? message) : base(message)
        {
        }
    }
}
