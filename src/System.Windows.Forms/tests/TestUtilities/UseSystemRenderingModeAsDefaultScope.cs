// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Primitives;

namespace System;

public readonly ref struct UseSystemRenderingModeAsDefaultScope
{
    private readonly AppContextSwitchScope _switchScope;

    public UseSystemRenderingModeAsDefaultScope(bool enable)
    {
        // Prevent multiple UseSystemRenderingModeAsDefault instances from running simultaneously. Using Monitor to allow recursion on
        // the same thread.
        Monitor.Enter(typeof(UseSystemRenderingModeAsDefaultScope));
        _switchScope = new(WinFormsAppContextSwitchNames.UseSystemRenderingModeAsDefault, GetDefaultValue, enable);
    }

    public void Dispose()
    {
        try
        {
            _switchScope.Dispose();
        }
        finally
        {
            Monitor.Exit(typeof(UseSystemRenderingModeAsDefaultScope));
        }
    }

    public static bool GetDefaultValue() =>
        typeof(LocalAppContextSwitches).TestAccessor()
            .CreateDelegate<Func<string, bool>>("GetSwitchDefaultValue")(WinFormsAppContextSwitchNames.UseSystemRenderingModeAsDefault);
}
