// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Primitives;

namespace System;

/// <summary>
///  Scope for enabling / disabling the ScaleTopLevelFormMinMaxSizeForDpi Switch.
///  Use in a <see langword="using"/> statement.
/// </summary>
public readonly ref struct ScaleTopLevelFormMinMaxSizeForDpiScope
{
    private readonly AppContextSwitchScope _switchScope;

    public ScaleTopLevelFormMinMaxSizeForDpiScope(bool enable)
    {
        // Prevent multiple ScaleTopLevelFormMinMaxSizeForDpi from running simultaneously.
        // Using Monitor to allow recursion on the same thread.
        Monitor.Enter(typeof(ScaleTopLevelFormMinMaxSizeForDpiScope));
        _switchScope = new(WinFormsAppContextSwitchNames.ScaleTopLevelFormMinMaxSizeForDpi, GetDefaultValue, enable);
    }

    public void Dispose()
    {
        try
        {
            _switchScope.Dispose();
        }
        finally
        {
            Monitor.Exit(typeof(ScaleTopLevelFormMinMaxSizeForDpiScope));
        }
    }

    public static bool GetDefaultValue() =>
        typeof(LocalAppContextSwitches).TestAccessor()
            .CreateDelegate<Func<string, bool>>("GetSwitchDefaultValue")(WinFormsAppContextSwitchNames.ScaleTopLevelFormMinMaxSizeForDpi);
}
