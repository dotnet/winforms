# Net11Api_05 VisualStylesMode High-Risk Review

This document records animation infrastructure findings that require architecture and lifecycle decisions before they can be patched safely.

## VisualStylesMode transition impact dispatch

`Control.OnVisualStylesModeChanged` now owns impact processing for effective renderer changes. Overrides that
call `base` inherit preferred-size cache clearing, style/non-client frame refresh, invalidation, and deferred
layout for metric-affecting changes. The transition carries immutable old/new effective modes through the
existing `EventArgs` virtual shape; nested property changes cannot overwrite an outer transition, and stale
child cascades are suppressed.

Metric layout requests are collected while the complete affected subtree transitions, then coalesced to one
layout per container. This is required so a container measures only fully updated children and so
`AutoSize = false` text boxes still trigger parent remeasurement. `TextBoxBase` treats only crossings between
classic/disabled and Net11-or-later rendering as metric changes; Net11-to-Latest shares its preferred-height
and non-client padding metrics and repaints only.

## Animation timing and thread ownership

The timer now uses an absolute `Stopwatch` schedule with a high-resolution waitable timer on supported Windows
versions and a coarse 30 Hz fallback. It no longer changes process-wide timer resolution, and residual spinning is
bounded to the sub-millisecond remainder. Registration/start/stop transitions are generation-owned, callback
dispatch is allocation-free in steady state, and stale generations cannot dispatch after replacement.

`AnimationManager` is now per UI thread. Each manager captures that thread's synchronization context, derives
animation progress from the timer tick timeline, and is disposed when its message loop exits. A renderer fault
quarantines that renderer without stopping unrelated animations on the same thread.

The remaining power risk is idle registration lifetime: after a UI thread starts its first animation, its manager
keeps one timer registration until `Application.ThreadExit`, even when no renderer is currently running. The pacer
therefore continues waking and dispatching an empty frame callback. A future optimization can make manager
registration lazy while preserving the generation and thread-ownership invariants established by the rework.
