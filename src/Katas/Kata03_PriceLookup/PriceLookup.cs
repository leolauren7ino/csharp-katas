namespace Katas.Kata03_PriceLookup;

public static class PriceLookup
{
    // TODO: implement with async/await. See README.md in this folder.
    // Note: the signature below is the contract the tests call. You will need to change
    // how the method is declared (hint: one keyword) to be able to use await inside it.
    public static async Task<decimal> GetBestPriceAsync(
        string sku,
        IEnumerable<IPriceProvider> providers,
        CancellationToken cancellationToken = default) 
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        ArgumentNullException.ThrowIfNull(providers);

        var lookups = providers
        .Select(provider => TryGetPriceAsync(provider, sku, cancellationToken))
        .ToList();

        decimal?[] results = await Task.WhenAll(lookups);

        var prices = results
            .Where(price => price.HasValue)
            .Select(price => price.Value)
            .ToList();

        if (prices.Count == 0)
        {
            throw new NoPriceAvailableException($"No price available for SKU '{sku}'.");
        }

        return prices.Min();
    }

    private static async Task<decimal?> TryGetPriceAsync(
    IPriceProvider provider, string sku, CancellationToken cancellationToken)
    {
        try
        {
            return await provider.GetPriceAsync(sku, cancellationToken);
        }
        catch (ProviderUnavailableException)
        {
            return null;
        }
    }
}
