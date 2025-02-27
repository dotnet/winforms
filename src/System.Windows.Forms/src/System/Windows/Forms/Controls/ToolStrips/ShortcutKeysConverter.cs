// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
///  Provides a type converter to convert <see cref="Keys"/> objects to and from strings.
/// </summary>
internal class ShortcutKeysConverter : KeysConverter
{
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context) =>
        new StandardValuesCollection(Array.Empty<object>());

    /// <summary>
    ///  Shortcuts do not have standard values, they are combinations of alphanumeric keys with modifiers.
    /// </summary>
    public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => false;
}
