// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Automation;
using Windows.Win32.UI.Accessibility;

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="UiaRaiseNotificationEvent(IRawElementProviderSimple*, NotificationKind, NotificationProcessing, BSTR, BSTR)"/>
    public static unsafe HRESULT UiaRaiseNotificationEvent(
        IRawElementProviderSimple.Interface provider,
        AutomationNotificationKind notificationKind,
        AutomationNotificationProcessing notificationProcessing,
        string? displayString)
    {
        if (OsVersion.IsWindows10_1709OrGreater())
        {
            using var providerScope = ComHelpers.GetComScope<IRawElementProviderSimple>(provider);
            using BSTR bstrText = displayString is null ? default : new(displayString);
            return UiaRaiseNotificationEvent(
                providerScope,
                (NotificationKind)notificationKind,
                (NotificationProcessing)notificationProcessing,
                bstrText,
                default);
        }

        return HRESULT.E_FAIL;
    }
}
