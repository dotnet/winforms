﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.ComponentModel.Design;

internal static class DesignerActionItemCollectionExtensions
{
    public static int Add(this DesignerActionItemCollection collection, object value) => value is DesignerActionItem item
        ? collection.Add(item)
        : throw new ArgumentException($"Value must be of type {nameof(DesignerActionItem)}", nameof(value));
}

public class DesignerActionItemCollection : CollectionBase
{
    public DesignerActionItem this[int index]
    {
        get => (DesignerActionItem)List[index]!;
        set => List[index] = value;
    }

    public int Add(DesignerActionItem value) => List.Add(value);

    public bool Contains(DesignerActionItem value) => List.Contains(value);

    public void CopyTo(DesignerActionItem[] array, int index) => List.CopyTo(array, index);

    public int IndexOf(DesignerActionItem value) => List.IndexOf(value);

    public void Insert(int index, DesignerActionItem value) => List.Insert(index, value);

    public void Remove(DesignerActionItem value) => List.Remove(value);
}
