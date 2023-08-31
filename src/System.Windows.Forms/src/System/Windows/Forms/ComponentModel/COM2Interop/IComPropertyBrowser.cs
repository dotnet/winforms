// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using Microsoft.Win32;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

/// <summary>
///  Allows Visual Studio to communicate internally with the PropertyGrid control.
/// </summary>
public interface IComPropertyBrowser
{
    /// <summary>
    ///  Closes andy open drop-down controls on the <see cref="PropertyGrid"/> control.
    /// </summary>
    void DropDownDone();

    /// <summary>
    ///  Gets a value indicating whether the <see cref="PropertyGrid"/> control is currently setting one of the
    ///  properties of its selected object.
    /// </summary>
    bool InPropertySet { get; }

    /// <summary>
    ///  Occurs when the <see cref="PropertyGrid"/> control is browsing a COM object and the user renames the object.
    /// </summary>
    event ComponentRenameEventHandler? ComComponentNameChanged;

    /// <summary>
    ///  Commits all pending changes to the <see cref="PropertyGrid"/> control.
    /// </summary>
    /// <returns>
    ///  <see langword="true"/> if the <see cref="PropertyGrid"/> successfully commits changes; otherwise,
    ///  <see langword="false"/>.
    /// </returns>
    bool EnsurePendingChangesCommitted();

    /// <summary>
    ///  Activates the <see cref="PropertyGrid"/> control when the user chooses Properties for a control in Design view.
    /// </summary>
    void HandleF4();

    /// <summary>
    ///  Loads user states from the registry into the <see cref="PropertyGrid"/> control.
    /// </summary>
    /// <param name="key">The registry key that contains the user states.</param>
    void LoadState(RegistryKey? key);

    /// <summary>
    ///  Saves user states from the <see cref="PropertyGrid"/> control to the registry.
    /// </summary>
    /// <param name="key">The registry key that contains the user states.</param>
    void SaveState(RegistryKey? key);
}
