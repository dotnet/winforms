// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the ReadyState of the WebBrowser control.
    ///  Returned by the <see cref='WebBrowser.ReadyState'/> property.
    /// </summary>
    public enum WebBrowserReadyState
    {
        Uninitialized = Ole32.READYSTATE.UNINITIALIZED,
        Loading = Ole32.READYSTATE.LOADING,
        Loaded = Ole32.READYSTATE.LOADED,
        Interactive = Ole32.READYSTATE.INTERACTIVE,
        Complete = Ole32.READYSTATE.COMPLETE
    }
}
