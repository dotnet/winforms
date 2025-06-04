// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Represents a method that will handle the <see cref="TreeView.OnNodeMouseClick"/> or
///  <see cref="TreeView.OnNodeMouseDoubleClick"/> event.
/// </summary>
public delegate void TreeNodeMouseClickEventHandler(object? sender, TreeNodeMouseClickEventArgs e);
