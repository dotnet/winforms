// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.WinForms.Test;

[AttributeUsage(AttributeTargets.Class)]
public class AnalyzerTestPathAttribute : Attribute
{
    public AnalyzerTestPathAttribute(string path)
    {
        Path = path;
    }

    public string Path { get; }
}
