// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

internal static class DesignerActionItemCollectionExtensions
{
    public static int Add(this DesignerActionItemCollection collection, object value) => value is DesignerActionItem item
       ? collection.Add(item)
       : throw new ArgumentException($"Value must be of type {nameof(DesignerActionItem)}", nameof(value));
}
