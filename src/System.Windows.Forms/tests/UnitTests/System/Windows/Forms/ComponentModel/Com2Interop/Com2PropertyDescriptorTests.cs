// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

namespace System.Windows.Forms.ComponentModel.Com2Interop.Tests;

// NB: doesn't require thread affinity
public class Com2PropertyDescriptorTests
{
    private static readonly MethodInfo s_miVersionInfo;
    private static readonly Type s_typeCom2PropertyDescriptor;

    static Com2PropertyDescriptorTests()
    {
        s_typeCom2PropertyDescriptor = typeof(Com2PropertyDescriptor);
        s_miVersionInfo = s_typeCom2PropertyDescriptor.GetMethod("TrimNewline", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.NotNull(s_miVersionInfo);
    }
}
