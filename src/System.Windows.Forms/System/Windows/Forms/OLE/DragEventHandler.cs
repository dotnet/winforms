﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Represents a method that will handle the <see cref="Control.DragDrop"/>,
///  <see cref="Control.DragEnter"/>, or <see cref="Control.DragOver"/>
///  event of a <see cref="Control"/>.
/// </summary>
public delegate void DragEventHandler(object? sender, DragEventArgs e);
