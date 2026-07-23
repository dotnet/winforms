# Net11Api_03 ImproveControlRendering High-Risk Review

This document records a review finding that needs native lifecycle investigation before it can be patched safely.

## Deferred reveal is not activated during initial display

Reviewed merge base: `8b618e7f5`
Reviewed branch tip: `1ee2b58a6`

The deferred-reveal implementation attempts to cloak a form from `OnHandleCreated`, while `ShouldUseDeferredAppearanceCloak` requires both `IsHandleCreated` and `Visible`. During `Show`, `ShowDialog`, and `Application.Run(form)`, the handle is created while evaluating `HWND`, before `ShowWindow` makes the form visible. The visibility state changes later while processing `WM_SHOWWINDOW`, and there is no subsequent cloak attempt. The intended initial-display cloak therefore does not activate on the normal display path.

A safe correction requires selecting a one-shot point after handle creation but before the first compositor presentation. Moving the operation into `WM_SHOWWINDOW` could cloak later hide/show cycles, while removing the visibility check could cloak hidden forms whose handles are created for unrelated reasons.

Before implementation, document and verify the state timeline for:

- `Show`, `ShowDialog`, and `Application.Run`;
- hidden forms with pre-created handles;
- handle recreation;
- hide/show cycles;
- owned forms and splash screens;
- DWM composition or cloak-call failure.

The selected design should define a one-shot invariant and explicitly reject stale state after handle recreation. Native verification should inspect the DWM cloak state before first paint and after reveal. Automated tests should cover all display paths and preserve a clear rollback path if compositor timing differs across supported Windows versions.
