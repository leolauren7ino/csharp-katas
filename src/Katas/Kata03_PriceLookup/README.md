# Kata 03 - Best price (async/await + exceptions)

## Goal
Ask several slow remote providers for the price of a product **at the same time** and return the lowest
price, tolerating providers that are down but never hiding real bugs.

## Requirements
`PriceLookup.GetBestPriceAsync(sku, providers, cancellationToken)` returns the lowest price among the providers that answered:

- `sku` null -> `ArgumentNullException`. `sku` empty or whitespace -> `ArgumentException`.
- `providers` null -> `ArgumentNullException`.
- All providers are called **concurrently**: every provider must be started before the method waits for any of them.
- A provider that throws `ProviderUnavailableException` is **skipped**; the others still count.
- Any **other** exception from a provider (a bug, for example) is **not swallowed**: it reaches the caller as the original type.
- If no provider produced a price (all unavailable, or no providers at all) -> `NoPriceAvailableException` whose message contains the SKU.
- The `CancellationToken` is passed to every provider call, and a cancellation is **not** swallowed.
- `providers` is enumerated **at most once**.

## Acceptance criteria
`dotnet test` is green with no change to the tests (all earlier katas stay green too).

## Interview angle
- What does `await` do? Does it block the thread? What is the difference between `async` and "multithreaded"?
- Why must you avoid `.Result` / `.Wait()` on a Task (blocking, deadlocks), and what is `async void` bad for?
- Awaiting in a `foreach` vs starting all tasks first and then `await Task.WhenAll(...)`: what changes in total time?
- When several tasks in `Task.WhenAll` fail, what do you get from `await`? What does `task.Exception` hold?
- `catch (Exception)` vs catching a specific type; `throw;` vs `throw ex;`; when to create a custom exception.
- What is a `CancellationToken`, and why is cancellation "cooperative"?

## Ladder
Try alone first. Write the recipe in words before the code: which keyword makes the method able to `await`,
how you start every provider without waiting, how you wait for all of them, and where the `try/catch` has to live
so that one failed provider does not kill the others.
