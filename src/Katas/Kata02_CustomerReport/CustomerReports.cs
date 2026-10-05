namespace Katas.Kata02_CustomerReport;

public static class CustomerReports
{
    // TODO: implement with LINQ (GroupJoin, SelectMany, Distinct, OrderBy). See README.md in this folder.
    public static IReadOnlyList<CustomerReport> Build(IEnumerable<Customer> customers, IEnumerable<Order> orders)
    {
        ArgumentNullException.ThrowIfNull(customers);
        ArgumentNullException.ThrowIfNull(orders);

        return customers
          .GroupJoin(
              orders,
              customer => customer.Id,
              order => order.CustomerId,
              (customer, customerOrders) => new { Customer = customer, Orders = customerOrders })
          .Select(x => new CustomerReport(
              x.Customer.Name,
              x.Orders.Count(),
              x.Orders.Sum(order => order.Total),
              x.Orders
                  .SelectMany(order => order.Lines)
                  .Select(line => line.Product)
                  .Distinct()
                  .OrderBy(product => product)
                  .ToList()))
          .OrderByDescending(report => report.TotalSpent)
          .ThenBy(report => report.Customer)
          .ToList();
    }
}
