// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design.Serialization;

internal sealed partial class ComponentCache
{
    internal readonly struct ResourceEntry(
        string name,
        object? value,
        bool forceInvariant,
        bool shouldSerializeValue,
        bool ensureInvariant,
        PropertyDescriptor? propertyDescriptor,
        ExpressionContext? expressionContext)
    {
        public readonly bool ForceInvariant = forceInvariant;
        public readonly bool EnsureInvariant = ensureInvariant;
        public readonly bool ShouldSerializeValue = shouldSerializeValue;
        public readonly string Name = name;
        public readonly object? Value = value;
        public readonly PropertyDescriptor? PropertyDescriptor = propertyDescriptor;
        public readonly ExpressionContext? ExpressionContext = expressionContext;
    }
}
