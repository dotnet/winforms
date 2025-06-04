// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Primitives;

namespace System;

/// <summary>
///  Scope for enabling / disabling the ApplyParentFontToMenus Switch.
///  Use in a <see langword="using"/> statement.
/// </summary>
public readonly ref struct ApplyParentFontToMenusScope
{
    private readonly AppContextSwitchScope _switchScope;

    public ApplyParentFontToMenusScope(bool enable)
    {
        // Prevent multiple ApplyParentFontToMenusScopes from running simultaneously. Using Monitor to allow recursion on
        // the same thread.
        Monitor.Enter(typeof(ApplyParentFontToMenusScope));
        _switchScope = new(WinFormsAppContextSwitchNames.ApplyParentFontToMenus, GetDefaultValue, enable);
    }

    public void Dispose()
    {
        try
        {
            _switchScope.Dispose();
        }
        finally
        {
            Monitor.Exit(typeof(ApplyParentFontToMenusScope));
        }
    }

    public static bool GetDefaultValue() =>
        typeof(LocalAppContextSwitches).TestAccessor()
            .CreateDelegate<Func<string, bool>>("GetSwitchDefaultValue")(WinFormsAppContextSwitchNames.ApplyParentFontToMenus);
}
