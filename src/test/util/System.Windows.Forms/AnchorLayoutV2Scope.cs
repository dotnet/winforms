// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Primitives;

namespace System;

/// <summary>
///  Scope for enabling / disabling the AnchorLayoutV2 Switch.
///  Use in a <see langword="using"/> statement.
/// </summary>
public readonly ref struct AnchorLayoutV2Scope
{
    private readonly AppContextSwitchScope _switchScope;

    public AnchorLayoutV2Scope(bool enable)
    {
        // Prevent multiple AnchorLayoutV2Scope instances from running simultaneously.
        // Using Monitor to allow recursion on the same thread.
        Monitor.Enter(typeof(AnchorLayoutV2Scope));
        _switchScope = new(WinFormsAppContextSwitchNames.AnchorLayoutV2, GetDefaultValue, enable);
    }

    public void Dispose()
    {
        try
        {
            _switchScope.Dispose();
        }
        finally
        {
            Monitor.Exit(typeof(AnchorLayoutV2Scope));
        }
    }

    public static bool GetDefaultValue() =>
        typeof(LocalAppContextSwitches).TestAccessor()
            .CreateDelegate<Func<string, bool>>("GetSwitchDefaultValue")(WinFormsAppContextSwitchNames.AnchorLayoutV2);
}
