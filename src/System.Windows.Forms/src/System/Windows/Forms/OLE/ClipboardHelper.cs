// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;
using Switches = System.Windows.Forms.Primitives.LocalAppContextSwitches;

namespace System.Windows.Forms;

internal static class ClipboardHelper
{
    /// <devdoc>
    ///  This resolver is used in deserialization of Clipboard and when wrapped into a serialization binder, suppresses all dangerous type.
    /// </devdoc>
    /// <exception cref="NotSupportedException"></exception>
    internal static Func<TypeName, Type> SafeResolver { get; } = (typeName) =>
        throw new NotSupportedException("Using BinaryFormatter is not supported in WinForms Clipboard data deserialization.");

    /// <devdoc>
    ///  This resolver is used in deserialization of Clipboard content and and when wrapped into a serialization binder, passes all types through.
    ///  It is used in compatibility scenarios when user explicitly opts into its use.
    /// </devdoc>
    /// <exception cref="NotSupportedException"></exception>
    internal static Func<TypeName, Type>? UnsafeResolver { get; } = (typeName) =>
    {
        if (!Switches.ClipboardEnableUnsafeBinaryFormatterDeserialization)
        {
            throw new NotSupportedException("Using BinaryFormatter is not supported in WinForms Clipboard data deserialization.");
        }

        return null!;
    };
}
