﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Represents a method that will handle the <see cref="Control.MouseDown"/>,
///  <see cref="Control.MouseUp"/> or <see cref="Control.MouseMove"/> events of a form,
///  control or other component.
/// </summary>
public delegate void MouseEventHandler(object? sender, MouseEventArgs e);
