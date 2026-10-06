# WinForms Application Builder core contracts

**Status:** Contract prototype for issue [#14942](https://github.com/dotnet/winforms/issues/14942)  
**Architecture decisions:** [lifetime architecture](lifetime-architecture.md)  
**Parent proposal:** [#14082](https://github.com/dotnet/winforms/issues/14082)

## Contract boundary

The prototype places `WinFormsApplicationBuilder`, `WinFormsApplication`,
`WinFormsApplicationLifetime`, and the internal `WinFormsApplicationOptions`
in the `Microsoft.Extensions.WinForms` namespace in `System.Windows.Forms.dll`.
This follows the proposal's single-assembly option and avoids adding a package
or dependency on Generic Host, dependency injection, configuration, or logging
before those capabilities are designed.

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

This issue does not implement `Run`, `RunAsync`, `StartAsync`, or `StopAsync`,
does not create the WinForms synchronization context, and does not start or
coordinate a message loop. It also does not decide disposal ownership for
caller-supplied forms or contexts. Those behaviors require UI-thread and
message-loop coordination and belong to #14943; the implementation must follow
the lifetime architecture decision record.

The proposal's `IHost`/`IHostApplicationBuilder` compatibility is not added to
these types yet. Doing so would introduce hosting abstractions and startup or
shutdown semantics outside this contracts-only issue. Generic Host
integration, including ownership of lifetime tokens and stop coordination, is
left for the runtime design.

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
