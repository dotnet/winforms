// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Represents a method that will handle the <see cref="TreeView.OnBeforeLabelEdit"/>
///  or <see cref="TreeView.OnAfterLabelEdit"/> event of
///  a <see cref="TreeView"/>.
/// </summary>
public delegate void NodeLabelEditEventHandler(object? sender, NodeLabelEditEventArgs e);
