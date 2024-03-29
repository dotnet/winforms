﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Design;

/// <summary>
///  method that will handle the ToolboxItemCreatorCallback event.
/// </summary>
public delegate ToolboxItem ToolboxItemCreatorCallback(object serializedObject, string format);
