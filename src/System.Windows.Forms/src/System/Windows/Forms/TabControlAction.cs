// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <devdoc>
    ///     This enum is used to specify the action that caused a TreeViewEventArgs.
    /// </devdoc>
    public enum TabControlAction {

        Selecting,
        Selected,
        Deselecting,
        Deselected
    }
}
