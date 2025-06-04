// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Represents a method that will handle the <see cref="TreeView.OnBeforeCheck"/>,
///  <see cref="TreeView.OnBeforeCollapse"/>, <see cref="TreeView.BeforeExpand"/>,
///  or <see cref="TreeView.BeforeSelect"/> event.
/// </summary>
public delegate void TreeViewCancelEventHandler(object? sender, TreeViewCancelEventArgs e);
