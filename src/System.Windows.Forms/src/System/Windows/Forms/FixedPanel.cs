﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the Fixed Panel in the SplitContainer Control.
    /// </devdoc>
    public enum FixedPanel
    {
        /// <devdoc>
        /// No panel is fixed. Resize causes the Resize of both the panels.
        /// </devdoc>
        None = 0,

        /// <devdoc>
        /// Panel1 is Fixed. The resize will increase the size of second panel.
        /// </devdoc>
        Panel1 = 1,

        /// <devdoc>
        /// Panel2 is Fixed. The resize will increase the size of first panel.
        /// </devdoc>
        Panel2 = 2,
    }
}
