﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;
/// <summary>
///  Scope for enabling / disabling the DataGridViewUIAStartRowCountAtZero Switch.
///  Use in a <see langword="using"/> statement.
/// </summary>
public readonly ref struct DataGridViewUIAStartRowCountAtZeroScope
{
    private readonly AppContextSwitchScope _switchScope;

    public DataGridViewUIAStartRowCountAtZeroScope(bool enable)
    {
        // Prevent multiple BinaryFormatterScopes from running simultaneously. Using Monitor to allow recursion on
        // the same thread.
        Monitor.Enter(typeof(DataGridViewUIAStartRowCountAtZeroScope));
        _switchScope = new(WinFormsAppContextSwitchNames.DataGridViewUIAStartRowCountAtZero, enable);
    }

    public void Dispose()
    {
        try
        {
            _switchScope.Dispose();
        }
        finally
        {
            Monitor.Exit(typeof(DataGridViewUIAStartRowCountAtZeroScope));
        }
    }
}
