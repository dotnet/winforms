// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal static partial class WebBrowserHelper
{
    // Enumeration of the different Edit modes
    internal enum AXEditMode
    {
        None = 0,       // object not being edited
        Object = 1,     // object provided an edit verb and we invoked it
        Host = 2        // we invoked our own edit verb
    }
}
