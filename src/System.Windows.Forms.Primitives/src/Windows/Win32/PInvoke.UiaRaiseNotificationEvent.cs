// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Automation;
using Windows.Win32.UI.Accessibility;

namespace Windows.Win32;

internal static partial class PInvoke
{
    public static unsafe HRESULT UiaRaiseNotificationEvent(
        IRawElementProviderSimple.Interface provider,
        AutomationNotificationKind notificationKind,
        AutomationNotificationProcessing notificationProcessing,
        string? displayString)
    {
        using var providerScope = ComHelpers.GetComScope<IRawElementProviderSimple>(provider);
        using BSTR bstrText = displayString is null ? default : new(displayString);
        // We do an OS check before this method is called.
#pragma warning disable CA1416 // This call site is reachable on 'windows' 6.1 and later, but is only supported on 'windows' 10.0.16299 and later.
        return UiaRaiseNotificationEvent(
            providerScope,
            (NotificationKind)notificationKind,
            (NotificationProcessing)notificationProcessing,
            bstrText,
            default);
#pragma warning restore
    }
}
