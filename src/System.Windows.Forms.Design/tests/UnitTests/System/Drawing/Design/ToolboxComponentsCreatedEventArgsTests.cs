// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Drawing.Design.Tests;

public class ToolboxComponentsCreatedEventArgsTests
{
    public static IEnumerable<object[]> Ctor_IComponentArray_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { Array.Empty<IComponent>() };
        yield return new object[] { new IComponent[] { null } };
    }

    [Theory]
    [MemberData(nameof(Ctor_IComponentArray_TestData))]
    public void Ctor_IComponentArray(IComponent[] components)
    {
        ToolboxComponentsCreatedEventArgs e = new(components);
        if (components is null)
        {
            Assert.Null(e.Components);
        }
        else
        {
            Assert.Equal(components, e.Components);
            Assert.NotSame(components, e.Components);
            Assert.Equal(e.Components, e.Components);
            Assert.NotSame(e.Components, e.Components);
        }
    }
}
