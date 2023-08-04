﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Represents a method that will handle the <see cref="ListView.RetrieveVirtualItem"/> event
///  of a <see cref="ListView"/>.
/// </summary>
public delegate void RetrieveVirtualItemEventHandler(object? sender, RetrieveVirtualItemEventArgs e);
