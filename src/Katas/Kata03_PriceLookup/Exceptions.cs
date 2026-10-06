namespace Katas.Kata03_PriceLookup;

/// <summary>A provider could not answer right now (timeout, 503...). Expected and recoverable.</summary>
public class ProviderUnavailableException : Exception
{
    public ProviderUnavailableException(string message) : base(message) { }

    public ProviderUnavailableException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>No provider could give a price for the requested SKU.</summary>
public class NoPriceAvailableException : Exception
{
    public NoPriceAvailableException(string message) : base(message) { }

    public NoPriceAvailableException(string message, Exception innerException)
        : base(message, innerException) { }
}
