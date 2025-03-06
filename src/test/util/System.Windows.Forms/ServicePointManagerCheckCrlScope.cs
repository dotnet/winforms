// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Primitives;

namespace System;

/// <summary>
///  Scope for enabling / disabling the ServicePointManagerCheckCrl Switch.
///  Use in a <see langword="using"/> statement.
/// </summary>
public readonly ref struct ServicePointManagerCheckCrlScope
{
    private readonly AppContextSwitchScope _switchScope;

    public ServicePointManagerCheckCrlScope(bool enable)
    {
        // Prevent multiple ServicePointManagerCheckCrlScope instances from running simultaneously.
        // Using Monitor to allow recursion on the same thread.
        Monitor.Enter(typeof(ServicePointManagerCheckCrlScope));
        _switchScope = new(WinFormsAppContextSwitchNames.ServicePointManagerCheckCrl, GetDefaultValue, enable);
    }

    public void Dispose()
    {
        try
        {
            _switchScope.Dispose();
        }
        finally
        {
            Monitor.Exit(typeof(ServicePointManagerCheckCrlScope));
        }
    }

    public static bool GetDefaultValue() =>
        typeof(LocalAppContextSwitches).TestAccessor()
            .CreateDelegate<Func<string, bool>>("GetSwitchDefaultValue")(WinFormsAppContextSwitchNames.ServicePointManagerCheckCrl);
}
