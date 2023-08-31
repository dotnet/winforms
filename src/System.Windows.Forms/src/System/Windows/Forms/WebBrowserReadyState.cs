// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Ole;

namespace System.Windows.Forms;

/// <summary>
///  Specifies the ReadyState of the WebBrowser control.
///  Returned by the <see cref="WebBrowser.ReadyState"/> property.
/// </summary>
public enum WebBrowserReadyState
{
    Uninitialized = READYSTATE.READYSTATE_UNINITIALIZED,
    Loading = READYSTATE.READYSTATE_LOADING,
    Loaded = READYSTATE.READYSTATE_LOADED,
    Interactive = READYSTATE.READYSTATE_INTERACTIVE,
    Complete = READYSTATE.READYSTATE_COMPLETE
}
