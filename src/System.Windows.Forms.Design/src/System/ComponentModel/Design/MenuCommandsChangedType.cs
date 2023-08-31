// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

/// <summary>
///  An enum that defines what time of action happened to the
///  related object's MenuCommands collection.
/// </summary>
public enum MenuCommandsChangedType
{
    /// <summary>
    ///  Signifies that one or more DesignerShortcut was added.
    /// </summary>
    CommandAdded,

    /// <summary>
    ///  Signifies that one or more DesignerShortcut was removed.
    /// </summary>
    CommandRemoved,

    /// <summary>
    ///  Signifies that one or more commands have changed their status.
    /// </summary>
    CommandChanged,
}
