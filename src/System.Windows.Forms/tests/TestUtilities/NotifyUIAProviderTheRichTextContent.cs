// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Primitives;

namespace System;

/// <summary>
///  Scope for enabling / disabling the NotifyUIAProviderTheText Switch.
/// </summary>
public readonly ref struct NotifyUIAProviderTheRichTextContent
{
    private readonly AppContextSwitchScope _switchScope;

    public NotifyUIAProviderTheRichTextContent(bool enable)
    {
        // Prevent multiple NotifyUIAProviderTheRichTextContent from running simultaneously. Using Monitor to allow recursion on
        // the same thread.
        Monitor.Enter(typeof(NotifyUIAProviderTheRichTextContent));
        _switchScope = new(WinFormsAppContextSwitchNames.NotifyUIAProviderTheText, GetDefaultValue, enable);
    }

    public void Dispose()
    {
        try
        {
            _switchScope.Dispose();
        }
        finally
        {
            Monitor.Exit(typeof(NotifyUIAProviderTheRichTextContent));
        }
    }

    public static bool GetDefaultValue() =>
        typeof(LocalAppContextSwitches).TestAccessor()
            .CreateDelegate<Func<string, bool>>("GetSwitchDefaultValue")(WinFormsAppContextSwitchNames.NotifyUIAProviderTheText);
}
