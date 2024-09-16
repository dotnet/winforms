// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

/// <summary>
///  This EventArgs class is used by the MenuCommandService to signify
///  that there has been a change in MenuCommands (added or removed)
///  on the related object.
/// </summary>
public class MenuCommandsChangedEventArgs : EventArgs
{
    /// <summary>
    ///  Constructor that requires the object in question, the type of change
    ///  and the remaining commands left for the object. "command" can be null
    ///  to signify multiple commands changed at once.
    /// </summary>
    public MenuCommandsChangedEventArgs(MenuCommandsChangedType changeType, MenuCommand? command)
    {
        ChangeType = changeType;
        Command = command;
    }

    /// <summary>
    ///  The type of changed that caused the related event
    ///  to be thrown.
    /// </summary>
    public MenuCommandsChangedType ChangeType { get; }

    /// <summary>
    ///  The command that was added/removed/changed. This can be null if more than one command changed at once.
    /// </summary>
    public MenuCommand? Command { get; }
}
