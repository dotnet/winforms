﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
        _switchScope = new(AppContextSwitchNames.NoClientNotifications, enable);
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
}
