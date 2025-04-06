// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Primitives;

namespace System;

/// <summary>
///  Scope for enabling / disabling the NoClientNotifications Switch.
///  Use in a <see langword="using"/> statement.
/// </summary>
public readonly ref struct NoClientNotificationsScope
{
    private readonly AppContextSwitchScope _switchScope;

    public NoClientNotificationsScope(bool enable)
    {
        // Prevent multiple NoClientNotificationsScopes from running simultaneously. Using Monitor to allow recursion on
        // the same thread.
        Monitor.Enter(typeof(NoClientNotificationsScope));
        _switchScope = new(WinFormsAppContextSwitchNames.NoClientNotifications, GetDefaultValue, enable);
    }

    public void Dispose()
    {
        try
        {
            _switchScope.Dispose();
        }
        finally
        {
            Monitor.Exit(typeof(NoClientNotificationsScope));
        }
    }

    public static bool GetDefaultValue() =>
        typeof(LocalAppContextSwitches).TestAccessor()
            .CreateDelegate<Func<string, bool>>("GetSwitchDefaultValue")(WinFormsAppContextSwitchNames.NoClientNotifications);
}
