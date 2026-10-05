namespace Katas.Kata01_SalesSummary;

public static class SalesReport
{
    public static IReadOnlyList<CategorySummary> SummarizeByCategory(IEnumerable<Sale> sales)
    {
        ArgumentNullException.ThrowIfNull(sales);

        return sales
            .GroupBy(sale => sale.Category)
            .Select(category => new CategorySummary(
                category.Key,
                category.Count(),
                category.Sum(sale => sale.Quantity),
                category.Sum(sale => sale.Total),
                TopProductOf(category)))
            .OrderByDescending(summary => summary.TotalRevenue)
            .ThenBy(summary => summary.Category)
            .ToList();
    }

    private static string TopProductOf(IEnumerable<Sale> categorySales) =>
        categorySales
            .GroupBy(sale => sale.Product)
            .Select(product => new { Name = product.Key, Revenue = product.Sum(sale => sale.Total) })
            .OrderByDescending(entry => entry.Revenue)
            .ThenBy(entry => entry.Name)
            .First()
            .Name;
}
