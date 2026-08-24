# Net11Api_04 KioskModeManager High-Risk Review

This document records review findings that require an API or lifecycle design decision before they can be patched safely.

## FullScreen requested state and actual state differ

`KioskModeManager.FullScreen` currently returns `true` when fullscreen has been requested but no target form is available. `FullScreenChanged`, however, is raised only when the resolved form actually enters or exits fullscreen. This creates two observable state models:

- The property reports requested state through `_pendingFullScreen || _isFullScreen`.
- The event reports actual window state through changes to `_isFullScreen`.

Consequently, setting `FullScreen = true` without a resolved form changes the property without raising the event. When a form later becomes available, the event is raised even though the property value remains `true`.

Before changing this behavior, decide whether `FullScreen` represents requested state or actual window state. The decision must cover:

- initialization and delayed parenting;
- missing, replaced, reparented, and disposed container controls;
- event ordering and two-way data binding;
- `ToggleFullScreen` behavior while a request is pending;
- disposal and failed fullscreen transitions;
- compatibility for applications already observing the property or event.

Tests should define the complete transition matrix for requested, pending, entered, exited, reparented, and disposed states.
