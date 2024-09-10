﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;
/// <summary>
///  Scope for enabling / disabling the TreeNodeCollectionAddRangeRespectsSortOrder Switch.
///  Use in a <see langword="using"/> statement.
/// </summary>
public readonly ref struct TreeNodeCollectionAddRangeRespectsSortOrderScope
{
    private readonly AppContextSwitchScope _switchScope;

    public TreeNodeCollectionAddRangeRespectsSortOrderScope(bool enable)
    {
        // Prevent multiple TreeNodeCollectionAddRangeRespectsSortOrderScopes from running simultaneously.
        // Using Monitor to allow recursion on the same thread.
        Monitor.Enter(typeof(TreeNodeCollectionAddRangeRespectsSortOrderScope));
        _switchScope = new(AppContextSwitchNames.TreeNodeCollectionAddRangeRespectsSortOrder, enable);
    }

    public void Dispose()
    {
        try
        {
            _switchScope.Dispose();
        }
        finally
        {
            Monitor.Exit(typeof(TreeNodeCollectionAddRangeRespectsSortOrderScope));
        }
    }
}
