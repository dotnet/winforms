// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms.Automation;

internal partial class Interop
{
    internal static partial class UiaCore
    {
        [DllImport(Libraries.UiaCore, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern HRESULT UiaRaiseNotificationEvent(
            IRawElementProviderSimple provider,
            AutomationNotificationKind notificationKind,
            AutomationNotificationProcessing notificationProcessing,
            [MarshalAs(UnmanagedType.BStr)] string displayString,
            [MarshalAs(UnmanagedType.BStr)] string activityId);
    }
}
