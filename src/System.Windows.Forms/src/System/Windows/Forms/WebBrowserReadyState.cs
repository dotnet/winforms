// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the ReadyState of the WebBrowser control.
    /// Returned by the <see cref='System.Windows.Forms.WebBrowser.ReadyState'/> property.
    /// </devdoc>
    public enum WebBrowserReadyState
    {
        Uninitialized = 0,
        Loading = 1,
        Loaded = 2,
        Interactive = 3,
        Complete = 4
    }
}
