namespace Katas.Kata01_SalesSummary;

public record Sale(string Category, string Product, int Quantity, decimal UnitPrice, DateOnly Date)
{
    public decimal Total => Quantity * UnitPrice;
}

public record CategorySummary(
    string Category,
    int SalesCount,
    int TotalQuantity,
    decimal TotalRevenue,
    string TopProduct);
