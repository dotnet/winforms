// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#pragma warning disable RS0016 // Add public types and members to the declared API
[Obsolete("ToolBarButtonStyle has been deprecated.")]
[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
public delegate void ToolBarButtonClickEventHandler(object sender, ToolBarButtonClickEventArgs e);
