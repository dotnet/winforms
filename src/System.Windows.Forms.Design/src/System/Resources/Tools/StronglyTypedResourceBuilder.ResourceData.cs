// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Resources.Tools;

public static partial class StronglyTypedResourceBuilder
{
    internal sealed class ResourceData
    {
        internal ResourceData(Type? type, string? valueAsString)
        {
            Type = type;
            ValueAsString = valueAsString;
        }

        internal Type? Type { get; }

        internal string? ValueAsString { get; }
    }
}
