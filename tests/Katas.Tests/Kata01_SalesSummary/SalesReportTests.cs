using Katas.Kata01_SalesSummary;

namespace Katas.Tests.Kata01_SalesSummary;

public class SalesReportTests
{
    private static readonly DateOnly Day = new(2026, 10, 1);

    private static Sale S(string category, string product, int qty, decimal price) =>
        new(category, product, qty, price, Day);

    [Fact]
    public void Null_input_throws()
    {
        Assert.Throws<ArgumentNullException>(() => SalesReport.SummarizeByCategory(null!));
    }

    [Fact]
    public void Empty_input_returns_empty_list()
    {
        var result = SalesReport.SummarizeByCategory([]);

        Assert.Empty(result);
    }

    [Fact]
    public void Aggregates_count_quantity_and_revenue_per_category()
    {
        var sales = new[]
        {
            S("Books", "Clean Code", 2, 50.00m),
            S("Books", "Refactoring", 1, 80.50m),
            S("Games", "Chess", 3, 20.00m),
        };

        var result = SalesReport.SummarizeByCategory(sales);

        var books = Assert.Single(result, r => r.Category == "Books");
        Assert.Equal(2, books.SalesCount);
        Assert.Equal(3, books.TotalQuantity);
        Assert.Equal(180.50m, books.TotalRevenue);

        var games = Assert.Single(result, r => r.Category == "Games");
        Assert.Equal(1, games.SalesCount);
        Assert.Equal(3, games.TotalQuantity);
        Assert.Equal(60.00m, games.TotalRevenue);
    }

    [Fact]
    public void Orders_by_revenue_desc_then_category_asc()
    {
        var sales = new[]
        {
            S("Toys", "Robot", 1, 100m),
            S("Books", "Novel", 1, 100m),
            S("Games", "Chess", 1, 300m),
        };

        var result = SalesReport.SummarizeByCategory(sales);

        Assert.Equal(["Games", "Books", "Toys"], result.Select(r => r.Category));
    }

    [Fact]
    public void Top_product_sums_all_lines_of_the_same_product()
    {
        var sales = new[]
        {
            S("Books", "A", 1, 60m),
            S("Books", "B", 1, 100m),
            S("Books", "A", 1, 60m),   // A = 120 > B = 100
        };

        var result = SalesReport.SummarizeByCategory(sales);

        Assert.Equal("A", Assert.Single(result).TopProduct);
    }

    [Fact]
    public void Top_product_tie_goes_to_alphabetically_first()
    {
        var sales = new[]
        {
            S("Books", "Zebra", 1, 50m),
            S("Books", "Alpha", 1, 50m),
        };

        var result = SalesReport.SummarizeByCategory(sales);

        Assert.Equal("Alpha", Assert.Single(result).TopProduct);
    }

    [Fact]
    public void Top_product_is_scoped_to_its_own_category()
    {
        var sales = new[]
        {
            S("Books", "Cheap", 1, 10m),
            S("Games", "Pricey", 1, 999m),
        };

        var result = SalesReport.SummarizeByCategory(sales);

        Assert.Equal("Cheap", result.Single(r => r.Category == "Books").TopProduct);
    }

    [Fact]
    public void Enumerates_the_source_only_once()
    {
        var enumerations = 0;
        IEnumerable<Sale> Source()
        {
            enumerations++;
            yield return S("Books", "A", 1, 10m);
            yield return S("Games", "B", 1, 20m);
        }

        SalesReport.SummarizeByCategory(Source());

        Assert.Equal(1, enumerations);
    }

    [Fact]
    public void Does_not_mutate_the_input()
    {
        var sales = new List<Sale> { S("Toys", "Robot", 1, 10m), S("Books", "Novel", 1, 99m) };
        var snapshot = sales.ToList();

        SalesReport.SummarizeByCategory(sales);

        Assert.Equal(snapshot, sales);
    }
}
