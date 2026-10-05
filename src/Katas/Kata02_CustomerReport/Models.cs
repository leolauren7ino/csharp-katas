namespace Katas.Kata02_CustomerReport;

public record Customer(int Id, string Name);

public record OrderLine(string Product, int Quantity, decimal UnitPrice)
{
    public decimal Total => Quantity * UnitPrice;
}

public record Order(int Id, int CustomerId, DateOnly Date, IReadOnlyList<OrderLine> Lines)
{
    public decimal Total => Lines.Sum(line => line.Total);
}

public record CustomerReport(
    string Customer,
    int OrderCount,
    decimal TotalSpent,
    IReadOnlyList<string> Products);
