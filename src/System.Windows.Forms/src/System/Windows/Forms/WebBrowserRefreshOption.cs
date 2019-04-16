// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the RefreshOptions in the <see cref='System.Windows.Forms.WebBrowser.Refresh'/> method.
    /// </devdoc>
    public enum WebBrowserRefreshOption
    {
        Normal = 0,
        IfExpired = 1,
        Continue = 2,
        Completely = 3
    }
}
