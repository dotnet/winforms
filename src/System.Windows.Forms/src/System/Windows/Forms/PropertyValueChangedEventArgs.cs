// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  The event class that is created when a property in the grid is modified by the user.
/// </summary>
public class PropertyValueChangedEventArgs : EventArgs
{
    public PropertyValueChangedEventArgs(GridItem? changedItem, object? oldValue)
    {
        ChangedItem = changedItem;
        OldValue = oldValue;
    }

    public GridItem? ChangedItem { get; }

    public object? OldValue { get; }
}
