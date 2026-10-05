# Kata 02 - Customer report (GroupJoin + SelectMany)

## Goal
Combine two separate lists (customers and orders) into one report row per customer.
This is the in-memory version of a SQL `LEFT JOIN` + `GROUP BY`.

## Requirements
`CustomerReports.Build(IEnumerable<Customer>, IEnumerable<Order>)` returns one `CustomerReport` per customer:

- `Customer`: the customer's name.
- `OrderCount`: how many orders the customer has.
- `TotalSpent`: sum of `Order.Total` over the customer's orders.
- `Products`: the **distinct** product names across all lines of all the customer's orders, sorted alphabetically.
- **Customers without orders still appear**, with `OrderCount = 0`, `TotalSpent = 0` and an empty `Products` list.
- Orders whose `CustomerId` matches no customer are ignored.
- Result ordered by `TotalSpent` descending, then `Customer` ascending.
- `null` for either argument throws `ArgumentNullException`.
- Each source is enumerated **at most once**.

## Acceptance criteria
`dotnet test` is green with no change to the tests.

## Interview angle
- `Join` vs `GroupJoin`: which one is an inner join and which one gives you a left join?
- What does `SelectMany` do that `Select` does not? Flatten: when do you need it?
- Why is joining with a nested loop O(n*m), and what makes `GroupJoin` / `ToLookup` roughly O(n+m)?
- In EF Core you would rarely write the join by hand. What replaces it? (navigation properties + `Include`)
- `Distinct` and ordering: is `Distinct` guaranteed to keep the original order?

## Ladder
Try alone first. Write the recipe in words before the code: which operator joins, which flattens, which removes duplicates.
