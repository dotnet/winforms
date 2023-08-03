// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design.Tests;

public class ProjectTargetFrameworkAttributeTests
{
    [Theory]
    [StringWithNullData]
    public void ProjectTargetFrameworkAttribute_Ctor_String(string targetFrameworkMoniker)
    {
        ProjectTargetFrameworkAttribute attribute = new(targetFrameworkMoniker);
        Assert.Equal(targetFrameworkMoniker, attribute.TargetFrameworkMoniker);
    }
}
