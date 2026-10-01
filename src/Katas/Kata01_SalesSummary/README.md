# Kata 01 - Sales summary (GroupBy + aggregation)

## Goal
Turn a flat list of sales into one summary row per category, using LINQ.

## Requirements
`SalesReport.SummarizeByCategory(IEnumerable<Sale>)` returns one `CategorySummary` per category:

- `SalesCount`: number of sale lines in the category.
- `TotalQuantity`: sum of `Quantity`.
- `TotalRevenue`: sum of `Sale.Total` (`Quantity * UnitPrice`).
- `TopProduct`: the product with the highest revenue **inside that category**
  (sum all lines of the same product). On a tie, the alphabetically first name wins.
- Result ordered by `TotalRevenue` descending, then `Category` ascending.
- Empty input returns an empty list. `null` input throws `ArgumentNullException`.
- Do not mutate the input. Do not enumerate `sales` more than once.

## Acceptance criteria
`dotnet test` is green with no change to the tests.

## Interview angle
- `GroupBy` is deferred; what actually runs and when? What does `ToList()` change?
- `IEnumerable<T>` vs `IReadOnlyList<T>` as a return type.
- Why `decimal` for money, not `double`?
- In EF Core, would this `GroupBy` translate to SQL? What if `TopProduct` used a custom method?
- Why does "enumerate only once" matter when the source is a DB query or a stream?

## Ladder
Try alone first. Stuck? Ask for a hint, then direction, and only then the solution.
