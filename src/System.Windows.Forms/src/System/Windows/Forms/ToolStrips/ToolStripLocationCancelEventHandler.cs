// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Represents a method that will handle the event raised when canceling an
///  <see cref="ToolStrip.OnLocationChanging"/> event for <see cref="ToolStrip"/>.
/// </summary>
internal delegate void ToolStripLocationCancelEventHandler(object? sender, ToolStripLocationCancelEventArgs e);
