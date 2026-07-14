// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

public partial class Form
{
#if NET11_0_OR_GREATER
    private static readonly object s_systemTextSizeChangedEvent = new();

    private double _lastSystemTextSize = ScaleHelper.GetSystemTextScaleFactor();

    /// <summary>
    ///  Occurs on this top-level <see cref="Form"/> when the Windows Accessibility text-scale setting changes.
    /// </summary>
    /// <remarks>
    ///  On operating systems earlier than Windows 10 version 1507, this event is not raised.
    /// </remarks>
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.FormOnSystemTextSizeChangedDescr))]
    public event EventHandler? SystemTextSizeChanged
    {
        add => Events.AddHandler(s_systemTextSizeChangedEvent, value);
        remove => Events.RemoveHandler(s_systemTextSizeChangedEvent, value);
    }

    /// <summary>
    ///  Raises the <see cref="SystemTextSizeChanged"/> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
    /// <remarks>
    ///  <para>
    ///   WinForms raises this event only after re-reading the current Windows Accessibility text-scale factor and
    ///   confirming that the underlying value actually changed. There is no dedicated Windows message for text-scale
    ///   changes.
    ///  </para>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnSystemTextSizeChanged(EventArgs e)
    {
        if (Events[s_systemTextSizeChangedEvent] is EventHandler handler)
        {
            handler(this, e);
        }
    }

    /// <summary>
    ///  Handles the WM_SETTINGCHANGE message.
    /// </summary>
    private void WmSettingChange(ref Message m)
    {
        base.WndProc(ref m);

        if (!GetTopLevel() || !ScaleHelper.TryGetSystemTextScaleFactor(out double systemTextSize))
        {
            return;
        }

        if (systemTextSize == _lastSystemTextSize)
        {
            return;
        }

        _lastSystemTextSize = systemTextSize;
        OnSystemTextSizeChanged(EventArgs.Empty);
    }
#endif
}
