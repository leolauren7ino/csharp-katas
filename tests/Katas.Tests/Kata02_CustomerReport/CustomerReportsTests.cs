using Katas.Kata02_CustomerReport;

namespace Katas.Tests.Kata02_CustomerReport;

public class CustomerReportsTests
{
    private static readonly DateOnly Day = new(2026, 10, 2);

    private static Customer C(int id, string name) => new(id, name);

    private static OrderLine L(string product, int quantity, decimal unitPrice) =>
        new(product, quantity, unitPrice);

    private static Order O(int id, int customerId, params OrderLine[] lines) =>
        new(id, customerId, Day, lines);

    [Fact]
    public void Null_customers_throws()
    {
        Assert.Throws<ArgumentNullException>(() => CustomerReports.Build(null!, []));
    }

    [Fact]
    public void Null_orders_throws()
    {
        Assert.Throws<ArgumentNullException>(() => CustomerReports.Build([C(1, "Ana")], null!));
    }

    [Fact]
    public void No_customers_returns_empty_list()
    {
        var result = CustomerReports.Build([], [O(1, 1, L("Pen", 1, 5m))]);

        Assert.Empty(result);
    }

    [Fact]
    public void Customer_without_orders_still_appears_with_zeros()
    {
        var result = CustomerReports.Build([C(1, "Ana")], []);

        var ana = Assert.Single(result);
        Assert.Equal("Ana", ana.Customer);
        Assert.Equal(0, ana.OrderCount);
        Assert.Equal(0m, ana.TotalSpent);
        Assert.Empty(ana.Products);
    }

    [Fact]
    public void Counts_orders_and_sums_all_lines()
    {
        var customers = new[] { C(1, "Ana") };
        var orders = new[]
        {
            O(1, 1, L("Pen", 2, 5m), L("Book", 1, 40m)),   // 50
            O(2, 1, L("Pen", 1, 5m)),                      // 5
        };

        var result = CustomerReports.Build(customers, orders);

        var ana = Assert.Single(result);
        Assert.Equal(2, ana.OrderCount);
        Assert.Equal(55m, ana.TotalSpent);
    }

    [Fact]
    public void Products_are_distinct_and_sorted_alphabetically()
    {
        var customers = new[] { C(1, "Ana") };
        var orders = new[]
        {
            O(1, 1, L("Pen", 1, 5m), L("Cup", 1, 10m)),
            O(2, 1, L("Book", 1, 40m), L("Pen", 2, 5m)),
        };

        var result = CustomerReports.Build(customers, orders);

        var ana = Assert.Single(result);
        Assert.Equal(new[] { "Book", "Cup", "Pen" }, ana.Products);
    }

    [Fact]
    public void Orders_by_total_spent_desc_then_name_asc()
    {
        var customers = new[] { C(1, "Bruno"), C(2, "Ana"), C(3, "Carla") };
        var orders = new[]
        {
            O(1, 1, L("Pen", 1, 100m)),     // Bruno 100
            O(2, 2, L("Pen", 1, 100m)),      // Ana   100
            O(3, 3, L("Pen", 1, 300m)),      // Carla 300
        };

        var result = CustomerReports.Build(customers, orders);

        Assert.Equal(new[] { "Carla", "Ana", "Bruno" }, result.Select(r => r.Customer));
    }

    [Fact]
    public void Orders_of_unknown_customers_are_ignored()
    {
        var customers = new[] { C(1, "Ana") };
        var orders = new[]
        {
            O(1, 1, L("Pen", 1, 5m)),
            O(2, 99, L("Yacht", 1, 1000000m)),   // nobody has id 99
        };

        var result = CustomerReports.Build(customers, orders);

        var ana = Assert.Single(result);
        Assert.Equal(1, ana.OrderCount);
        Assert.Equal(5m, ana.TotalSpent);
    }

    [Fact]
    public void Enumerates_each_source_only_once()
    {
        var customerPasses = 0;
        var orderPasses = 0;

        IEnumerable<Customer> Customers()
        {
            customerPasses++;
            yield return C(1, "Ana");
            yield return C(2, "Bruno");
        }

        IEnumerable<Order> Orders()
        {
            orderPasses++;
            yield return O(1, 1, L("Pen", 1, 5m));
            yield return O(2, 2, L("Cup", 1, 10m));
        }

        CustomerReports.Build(Customers(), Orders());

        Assert.Equal(1, customerPasses);
        Assert.Equal(1, orderPasses);
    }
}
