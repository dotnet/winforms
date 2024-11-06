// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

namespace System.Windows.Forms.ComponentModel.Com2Interop.Tests;

// NB: doesn't require thread affinity
public class Com2PropertyDescriptorTests
{
    private static readonly Type s_typeCom2PropertyDescriptor = typeof(Com2PropertyDescriptor);
    private static readonly MethodInfo s_miVersionInfo = s_typeCom2PropertyDescriptor.GetMethod("TrimNewline", BindingFlags.Static | BindingFlags.NonPublic);

    static Com2PropertyDescriptorTests()
    {
        Assert.NotNull(s_miVersionInfo);
    }
}
