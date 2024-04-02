// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#nullable disable
[Obsolete(
    Obsoletions.DataGridCaptionMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridCaptionDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
internal class DataGridCaption
{
    protected virtual void AddEventHandler(object key, Delegate handler)
        => throw new PlatformNotSupportedException();

    protected static void OnBackwardClicked(EventArgs e)
        => throw new PlatformNotSupportedException();

    protected static void OnCaptionClicked(EventArgs e)
        => throw new PlatformNotSupportedException();

    protected static void OnDownClicked(EventArgs e)
        => throw new PlatformNotSupportedException();

    protected virtual Delegate GetEventHandler(object key)
        => throw new PlatformNotSupportedException();

    protected virtual void RaiseEvent(object key, EventArgs e)
    {
        Delegate handler = GetEventHandler(key);
        if (handler is not null)
            ((EventHandler)handler)(this, e);
    }

    protected virtual void RemoveEventHandler(object key, Delegate handler)
        => throw new PlatformNotSupportedException();

    protected virtual void RemoveEventHandlers()
        => throw new PlatformNotSupportedException();
}
