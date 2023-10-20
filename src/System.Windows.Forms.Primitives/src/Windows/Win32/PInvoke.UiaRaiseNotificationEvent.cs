// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using System.Windows.Forms.Automation;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Windows.Win32;

internal static partial class PInvoke
{
    [DllImport(Interop.Libraries.UiaCore, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern HRESULT UiaRaiseNotificationEvent(
    IRawElementProviderSimple.Interface provider,
    AutomationNotificationKind notificationKind,
    AutomationNotificationProcessing notificationProcessing,
    [MarshalAs(UnmanagedType.BStr)] string? displayString,
    [MarshalAs(UnmanagedType.BStr)] string activityId);

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

        return UiaRaiseNotificationEvent(provider, notificationKind, notificationProcessing, displayString, string.Empty);
    }
}
