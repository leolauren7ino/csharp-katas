namespace Katas.Kata03_PriceLookup;

/// <summary>
/// A remote source of prices (think: a supplier API). Calls are slow, I/O-bound and can fail.
/// </summary>
public interface IPriceProvider
{
    Task<decimal> GetPriceAsync(string sku, CancellationToken cancellationToken);
}
