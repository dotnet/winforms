// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Represents a method that handles the <see cref="TabControl.Deselected"/> and
///  <see cref="TabControl.Deselecting"/> events of a <see cref="TabControl"/>.
/// </summary>
public delegate void TabControlCancelEventHandler(object? sender, TabControlCancelEventArgs e);
