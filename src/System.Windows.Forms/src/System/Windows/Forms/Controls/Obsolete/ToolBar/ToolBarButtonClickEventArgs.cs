// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete("ToolBarButtonStyle has been deprecated.")]
#pragma warning disable RS0016 // Add public types and members to the declared API
public class ToolBarButtonClickEventArgs : EventArgs
{
    /// <summary>
    ///  Initializes a new instance of the <see cref='ToolBarButtonClickEventArgs'/>
    ///  class.
    /// </summary>
    public ToolBarButtonClickEventArgs(ToolBarButton button)
    {
        throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public ToolBarButton Button { get; set; }
}
