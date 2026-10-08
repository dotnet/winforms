# Work Order — Port `TextBoxBase` NC-Painting + `VisualStylesMode` Chrome onto `VisualStylesNet11`

**Target branch:** `KlausLoeffelmann/winforms` → `VisualStylesNet11` (current-`main`-based; modern path layout, no `/src/src/` doubling; `TextBoxBase.cs` ≈ 2130 lines).

**Source of the original implementation (pinned):** `KlausLoeffelmann/winforms` @ `cf32e9c4efeba9d77e1a14025a8590b104e3c705` (old `/src/System.Windows.Forms/src/...` layout; `TextBoxBase.cs` ≈ 2562 lines). Relevant files:
- `.../Controls/TextBox/TextBoxBase.cs` — NC paint, NC calc, focus invalidation, `GetVisualStylesPadding`, helpers.
- `.../Controls/TextBox/TextBoxBase.NonClientBitmapCache.cs` — the offscreen cache class.
- `.../Controls/TextBox/TextBox.cs` — derived-level overrides (`CreateParams`, `WndProc`, `OnBackColorChanged`, `PRF_NONCLIENT` path).

**Standing approval:** API review board sign-off from the .NET 9 cycle is still valid; DRI owns the call. This is a port + cleanup, **not** a redesign. Do not invent new public API beyond what the original exposed plus the `Padding` unshadowing called out below.

**House style (apply throughout):** namespaces globally imported (no `using`/`imports` noise); C# 13/14; NRTs on; `var` only for long type names or when the type is obvious from the RHS, explicit type names for primitives; blank line between a new block and a following `return`; pattern matching / `is` / `and` / `or` / switch expressions preferred; collection expressions (`List<T> x = [];`); expression-bodied members for single-line methods/read-only props formatted with the `=>` on the next line, 1-space-indented. Generate XML doc comments; use `<remarks>`/`<para>`.

---

## PROMPT 1 — Carry-over / Port

> **Role:** You are porting a working-but-imperfect feature across a 3-year gap in the surrounding file. Faithfully reproduce the *mechanism*; do not improve, simplify, or "modernize" the algorithm except where this document explicitly says to. Where the surrounding `VisualStylesNet11` code has moved on, rebase onto the new shape rather than pasting.

**Task.** Bring the non-client (NC) painting feature for `TextBoxBase` (and the `TextBox` derived touchpoints) from the pinned SHA `cf32e9c4…` onto `VisualStylesNet11`. The target branch currently has **none** of this work (no `WmNcPaint`/`WmNcCalcSize`/`OnNcPaint`/`GetVisualStylesPadding`/`NonClientBitmapCache`), but it **does** have the `VisualStylesMode` property infrastructure referenced in doc comments — wire into that, don't redeclare it.

**Steps, in order:**

1. **Fetch and diff the three source files** at SHA `cf32e9c4…` against their `VisualStylesNet11` counterparts. Produce a short inventory of every member you intend to add or modify, grouped by file, before writing any code.

2. **Port `TextBoxBase.cs` members:**
   - `WmNcPaint` / `OnNcPaint` (offscreen bitmap fill → AA rounded/single chrome → blit; `GetWindowDC`/`ReleaseDC` in `finally`).
   - `WmNcCalcSize` (carves the padding band from `NCCALCSIZE_PARAMS->rgrc[0]`; gated on the `_triggerNewClientSizeRequest` latch).
   - `InitializeClientArea` (the one-shot `SetWindowPos(SWP_FRAMECHANGED|NOMOVE|NOSIZE|NOZORDER|NOACTIVATE)` that provokes the single NC-calc).
   - `GetVisualStylesPadding` / `GetScrollBarPadding` and the `VisualStyles{Fixed3D|FixedSingle|NoBorder}BorderPadding`, `BorderThickness` consts.
   - The `WM_NCCALCSIZE` / `WM_NCPAINT` cases in `WndProc`.
   - `OnGotFocus` / `OnLostFocus` / `OnSizeChanged` NC-frame invalidation (`RedrawWindow` with `RDW_FRAME|RDW_INVALIDATE`).
   - `PreferredHeight` split (`PreferredHeightClassic` vs `PreferredHeightCore` selected by `VisualStylesMode`).
   - Reconcile `GetPreferredSizeCore` with the modern branch already present on target.

3. **Port `TextBoxBase.NonClientBitmapCache.cs`** — BUT this is a **decision point**, see Step 6.

4. **Reconcile `TextBox.cs` (derived):** the target already has `CreateParams`, `WndProc`, `OnBackColorChanged` (special-casing `Fixed3D`), `OnGotFocus`, and a `WM_PRINTCLIENT`/`PRF_NONCLIENT` + `Application.RenderWithVisualStyles` path. Merge the NC behavior so the derived overrides cooperate with the new base NC painting (no double border draw, no fighting the `PRF_NONCLIENT` path). Call out every conflict you resolve.

5. **Unshadow `Padding`.** On target it is still the neutered shadow (`[Browsable(false)]`, `EditorBrowsableState.Never`, `DesignerSerializationVisibility.Hidden`, `get/set => base.Padding`). Make it a real, browsable, serializable property that feeds the NC band via `GetVisualStylesPadding`. Preserve classic-mode behavior when `VisualStylesMode` is `Disabled`/`Classic`. Note the designer-serialization and back-compat implications in the PR description.

6. **Retarget the rounded-rectangle helpers to the framework.** The original called fork-local `FillRoundedRectangle`/`DrawRoundedRectangle`. These now ship as **`System.Drawing.Graphics` instance methods** (`(Pen, Rectangle, Size)` + `RectangleF`/`SizeF` overloads; landed in the .NET 9 wave, current on target). **Delete the fork-local helpers and call the shipped methods.** ⚠️ Verify a **`FillRoundedRectangle(Brush, …)`** overload actually exists on target — the public ref only enumerates the `Draw`(`Pen`) overloads. If the `Fill`/`Brush` overload is absent, STOP and flag it; do not re-add a private helper without surfacing the gap.

7. **Cache → `BufferedGraphics` (DECIDED — implement, do not re-evaluate).** The original used a hand-rolled per-instance `NonClientBitmapCache` (`CreateCompatibleBitmap` + `Image.FromHbitmap` + manual `DeleteObject`, `EnsureSize` realloc). Jeremy's late "use the existing cached bitmap" is confirmed to mean WinForms' own **`BufferedGraphics`/`BufferedGraphicsContext`** (the engine behind `OptimizedDoubleBuffer`; in `System.Drawing.Common` since .NET Framework 2.0, present on target). It is **not** `System.Drawing.Imaging.CachedBitmap` — that type is a read-only, device-dependent, blit-only frozen copy (no `Graphics`, translation-only, dies on bit-depth change) and cannot be a render target. **Delete `NonClientBitmapCache` entirely** (and its file `TextBoxBase.NonClientBitmapCache.cs`) and the `_cachedBitmap` field; replace with the shared buffer. Exact wiring:
   - Inside `OnNcPaint`, get the shared context and allocate the buffer against the **window-DC `Graphics` already created in `WmNcPaint`**, sized to the window `bounds`:
     `BufferedGraphicsContext context = BufferedGraphicsManager.Current;`
     `using BufferedGraphics buffer = context.Allocate(graphics, bounds);`
     `Graphics offscreenGraphics = buffer.Graphics;`
   - **Do NOT `using`/dispose `buffer.Graphics`** — the `buffer` owns it; `using` the **buffer** only. (The original `using`-disposed its `GetNewGraphics()` because it owned that `Graphics`; that ownership is now the buffer's.)
   - **All drawing into `offscreenGraphics` is unchanged** — the `FillRectangle(parentBackgroundBrush…)` corner-fill, the `BorderStyle` switch, the focus line: byte-for-byte identical, just a different `Graphics` target.
   - **`ExcludeClip(clientBounds)` stays on the *target* `graphics`** (the window-DC one), exactly where it is now, set *before* the buffer draws. It governs where `Render()` may blit, protecting the client area — unchanged semantics.
   - Replace the final blit `graphics.DrawImageUnscaled(offscreenBitmap, Point.Empty);` with **`buffer.Render();`** (no argument — it blits to the `graphics` captured at `Allocate` time).
   - While here, fix the pre-existing **double-dispose** in `WmNcPaint`: it has both `using Graphics graphics = …` and an explicit `graphics.Dispose()` in `finally`. Drop the explicit `Dispose()`; keep the `using` (or keep explicit and drop `using` — one, not both).
   - **Rationale to record in PR notes:** WinForms paints NC serially (one HWND at a time on the UI thread), so a single shared buffer suffices for any number of controls; the per-instance cache kept N resident GDI bitmaps to serve a one-deep queue. Steady-state allocation is unchanged (zero — shared buffer is reused when size fits); resident GDI memory drops from N× to 1×. The only cost is buffer-resize churn if controls of *wildly varying* sizes paint in a grow/shrink-alternating order — see smoke scenario 7 instrumentation.

8. **Carve clamp + chrome degradation (settled design — implement exactly as stated, do NOT add a minimum size).** The original `WmNcCalcSize` does raw subtraction on `rgrc[0]` with **no clamp**, so a large `Padding` (made worse because `GetVisualStylesPadding(true)` *adds* the live scrollbar allowance from `GetScrollBarPadding` on top of the border padding) can drive the carved client rect to **zero or inverted**. Two separate fixes, and they are deliberately *not* a `MinimumSize`:
   - **(8a) Never-invert clamp in `WmNcCalcSize`.** Floor each carved extent so the client rect can never invert: after the four adjustments, ensure `bottom >= top` and `right >= left` (e.g. clamp so the resulting client width/height is `Math.Max(0, …)`). A 0–1px client area is **acceptable and intended** — shipping multiline `TextBox` already shrinks to ~1px with scrollbars present, and we match that exactly. **Do NOT introduce a min-height/`MinimumSize`**, and do NOT make sizing behavior differ by `VisualStylesMode` (that would fracture the appearance-only contract of the opt-in). The clamp only prevents *underflow past zero*, which raw subtraction does and plain shrinking does not.
   - **(8b) Paint-time chrome degradation in `OnNcPaint`.** The rounded `Fixed3D` chrome (15px radius) renders as a broken lozenge below roughly `2 × cornerRadius + BorderThickness` in height. When the available band/height is below that viable threshold, **fall back to the original/simple chrome render** (flat or single-style border) instead of the rounded path. This is a *rendering* fallback only — it does not change size or layout. Rationale on record: if the box is so small there's no usable client area, the control isn't usable anyway, so graceful visual degradation (not a size floor) is the correct response.

9. **Build** `System.Windows.Forms` for the target TFM. Resolve all errors. Do not suppress new analyzer warnings without a one-line justification each.

**Deliverable:** a single commit (or tight series) on `VisualStylesNet11` plus a PR description that lists: members added/modified per file, every `TextBox.cs` conflict resolved, confirmation that `NonClientBitmapCache` was removed and replaced by `BufferedGraphics` (Step 7) with the resize-churn rationale, the clamp/degradation (Step 8) confirmed as render-only with no size minimum, the `Padding` unshadowing implications, and any flagged gaps (Step 6 `Fill` overload).

---

## PROMPT 2 — Critical Review (run AFTER Prompt 1, BEFORE smoke test)

> **Role:** Adversarial reviewer. The author wants the issues a sharp WinForms maintainer would catch, not reassurance. Cite file + line for every finding. Classify each as **MUST-FIX**, **SHOULD-FIX**, or **PRESERVE (do not 'improve')**.

Audit the ported code against this checklist. For each item, state the finding and the exact location.

1. **DPI scaling of the corner radius.** The original hardcodes `const int cornerRadius = 15` and `BorderThickness = 1` in device-independent units, then uses them inside a DPI-scaled NC band, while `GetVisualStylesPadding` *does* take a DPI path (`_deviceDpi`). Confirm whether the radius/thickness now scale Per-Monitor-V2. If not → **MUST-FIX** (corners look proportionally too tight at 150/200%).

2. **Full-frame NC repaint vs. partial `hrgnClip`.** `WmNcPaint` ignores the wParam clip region and repaints the whole frame. This is the **intended** fix for the offscreen-restore "dirty corners" artifact — verify it's preserved. But confirm `base.WndProc(ref m)` is still invoked with the original message and isn't double-painting the native border under the custom chrome. Classify the "ignore clip" behavior as **PRESERVE**.

3. **Corner-blend source = `Parent?.BackColor ?? BackColor`.** This is the known ceiling: corners blend against the parent's flat back color, so they mismatch over a gradient/image/Mica/sibling. For the common case (solid form/panel) it's correct. **PRESERVE** — do not let it be "improved" into a fake general-case solution. Note it as a documented limitation only.

4. **`WM_NCCALCSIZE` ↔ `Padding` round-trip + underflow.** Verify the band carved in `WmNcCalcSize` matches what `GetVisualStylesPadding(true)` reports and what `GetPreferredSizeCore`/`SizeFromClientSize` assume, for all three `BorderStyle` values × `Multiline` × scrollbars. Off-by-one here clips text or the caret. **Additionally** confirm the never-invert clamp (Prompt 1 Step 8a) is present and correct: with large `Padding` on a small multiline box *with both scrollbars*, the carved client rect must floor at 0, never invert. Remember the threshold is **border padding + live scrollbar padding** (`GetScrollBarPadding` reads `WS_HSCROLL`/`WS_VSCROLL`), so underflow hits sooner than the `Padding` value alone implies. Classify "0–1px client area is allowed, no min-size" as **PRESERVE** — do not let a reviewer or the agent add a `MinimumSize` floor.

4b. **Chrome degradation below viable height.** Confirm `OnNcPaint` (Prompt 1 Step 8b) falls back to simple/flat chrome when height < ≈`2 × cornerRadius + BorderThickness`, instead of drawing a corrupted rounded rect. This is **render-only**; assert it does **not** alter size, layout, or `ClientSize`. Verify the fallback path itself is DPI-correct (the threshold scales with the radius, which per item 1 must scale).

5. **`BorderStyle` fork + native edge suppression.** `Fixed3D` → rounded chrome, `FixedSingle` → single + underline, `None` → fill. Confirm the native `WS_EX_CLIENTEDGE`/`WS_BORDER` from `CreateParams` is suppressed when NC chrome is active, so the native edge isn't drawn under the custom one.

6. **`VisualStylesMode` gating is total.** Every NC entry point (`WmNcPaint`, `WmNcCalcSize`, `InitializeClientArea`, the focus/size invalidations) must early-out to byte-for-byte classic behavior when `VisualStylesMode` is `Disabled`/`Classic`. One missing guard = a back-compat regression. Note the original's `OnLostFocus` was **missing** the guard that `OnGotFocus` had — verify the port fixed this asymmetry.

7. **DC / GDI lifetime.** `GetWindowDC`→`ReleaseDC` in `finally`, and `Graphics.FromHdc`+`Dispose` ordering: correct **only** because `FromHdc` doesn't own the DC. **PRESERVE** — flag any "tidy into a single `using`" as a regression. Confirm the `WmNcPaint` **double-dispose** was fixed (it had both `using Graphics` and an explicit `graphics.Dispose()`). For the `BufferedGraphics` swap (Prompt 1 Step 7): verify the **buffer** is `using`-scoped but **`buffer.Graphics` is NOT separately disposed**; verify `Allocate` targets the window-DC `graphics` and `Render()` is called with no argument; confirm `NonClientBitmapCache` and the `_cachedBitmap` field are fully removed with no dangling refs. Audit for any HBITMAP/HDC leak in the new path (there should be none — the buffer owns it).

8. **DPI-change without handle recreate.** `_triggerNewClientSizeRequest` is a one-shot latch reset on handle recreate. Does a DPI change that does *not* recreate the handle re-carve the band with new padding? If the band can go stale on monitor move → **MUST-FIX** or at least an explicit tracked issue.

9. **Caret / IME / selection repaint.** The native `EDIT` invalidates aggressively. Confirm NC chrome doesn't go stale on caret blink/IME composition, and conversely that NC isn't thrashing-repainting on every caret tick. (Author never confirmed this was clean in the original.)

10. **`TextBox.cs` derived reconciliation.** Verify the derived `WndProc`, `OnBackColorChanged` `Fixed3D` special-case, and the `PRF_NONCLIENT`/`Application.RenderWithVisualStyles` path don't conflict with base NC painting (double draw, wrong-mode paint).

11. **Allocation churn.** Brushes/pens use cached scopes (`GetCachedSolidBrushScope`/`GetCachedPenScope`) — good; confirm preserved. Confirm the offscreen surface isn't reallocated per paint (only on size change).

**Deliverable:** a findings list (file:line, severity, recommendation). MUST-FIX items get fixed in this pass; SHOULD-FIX either fixed or filed; PRESERVE items annotated in code with a brief `// Intentional:` comment so the next reader doesn't "fix" them.

---

## PROMPT 3 — Smoke Test Harness

> *("Smoke test" = the shallow "does it power on without catching fire" pass — from hardware bring-up, where first power-on literally checked for smoke — run before any deep/perf testing. Goal here: broad coverage that it comes up and behaves on the obvious axes, with the two known-fragile cases as explicit named tests.)*

**Task.** Build a throwaway WinForms test app (separate project, not shipped) that exercises the ported feature across its permutation space and **specifically reproduces the two regressions this feature is prone to.**

**Permutation grid** — generate a form populated with `TextBox`es (and at least one `RichTextBox`, since `TextBoxBase` is the shared base) covering the cross-product of:
- `BorderStyle`: `None` × `FixedSingle` × `Fixed3D`
- `Multiline`: `false` × `true` (+ `WordWrap` on/off for multiline)
- `Padding`: `Empty` × asymmetric (e.g. `2,6,2,6`) × large (`12`)
- Scrollbars: none × vertical × both
- `VisualStylesMode`: `Disabled`/`Classic` (must look exactly like today) × `Net10`+ (new chrome)
- Focused vs unfocused (drive focus programmatically to capture the adorner/underline)

**Named, must-pass scenarios (the ones that silently regress):**

1. **Offscreen-restore ("dirty corners").** Move the window partly off the left/top screen edge, then back. Assert the NC corner regions are repainted clean (no stale pixels). This is the artifact that drove the full-frame-repaint design. Automate the drag via `SetWindowPos`/`MoveWindow`; capture before/after.

2. **Partial NC invalidation.** Trigger a partial `WM_NCPAINT` (e.g. overlap then reveal a sliver of the frame) and assert the whole chrome is coherent, not just the revealed strip.

3. **Per-Monitor-V2 DPI.** Run DPI-aware; move forms between a 100% and a 150%/200% monitor (or fake via `LogicalToDeviceUnits`/DPI-changed messages). Assert corner radius, border thickness, and padding band all scale; assert no clipped text/caret.

4. **Classic-mode parity.** With `VisualStylesMode = Disabled`, assert the control is pixel-identical to baseline `main` (native edge, no custom NC). A regression here is the back-compat line breaking.

5. **`BorderStyle` switch at runtime** (`Fixed3D`↔`FixedSingle`↔`None`) and **`Padding` change at runtime** — assert the band re-carves and chrome redraws without artifacts (exercises the `_triggerNewClientSizeRequest` reset path).

6. **Focus transitions** — tab through the grid; assert the focus underline (single) / 3D focus line (Fixed3D, shortened to clear the corner curve) appears/clears correctly.

7. **Shrink-to-collapse (clamp + degradation).** Take a multiline `Fixed3D` box with large `Padding` (e.g. `12`) and **both** scrollbars visible, then programmatically drag/resize its height down toward 1px. Assert: (a) **no crash / no inverted client rect** handed to the native `EDIT` — the carve floors at 0 (Step 8a); (b) below ≈`2 × cornerRadius + thickness` the chrome **falls back to flat/simple render** rather than drawing a corrupted lozenge (Step 8b); (c) the control **still collapses** to ~1px exactly like classic multiline — assert it is **not** held open by any min-size (regression if a floor appeared). Repeat at 150%/200% DPI so the degradation threshold is verified scaled, not fixed at 96-dpi pixels.

8. **BufferedGraphics allocation churn (perf sanity).** Build two forms: (a) **40 same-size** textboxes, (b) **40 wildly varying-size** textboxes (mix tiny and large), all `VisualStylesMode ≥ Net10`. Force a full repaint storm (invalidate all NC frames repeatedly; resize the form to cascade re-layout). Instrument the shared buffer: wrap/observe `BufferedGraphicsManager.Current` and count actual **bitmap (re)allocations** vs. reuses across the storm. Assert: case (a) allocates the buffer ≈once then reuses (steady-state alloc ≈ 0); case (b) may reallocate on grow but must **not** allocate-per-paint. Log alloc count per case. This empirically confirms the shared-buffer reasoning from Prompt 1 Step 7 and catches any accidental per-paint allocation regression.

**Harness mechanics:**
- A "capture all" button that screenshots each form to disk per `VisualStylesMode`, for eyeball diffing classic-vs-modern and pre-vs-post-DPI.
- A console/log line per assertion (pass/fail) so it can run semi-automated.
- Keep it dependency-light: raw WinForms + `SetWindowPos`/`RedrawWindow` P/Invoke for the offscreen and invalidation drivers.

**Deliverable:** the test project + a one-screen README naming the six scenarios and how to run them, plus a results log from one full run on the porter's machine (note DPI of monitors used).
