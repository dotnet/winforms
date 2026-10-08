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

`SystemVisualSettings` High Contrast transitions enter the same dispatcher with immutable old/new snapshots.
Every affected control compares its own effective mode, so locally configured descendants are included, while
effective-equality controls bypass visual-styles events, invalidation, and layout. The shared transition state
preserves existing reentrant-change suppression and layout coalescing.

## System visual settings renderer composition

Animated renderers subscribe only to their owning control's instance settings cascade. Accent transitions clear
their cached accent color; the control cascade repaints the tree. Disabling client-area animation completes an
active transition at progress 1 and unregisters it from the per-thread manager. Re-enabling does not restart a
completed transition; a later state change starts a new one.

`TextBoxBase` derives modern focus-border thickness and focus-band height from the system focus-border metrics,
text-scale factor, and device DPI. These metrics affect only modern chrome. The existing never-invert client
carve, small-control flat fallback, and classic-mode metrics remain unchanged.

## GroupBox content geometry

The Net11 `FlatStyle.Standard` GroupBox renders its caption above a borderless rectangular surface. Its
`DisplayRectangle` therefore starts lower and is shorter than the classic etched-frame rectangle. This is the
largest compatibility lever in the GroupBox change: the mode is opt-in, the paint and layout paths share the
same metrics, and the VisualStylesMode impact dispatcher remeasures AutoSize containers when the effective
renderer crosses the classic/modern boundary. The caption aligns to the control Padding, and the smaller bottom
content inset preserves separation when GroupBoxes are stacked. `FlatStyle.Flat` uses a rounded Windows-accent
outline; `FlatStyle.Popup` uses a Windows-accent header. `FlatStyle.System` remains native and does not use these
metrics.

## ComboBox adapter routing

Net11 routes `FlatStyle.Standard` ComboBox controls through the WinForms adapter path for the first time so the
field can share TextBoxBase's rounded chrome metrics and color scheme. `Flat` uses the same scheme with a
square-corner border; `Popup` keeps the rounded Standard geometry but uses the Windows accent color for its
border. All three styles add one logical pixel of framework inset on top of the now-designer-visible public
Padding. The adapter changes preferred height across the classic/modern boundary; `FlatStyle.System` remains
native and bypasses the adapter.

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
