# WinForms Application Builder core contracts

**Status:** Contract prototype for issue [#14942](https://github.com/dotnet/winforms/issues/14942)  
**Architecture decisions:** [lifetime architecture](lifetime-architecture.md)  
**Parent proposal:** [#14082](https://github.com/dotnet/winforms/issues/14082)

## Contract boundary

The prototype places `WinFormsApplicationBuilder`, `WinFormsApplication`,
`WinFormsApplicationLifetime`, and the internal `WinFormsApplicationOptions`
in the `Microsoft.Extensions.WinForms` namespace in `System.Windows.Forms.dll`.
This follows the proposal's single-assembly option. Runtime coordination uses
`Microsoft.Extensions.Hosting.Abstractions` only to accept and coordinate an
existing `IHost`; the application builder does not create a host or add
dependency-injection, configuration, or logging APIs.

The builder supports selecting a form by type or instance, or selecting a
default or supplied `ApplicationContext`. The last startup-selection call wins.
Generic form selection stores a factory; it does not instantiate a control
during builder creation, `Build`, or option copying. `Build` snapshots the
builder's options so subsequent builder changes do not alter an already-built
application.

The options type is internal. The prototype does not expose services,
configuration, logging, or a public options pattern. Applications continue to
own their generated `ApplicationConfiguration.Initialize()` call; the builder
does not attempt to reference application-specific generated code.

## Lifetime contract

The application exposes one lifetime object with `ApplicationStarted`,
`ApplicationStopping`, and `ApplicationStopped` events. Internal notification
methods are one-shot and retain the required ordering; a stop after failed
startup does not synthesize an `ApplicationStarted` notification. Event-handler
exceptions propagate to the runtime coordinator, which must preserve cleanup
and failure semantics when it is implemented.

## Deferred to issue #14943

The runtime added by #14943 runs on the calling UI thread, installs the
WinForms synchronization context before creating a deferred startup form, and
uses the existing `Application.Run(ApplicationContext)` message loop. A
configured `IHost` is started before the WinForms started notification and is
stopped before an intercepted thread exit is allowed to unwind the loop. The
application owns and disposes a host passed to `UseHost`. Host-originated
stopping notifications are marshalled to the UI thread while the coordinator
is active.

Application-context exit deferral is an internal hook used to keep the message
pump responsive while asynchronous host stop callbacks finish. Ordinary
WinForms contexts without a configured host retain their existing synchronous
exit behavior. A host stop is terminal: after the host has begun stopping, an
application shutdown request is not canceled by a form-close veto.

The current bridge accepts an already-created `IHost`; it does not create the
host or expose `IServiceCollection`, configuration, logging, hosted-service
registration, or an options pattern. Runnable C# and Visual Basic examples
using an externally built host are in [samples](samples/README.md).

## Alternatives considered

- **A separate hosting assembly:** deferred. The proposal recommends the
  single-assembly option for the core types, and this prototype has no
  independent package dependency that would justify a second assembly.
- **Implementing a message loop or a host adapter here:** rejected. WinForms
  already owns message-loop behavior, and implementing runtime coordination
  here would overlap #14943.
- **Adding dependency-injection or application-configuration APIs now:**
  rejected. Those APIs and application-specific initialization semantics are
  outside the minimal contract prototype.
