# WinForms Application Builder lifetime architecture

**Status:** Architecture decision record for issue [#14941](https://github.com/dotnet/winforms/issues/14941)\
**Parent proposal:** [#14082](https://github.com/dotnet/winforms/issues/14082)\
**Related proposal:** [#11415](https://github.com/dotnet/winforms/issues/11415)

This document records the lifetime and hosting decisions that should constrain the later Application Builder implementation. It does not add public APIs or runtime behavior. Contracts, message-loop coordination, tests, samples, and benchmarks remain assigned to their respective child issues.

## Decision summary

1. The thread that calls `WinFormsApplication.Run` is the UI thread. The builder does not create a second UI thread.
2. The hosting layer delegates to the existing `Application.Run(Form)` or `Application.Run(ApplicationContext)` implementation. It does not implement another message pump or replace WinForms modal-loop behavior.
3. Host construction (`Build`) is distinct from host startup (`IHost.StartAsync`). The host can be built before the UI synchronization context exists. At run time, establish the WinForms synchronization context and create/obtain the startup UI object before completing host startup and publishing the WinForms `ApplicationStarted` event. This ensures that the WinForms started event follows UI initialization and precedes entry into `Application.Run`.
4. Host-initiated exit must be marshaled to the UI thread. Shutdown coordination must be asynchronous and idempotent; a UI-thread caller must not synchronously wait for host work that needs UI dispatch.
5. Form-close cancellation and `ApplicationContext.ExitThread` are materially different shutdown paths. The latter is synchronous and has no cancellation point. Graceful asynchronous host shutdown for an arbitrary supplied `ApplicationContext` requires an explicit pre-exit coordination mechanism; subscribing to `ThreadExit` or `ApplicationExit` alone is too late to guarantee it.
6. Generic Host lifetime tokens remain the host-facing lifecycle source. WinForms lifetime notifications must be one-shot and ordered, not an independent competing host lifetime.

## Existing WinForms behavior

### Thread and message-loop ownership

`Application.Run()` overloads delegate to `Application.ThreadContext.RunMessageLoop`. `ThreadContext` is thread-static, and the loop is started on the calling thread. The concrete thread contexts use the existing WinForms/component-manager message-pump infrastructure. Modal forms and `DoEvents` use nested loops with different loop reasons; an application builder must not replace these paths with a custom loop or call `DoEvents` as a substitute for the main loop.

For a main loop, `ThreadContext.RunMessageLoopInner` associates the `ApplicationContext`, makes its `MainForm` visible, installs the synchronization context if needed, and then invokes the existing message loop. The main-loop path rejects a nested main loop. The UI thread is therefore owned by the caller; the hosting layer owns only the decision to enter and leave the normal WinForms run path.

The standard `[STAThread]` entry point remains the expected way to select the UI thread. A future runtime implementation should validate that `Run` executes on the intended thread and must not silently move a supplied form or context to another thread. Builder creation and `Build` should not create controls or start the message loop.

### Synchronization context

`WindowsFormsSynchronizationContext.InstallIfNeeded` is called automatically by `Control` construction when `AutoInstall` is enabled, and again when the first message loop is entered. It does not replace a non-default synchronization context. Installation creates/uses the thread's WinForms marshaling control; `Post` uses `BeginInvoke` and `Send` uses `Invoke`. The context records the prior context for restoration.

The current main-loop implementation makes `ApplicationContext.MainForm` visible before its message-loop-level synchronization-context installation. Consequently, the hosting path must establish the context before startup-form activation rather than relying only on the installation that occurs inside `Application.Run`.

`UseStartupForm(Form)` accepts an already-constructed control. In the normal case, WinForms auto-installation occurs during base `Control` construction, before the derived form constructor body, but that is not guaranteed when `AutoInstall` is disabled or another synchronization context is already present. A future implementation must preserve and restore a caller's existing context and must not unconditionally overwrite it. The generic host adapter should use WinForms' existing installation/restoration mechanism through an appropriate in-assembly hook; it should not duplicate the marshaling-control implementation or call the public `Uninstall()` in a way that permanently disables auto-installation.

### Shutdown and failures

`Application.Exit` raises closing notifications for open forms and can be canceled by a form. If not canceled, it closes the forms and asks all thread contexts to exit. `Application.ExitThread` exits only the current thread and delegates to its `ApplicationContext` when one exists. `ApplicationContext.ExitThreadCore` raises the synchronous `ThreadExit` event; the default main-form close path reaches it through the context's main-form handle-destroyed notification. `ThreadContext` posts quit and performs thread/context cleanup as its loop unwinds. `ApplicationExit` is raised during thread-context teardown, not as a cancellable pre-exit hook.

WinForms also has its own UI-thread exception routing. Exceptions from form/message dispatch may be passed to `Application.ThreadException` (or the default WinForms handling policy); they are not automatically equivalent to exceptions thrown by host startup or by the outer `Application.Run` call. The hosting layer must not silently replace existing exception behavior as part of this architecture work.

## Generic Host reconciliation

The Generic Host owns service startup and shutdown, not the WinForms message pump. `IHost.StartAsync` runs host startup and hosted-service callbacks and publishes `IHostApplicationLifetime.ApplicationStarted` after successful startup callbacks. `IHost.StopAsync` signals `ApplicationStopping`, stops hosted services in reverse registration order, and publishes `ApplicationStopped` after hosted-service stop callbacks. It then stops `IHostLifetime`; therefore the `ApplicationStopped` token does not mean the outer `StopAsync` call has returned. The stop cancellation token is cooperative; services can observe cancellation and failures can be reported/aggregated. `IHostLifetime` represents host/process start-stop signaling and is not a reason to create a separate WinForms UI thread.

The terms in the expected WinForms startup sequence must distinguish building the host from starting it:

1. `CreateBuilder` and `Build` compose the host; they do not start hosted services.
2. `Run` binds execution to the caller's UI thread and establishes the WinForms synchronization context.
3. Startup form or application context creation/selection happens on that UI thread.
4. `IHost.StartAsync` completes host startup; its `ApplicationStarted` token and the WinForms `ApplicationStarted` notification must not precede successful UI initialization.
5. The hosting layer enters `Application.Run` using the selected form or application context.

This keeps host construction before UI setup while avoiding signaling that the WinForms application is started before its startup UI object exists. If future API constraints force host startup earlier, the relationship between the two `ApplicationStarted` signals must be reconsidered explicitly rather than publishing contradictory lifecycle events.

## Ownership and lifecycle decisions

### UI thread

- The application entry point selects and owns the thread by calling `Run`; it should be an STA thread, as in existing WinForms programs.
- A supplied `Form` or `ApplicationContext` remains associated with the thread on which it was created. `Run` must validate incompatible thread use rather than move the object.
- The hosting layer owns the run operation and shutdown coordination, not thread creation or Win32 message dispatch.
- `Run` is the synchronous boundary for the existing message loop. An async API is not required by this architecture record; adding one requires a separate design for preserving UI-thread affinity and avoiding a pre-loop async deadlock.

### Message loop

- Call the existing `Application.Run(Form)` or `Application.Run(ApplicationContext)` exactly once for the main loop.
- Let WinForms own nested modal/modeless message loops, filters, component-manager integration, thread-exception routing, and loop teardown.
- Host requests must be posted to the UI thread before invoking UI shutdown APIs. Do not call `Application.Exit` from an arbitrary worker thread.
- Startup failures before `Application.Run` propagate to the caller after cleanup. Exceptions escaping the run operation also propagate after host/context cleanup; UI-dispatch exceptions retain WinForms' established routing.

### Startup and lifetime events

- `ApplicationStarted` is raised once, only after host startup succeeds and startup UI initialization has completed, and immediately before entering the main message loop.
- `ApplicationStopping` is raised once when terminal shutdown coordination begins, before requesting loop exit.
- `ApplicationStopped` is raised once after the message loop has exited and hosted-service shutdown/cleanup has completed. The outer `IHost.StopAsync` may still be completing `IHostLifetime.StopAsync`; cleanup must still run when startup, runtime, shutdown, or cancellation paths fail.
- Repeated or concurrent stop requests converge on the same shutdown operation. They must not raise lifetime notifications more than once.

The existing Generic Host token semantics should be exposed consistently; the WinForms layer must not create a second independent set of host tokens with different ordering. The implementation must also account for Generic Host's synchronous lifetime-token callbacks: callbacks may request UI work, but must not block waiting for the UI loop to exit.

## Shutdown direction and constraints

### Host to WinForms

A host stop request starts the stopping transition. The application lifetime coordinator posts the loop-exit request to the owning UI thread, then allows the existing WinForms loop to unwind. Host stop callbacks and cleanup must be coordinated so that the host's stopped notification cannot precede loop exit. A call made on the UI thread must not synchronously wait for a continuation that needs that same UI thread.

`Application.Exit` can be vetoed by `FormClosing`. The eventual implementation must specify whether a host-initiated stop is terminal despite a form veto, or whether the veto aborts host stopping. Generic Host's stopping token is one-shot and cannot be retracted, so treating a veto as a return to a fully running application is not consistent with Generic Host semantics. This policy must be resolved in the runtime issue without silently changing ordinary `Application.Exit` behavior.

### WinForms to host

A main-form close can be intercepted while it is still cancellable. The host coordinator can defer final closure, run asynchronous host shutdown without blocking the message loop, and complete closure once shutdown is ready. This path must avoid re-entrant close and coalesce repeated close/exit requests.

An arbitrary `ApplicationContext.ExitThread()` is not cancellable: `ThreadExit` is a synchronous notification and the WinForms loop is asked to quit after the event returns. `ApplicationExit` is later still. Neither event alone provides a safe place to await asynchronous host shutdown while continuing to pump UI messages. A host-owned/interceptable context or a narrow WinForms pre-exit hook is therefore required if graceful, async host shutdown is guaranteed for every supplied `ApplicationContext`. The runtime issue must choose and test that mechanism; it must not claim that merely subscribing to `ThreadExit` provides the guarantee.

## Exception and cancellation model

- Builder validation and `Build` failures are synchronous and do not start a message loop.
- Startup-form or context construction failures occur on the UI thread before the loop starts. They prevent `ApplicationStarted`; cleanup still runs.
- Host `StartAsync` failures/cancellation prevent `ApplicationStarted` and prevent entering `Application.Run`. Any partially started host services are stopped according to Generic Host behavior, and cleanup failures must not hide the original startup failure.
- Exceptions routed by WinForms' UI-thread exception mechanism retain that mechanism. Exceptions escaping the outer run operation are surfaced to the caller after cleanup.
- Host-stop exceptions and cancellation are surfaced after best-effort cleanup. Preserve the primary runtime/startup error and retain shutdown errors as additional failure information.
- A startup or stop `CancellationToken` is cooperative and distinct from a WinForms `FormClosing` veto. A canceled shutdown request must not cause duplicate lifetime events or skip cleanup. Exact public cancellation overloads are deferred to API design.

## Alternatives considered

- **Create a dedicated GUI thread from an `IHostedService`: rejected.** This is the approach described by #11415's prototype. It conflicts with conventional WinForms entry-point/thread ownership, complicates supplied-form affinity, and requires cross-thread marshaling between the application and UI lifetimes.
- **Implement a parallel message pump: rejected.** WinForms already provides `Application.Run`, thread contexts, modal-loop handling, component-manager behavior, and teardown. Reimplementing these would duplicate sensitive infrastructure and risk regressions.
- **Use `ApplicationExit` as the graceful-shutdown hook: rejected.** It occurs during teardown and cannot defer loop exit for async host cleanup.
- **Assume `ApplicationContext.ThreadExit` is cancellable: rejected.** The event is synchronous and `ExitThreadCore` has no cancellation result.
- **Replace the current synchronization context unconditionally: rejected.** This would break callers that install a custom context and would diverge from `WindowsFormsSynchronizationContext.InstallIfNeeded` behavior.

## Scope and follow-up ownership

This record completes architecture research only. It does not implement builder/lifetime contracts or runtime coordination.

- **#14942:** prototype the minimal builder, application, options, and lifetime contracts; settle public API boundaries without DI, configuration, or logging.
- **#14943:** implement same-thread startup, synchronization-context timing, existing message-loop ownership, and host/WinForms shutdown coordination. Resolve the cancellable-form-close versus non-cancellable-`ApplicationContext.ExitThread` constraint before claiming complete graceful shutdown.
- **#14944:** add deterministic lifecycle, cancellation, exception, and shutdown tests.
- **#14945:** add samples only; do not add hosted-service infrastructure as part of the samples issue.
- **#14946:** add benchmarks only after the lifecycle implementation is stable.

## References

- WinForms: [`Application.cs`](../../src/System.Windows.Forms/System/Windows/Forms/Application.cs), [`Application.ThreadContext.cs`](../../src/System.Windows.Forms/System/Windows/Forms/Application.ThreadContext.cs), [`ApplicationContext.cs`](../../src/System.Windows.Forms/System/Windows/Forms/ApplicationContext.cs), [`WindowsFormsSynchronizationContext.cs`](../../src/System.Windows.Forms/System/Windows/Forms/WindowsFormsSynchronizationContext.cs), [`Control.cs`](../../src/System.Windows.Forms/System/Windows/Forms/Control.cs), [`Application.LightThreadContext.cs`](../../src/System.Windows.Forms/System/Windows/Forms/Application.LightThreadContext.cs), [`Application.ComponentThreadContext.cs`](../../src/System.Windows.Forms/System/Windows/Forms/Application.ComponentThreadContext.cs).
- Generic Host implementation: [`Host.StartAsync`/`Host.StopAsync`](https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Hosting/src/Internal/Host.cs), [`IHostApplicationLifetime`](https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Hosting.Abstractions/src/IHostApplicationLifetime.cs).
- Earlier proposal: [#11415](https://github.com/dotnet/winforms/issues/11415).
