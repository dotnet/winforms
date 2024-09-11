// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

/// <summary>
///  This event is raised by the <see cref="DesignerActionService"/> when a shortcut is either added or removed
///  to/from the related object.
/// </summary>
public delegate void DesignerActionListsChangedEventHandler(object sender, DesignerActionListsChangedEventArgs e);
