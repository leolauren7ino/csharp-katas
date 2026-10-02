namespace Katas.Kata01_SalesSummary;

public static class SalesReport
{
    // TODO: implement using LINQ (GroupBy + aggregation). See README.md in this folder.
    public static IReadOnlyList<CategorySummary> SummarizeByCategory(IEnumerable<Sale> sales)
    {
        ArgumentNullException.ThrowIfNull(sales);
        List<CategorySummary> categories = new List<CategorySummary>();
        var groupsByCategory = sales.GroupBy(sale => sale.Category);

       

        foreach(var g in groupsByCategory)
        {
            var topProduct = g
                .GroupBy(sale => sale.Product)
                .Select(product => new { Name = product.Key, Revenue = product.Sum(sale => sale.Total) })
                .OrderByDescending(entry => entry.Revenue)
                .ThenBy(entry => entry.Name)
                .First()
                .Name;
            

            var summary = new CategorySummary(
                g.Key,
                g.Count(),
                g.Sum(sales => sales.Quantity),
                g.Sum(sales => sales.Total),
                topProduct
                );
            categories.Add(summary);
        }

        var orderedProducts = categories
            .OrderByDescending(c => c.TotalRevenue)
            .ThenBy(c => c.Category)
            .ToList();
   
        return orderedProducts;
    }
}
