# InvokeAsync Unit Test Instructions for Copilot

## Method Signatures to Test

```csharp
// Async callback returning ValueTask
public async Task InvokeAsync(Func<CancellationToken, ValueTask> callback, CancellationToken cancellationToken = default)

// Async callback returning ValueTask<T>
public async Task<T> InvokeAsync<T>(Func<CancellationToken, ValueTask<T>> callback, CancellationToken cancellationToken = default)

// Sync callback returning T
public async Task<T> InvokeAsync<T>(Func<T> callback, CancellationToken cancellationToken = default)

// Sync callback returning void
public async Task InvokeAsync(Action callback, CancellationToken cancellationToken = default)
```

## Required Test Coverage

Please create comprehensive unit tests for each overload that verify:

### Core Functionality
- **UI Thread Delegation**: Verify the callback executes on the UI thread (different from calling thread)
- **Cancellation Support**: Test cancellation works even when callback doesn't support it (sync overloads)
- **Async Cancellation**: Test cancellation works when callback supports it (async overloads with CancellationToken)
- **Exception Propagation**: Verify exceptions from callbacks are properly propagated to caller

### Edge Cases
- **Handle Not Created**: Verify `InvalidOperationException` when control handle isn't created
- **Pre-cancelled Token**: Verify early return when token is already cancelled
- **Multiple Concurrent Calls**: Test thread safety with overlapping invocations
- **Reentry Scenarios**: Test calling InvokeAsync from within a callback

### Cancellation Scenarios
- **External Cancellation**: Cancel token while callback is queued/running
- **Callback Cancellation**: For async overloads, test cancellation within the callback itself
- **Registration Cleanup**: Verify cancellation registrations are properly disposed

### Return Value Testing
- **Generic Overloads**: Test proper return value handling for `Task<T>` variants
- **Void Overload**: Test completion signaling for `Action` overload

### Performance/Resource Testing
- **Memory Leaks**: Verify no leaked registrations or task completion sources
- **Async Context**: Verify ConfigureAwait behavior and sync context handling

## Test Structure Guidance

- Use a test control with proper handle creation for UI thread tests
- Use `Thread.CurrentThread.ManagedThreadId` to verify thread marshalling
- Use `CancellationTokenSource` with timeouts for cancellation tests
- Include both immediate and delayed cancellation scenarios
- Test with both short-running and long-running callbacks
- Use appropriate async test patterns with proper awaiting

Create tests that are robust, deterministic, and cover both happy path and error conditions.