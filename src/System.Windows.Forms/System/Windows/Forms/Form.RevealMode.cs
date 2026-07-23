// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Windows.Win32.Graphics.Dwm;

namespace System.Windows.Forms;

public partial class Form
{
#if NET11_0_OR_GREATER
    private static readonly int s_propFormRevealMode = PropertyStore.CreateKey();
    private static readonly object s_formRevealModeChangedEvent = new();

    // Guards RevealDeferredAppearance against re-entrancy: its forced RDW_UPDATENOW repaint can
    // dispatch WM_PAINT synchronously, and the form's WM_PAINT handler also calls the reveal.
    private bool _isRevealingDeferredAppearance;

    /// <summary>
    ///  Gets or sets how this form is presented while its initial appearance is prepared.
    /// </summary>
    /// <value>
    ///  A <see cref="FormRevealMode"/> value. When not explicitly set, the effective value is
    ///  <see cref="FormRevealMode.Deferred"/> or <see cref="FormRevealMode.Classic"/>, resolved from
    ///  <see cref="Application.IsFormRevealDeferred"/>.
    /// </value>
    /// <remarks>
    ///  <para>
    ///   As an ambient property, a form that does not have this value set explicitly resolves it from
    ///   <see cref="Application.DefaultFormRevealMode"/>. Unlike a control-level ambient property that
    ///   chains through a parent hierarchy, this property does not chain through a parent hierarchy:
    ///   deferred reveal is a top-level-window-only concept (see remarks on
    ///   <see cref="FormRevealMode.Deferred"/> and the DWM cloaking mechanism it relies on), so only
    ///   <see cref="Form"/> itself, and the process-wide <see cref="Application"/> default, participate.
    ///  </para>
    ///  <para>
    ///   A splash screen or other form that should always appear instantly, even while the rest of the
    ///   application defers by default, can set this property on itself to
    ///   <see cref="FormRevealMode.Classic"/> without affecting the process-wide default.
    ///  </para>
    /// </remarks>
    [SRCategory(nameof(SR.CatWindowStyle))]
    [AmbientValue(FormRevealMode.Inherit)]
    [SRDescription(nameof(SR.FormFormRevealModeDescr))]
    public virtual FormRevealMode FormRevealMode
    {
        get
        {
            if (!Properties.TryGetValue(s_propFormRevealMode, out FormRevealMode value)
                || value == FormRevealMode.Inherit)
            {
                value = Application.IsFormRevealDeferred ? FormRevealMode.Deferred : FormRevealMode.Classic;
            }

            return value;
        }
        set
        {
            SourceGenerated.EnumValidator.Validate(value, nameof(value));

            FormRevealMode previous = FormRevealMode;

            if (value == FormRevealMode.Inherit)
            {
                Properties.RemoveValue(s_propFormRevealMode);
            }
            else
            {
                Properties.AddValue(s_propFormRevealMode, value);
            }

            if (FormRevealMode != previous)
            {
                OnFormRevealModeChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///  Occurs when the effective value of the <see cref="FormRevealMode"/> property changes.
    /// </summary>
    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.FormOnFormRevealModeChangedDescr))]
    public event EventHandler? FormRevealModeChanged
    {
        add => Events.AddHandler(s_formRevealModeChangedEvent, value);
        remove => Events.RemoveHandler(s_formRevealModeChangedEvent, value);
    }

    /// <summary>
    ///  Raises the <see cref="FormRevealModeChanged"/> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
    protected virtual void OnFormRevealModeChanged(EventArgs e)
        => (Events[s_formRevealModeChangedEvent] as EventHandler)?.Invoke(this, e);

    private bool ShouldSerializeFormRevealMode() => Properties.ContainsKey(s_propFormRevealMode);

    private void ResetFormRevealMode() => Properties.RemoveValue(s_propFormRevealMode);

    internal bool DeferredAppearanceCloaked
    {
        get => Properties.GetValueOrDefault(s_propFormAppearanceCloaked, false);
        set => Properties.AddOrRemoveValue(s_propFormAppearanceCloaked, value, defaultValue: false);
    }

    private void CloakForDeferredAppearanceIfNeeded()
    {
        if (!ShouldUseDeferredAppearanceCloak())
        {
            return;
        }

        if (SetDwmCloak(cloaked: true))
        {
            DeferredAppearanceCloaked = true;
        }
    }

    private void UncloakDeferredAppearanceIfNeeded()
    {
        if (!DeferredAppearanceCloaked)
        {
            return;
        }

        if (SetDwmCloak(cloaked: false))
        {
            DeferredAppearanceCloaked = false;
        }
    }

    /// <summary>
    ///  Reveals a form that was cloaked for deferred appearance, once its entire control tree has
    ///  painted its first frame.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The window is cloaked from <see cref="OnHandleCreated"/> while still hidden and shown while
    ///   cloaked, so nothing reaches the screen yet. Simply uncloaking on the form's own first
    ///   <c>WM_PAINT</c> would reveal the window before its child controls — each a separate window —
    ///   have painted, leaving a residual flash of controls popping in. This forces a synchronous
    ///   paint of the whole tree (<see cref="REDRAW_WINDOW_FLAGS.RDW_ALLCHILDREN"/> plus
    ///   <see cref="REDRAW_WINDOW_FLAGS.RDW_UPDATENOW"/>) into the still-cloaked redirection surface,
    ///   then uncloaks, so the finished first frame appears at once. It is a no-op once the form has
    ///   already been revealed (or was never cloaked), so it is safe to call from more than one hook.
    ///  </para>
    ///  <para>
    ///   <see cref="REDRAW_WINDOW_FLAGS.RDW_UPDATENOW"/> dispatches <c>WM_PAINT</c> synchronously, and
    ///   one of this method's callers is the form's own <c>WM_PAINT</c> handler, so a re-entrancy guard
    ///   prevents the forced repaint from recursively re-entering the reveal before the cloak clears.
    ///  </para>
    /// </remarks>
    private void RevealDeferredAppearance()
    {
        if (!DeferredAppearanceCloaked || _isRevealingDeferredAppearance)
        {
            return;
        }

        _isRevealingDeferredAppearance = true;
        try
        {
            // Flush the first-frame paint for the form and its entire child tree into the still-cloaked
            // redirection surface before revealing, so the finished frame appears in one step.
            PInvoke.RedrawWindow(
                this,
                lprcUpdate: null,
                HRGN.Null,
                REDRAW_WINDOW_FLAGS.RDW_INVALIDATE
                    | REDRAW_WINDOW_FLAGS.RDW_ERASE
                    | REDRAW_WINDOW_FLAGS.RDW_ALLCHILDREN
                    | REDRAW_WINDOW_FLAGS.RDW_UPDATENOW);

            UncloakDeferredAppearanceIfNeeded();
        }
        finally
        {
            _isRevealingDeferredAppearance = false;
        }
    }

    private void ClearDeferredAppearanceCloakState() => DeferredAppearanceCloaked = false;

    private bool ShouldUseDeferredAppearanceCloak()
        // This is evaluated from OnHandleCreated, at which point a top-level form's window is
        // always still hidden: Form clears WS_VISIBLE from the create params and defers the show
        // (see CreateParams / s_formStateShowWindowOnCreate). The window's Visible state is only
        // set later, while WM_SHOWWINDOW is processed. Checking Visible here would therefore never
        // be true and the cloak would never engage, so the window is shown uncloaked and the
        // default-background flash the mode is meant to prevent still occurs. Cloaking the
        // already-hidden window now is exactly the intended behavior for both Show and ShowDialog.
        => FormRevealMode == FormRevealMode.Deferred
            && TopLevel
            && !IsMdiChild
            && IsHandleCreated;

    private unsafe bool SetDwmCloak(bool cloaked)
    {
        BOOL cloak = cloaked;
        HRESULT result = PInvoke.DwmSetWindowAttribute(
            HWND,
            DWMWINDOWATTRIBUTE.DWMWA_CLOAK,
            &cloak,
            (uint)sizeof(BOOL));

        return result.Succeeded;
    }
#endif
}
