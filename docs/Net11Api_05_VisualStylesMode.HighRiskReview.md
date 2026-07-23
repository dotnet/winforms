# Net11Api_05 VisualStylesMode High-Risk Review

This document records animation infrastructure findings that require architecture and lifecycle decisions before they can be patched safely.

## Animation timer remains active and can consume a CPU core

`AnimationManager` retains its high-precision timer registration after the first animation. The timer uses a 14 ms `PeriodicTimer` cadence with a 16.667 ms absolute frame target and then actively spins to that target. Once the periodic schedule falls behind, ticks can complete immediately and the loop can spend nearly an entire frame spinning. The process can therefore retain 1 ms timer resolution and consume approximately one logical processor after animations have settled.

A safe redesign must establish:

- lazy timer ownership based on the number of running animations;
- an idle-shutdown invariant after the final animation settles;
- balanced `timeBeginPeriod` and `timeEndPeriod` calls;
- atomic registration, unregistration, start, and stop transitions;
- a scheduling algorithm that does not accumulate backlog or busy-wait indefinitely;
- disposal behavior during application and message-loop shutdown.

Before implementation, capture CPU and timer-resolution measurements, analyze current register/unregister races, and add tests proving that the timer parks after all animations stop.

## Process-wide animation dispatch targets the first UI thread

The process-wide `AnimationManager` singleton captures the `SynchronizationContext` of the first UI thread that initializes it. Renderers created by another supported WinForms UI thread are then ticked on the first thread. Renderer callbacks invalidate their controls directly, even though `Control.Invalidate` is not a cross-thread-safe API. The singleton's unsynchronized initialization also permits competing manager construction.

A safe redesign must choose between:

- one animation manager per UI thread or synchronization context; or
- per-renderer dispatch to each control's owning context.

The selected ownership model must cover message-loop teardown, renderer disposal, concurrent registration, and controls moving through handle recreation. Validation should include two independent `Application.Run` UI threads and prove that every animation callback executes on its control's owning thread.
