// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
public sealed class ProjectTargetFrameworkAttribute : Attribute
{
    public ProjectTargetFrameworkAttribute(string targetFrameworkMoniker)
    {
        TargetFrameworkMoniker = targetFrameworkMoniker;
    }

    public string TargetFrameworkMoniker { get; }
}
