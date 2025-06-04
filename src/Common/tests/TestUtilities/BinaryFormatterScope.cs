// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization.Formatters.Binary;

namespace System;

/// <summary>
///  Scope for enabling / disabling the <see cref="BinaryFormatter"/>. Use in a <see langword="using"/> statement.
/// </summary>
public readonly ref struct BinaryFormatterScope
{
    private readonly AppContextSwitchScope _switchScope;

    public BinaryFormatterScope(bool enable)
    {
        // Prevent multiple BinaryFormatterScopes from running simultaneously. Using Monitor to allow recursion on
        // the same thread.
        Monitor.Enter(typeof(BinaryFormatterScope));
        _switchScope = new(AppContextSwitchNames.EnableUnsafeBinaryFormatterSerialization, GetDefaultValue, enable);
    }

    public void Dispose()
    {
        try
        {
            _switchScope.Dispose();
        }
        finally
        {
            Monitor.Exit(typeof(BinaryFormatterScope));
        }
    }

    public static bool GetDefaultValue() => false;
}
