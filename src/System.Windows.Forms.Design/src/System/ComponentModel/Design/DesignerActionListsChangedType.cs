// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

/// <summary>
///  An enum that defines what type of action happened to the related object's
///  <see cref="DesignerActionListCollection">designer action lists collection.</see>
/// </summary>
public enum DesignerActionListsChangedType
{
    /// <summary>
    ///  Signifies that one or more DesignerActionList was added.
    /// </summary>
    ActionListsAdded,

    /// <summary>
    ///  Signifies that one or more DesignerActionList was removed.
    /// </summary>
    ActionListsRemoved
}
