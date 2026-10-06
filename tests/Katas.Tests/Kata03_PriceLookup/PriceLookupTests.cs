using System.Collections;
using Katas.Kata03_PriceLookup;

namespace Katas.Tests.Kata03_PriceLookup;

public class PriceLookupTests
{
    private sealed class FakeProvider(Func<string, CancellationToken, Task<decimal>> handler) : IPriceProvider
    {
        public Task<decimal> GetPriceAsync(string sku, CancellationToken cancellationToken) =>
            handler(sku, cancellationToken);
    }

    private sealed class CountingEnumerable<T>(IReadOnlyList<T> items) : IEnumerable<T>
    {
        public int EnumerationCount { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            EnumerationCount++;
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private static IPriceProvider Returns(decimal price) =>
        new FakeProvider((_, _) => Task.FromResult(price));

    private static IPriceProvider Unavailable() =>
        new FakeProvider((_, _) => Task.FromException<decimal>(new ProviderUnavailableException("down")));

    [Fact]
    public async Task Null_sku_throws()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => PriceLookup.GetBestPriceAsync(null!, [Returns(1m)]));
    }

    [Fact]
    public async Task Blank_sku_throws()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => PriceLookup.GetBestPriceAsync("   ", [Returns(1m)]));
    }

    [Fact]
    public async Task Null_providers_throws()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => PriceLookup.GetBestPriceAsync("SKU-1", null!));
    }

    [Fact]
    public async Task Returns_the_lowest_price()
    {
        var price = await PriceLookup.GetBestPriceAsync("SKU-1", [Returns(30m), Returns(10m), Returns(20m)]);

        Assert.Equal(10m, price);
    }

    [Fact]
    public async Task Skips_unavailable_providers()
    {
        var price = await PriceLookup.GetBestPriceAsync("SKU-1", [Unavailable(), Returns(25m), Unavailable()]);

        Assert.Equal(25m, price);
    }

    [Fact]
    public async Task Unexpected_provider_errors_propagate_with_their_original_type()
    {
        var broken = new FakeProvider((_, _) => Task.FromException<decimal>(new InvalidOperationException("bug")));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => PriceLookup.GetBestPriceAsync("SKU-1", [Returns(10m), broken]));
    }

    [Fact]
    public async Task All_providers_unavailable_throws_NoPriceAvailable_mentioning_the_sku()
    {
        var ex = await Assert.ThrowsAsync<NoPriceAvailableException>(
            () => PriceLookup.GetBestPriceAsync("SKU-42", [Unavailable(), Unavailable()]));

        Assert.Contains("SKU-42", ex.Message);
    }

    [Fact]
    public async Task No_providers_throws_NoPriceAvailable()
    {
        await Assert.ThrowsAsync<NoPriceAvailableException>(
            () => PriceLookup.GetBestPriceAsync("SKU-1", []));
    }

    [Fact]
    public async Task Starts_all_providers_before_waiting_for_any()
    {
        var started = 0;
        var gates = new[]
        {
            new TaskCompletionSource<decimal>(),
            new TaskCompletionSource<decimal>(),
            new TaskCompletionSource<decimal>(),
        };
        var providers = gates
            .Select(gate => (IPriceProvider)new FakeProvider((_, _) =>
            {
                started++;
                return gate.Task;
            }))
            .ToList();

        var pending = PriceLookup.GetBestPriceAsync("SKU-1", providers);

        // Nobody has answered yet, but every provider must already have been called.
        Assert.Equal(3, started);

        gates[0].SetResult(30m);
        gates[1].SetResult(10m);
        gates[2].SetResult(20m);

        Assert.Equal(10m, await pending);
    }

    [Fact]
    public async Task Passes_the_cancellation_token_to_every_provider()
    {
        using var cts = new CancellationTokenSource();
        var seen = new List<CancellationToken>();

        IPriceProvider Capture(decimal price) => new FakeProvider((_, token) =>
        {
            seen.Add(token);
            return Task.FromResult(price);
        });

        await PriceLookup.GetBestPriceAsync("SKU-1", [Capture(1m), Capture(2m)], cts.Token);

        Assert.Equal(2, seen.Count);
        Assert.All(seen, token => Assert.Equal(cts.Token, token));
    }

    [Fact]
    public async Task Cancellation_is_not_swallowed()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var provider = new FakeProvider((_, token) => Task.FromCanceled<decimal>(token));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => PriceLookup.GetBestPriceAsync("SKU-1", [provider], cts.Token));
    }

    [Fact]
    public async Task Enumerates_the_providers_only_once()
    {
        var source = new CountingEnumerable<IPriceProvider>([Returns(5m), Returns(7m)]);

        await PriceLookup.GetBestPriceAsync("SKU-1", source);

        Assert.Equal(1, source.EnumerationCount);
    }
}
