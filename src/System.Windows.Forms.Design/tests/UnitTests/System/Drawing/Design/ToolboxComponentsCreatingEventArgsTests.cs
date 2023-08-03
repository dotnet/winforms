// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using Moq;

namespace System.Drawing.Design.Tests;

public class ToolboxComponentsCreatingEventArgsTests
{
    public static IEnumerable<object[]> Ctor_IDesignerHost_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new Mock<IDesignerHost>(MockBehavior.Strict).Object };
    }

    [Theory]
    [MemberData(nameof(Ctor_IDesignerHost_TestData))]
    public void Ctor_IDesignerHost(IDesignerHost host)
    {
        ToolboxComponentsCreatingEventArgs e = new(host);
        Assert.Equal(host, e.DesignerHost);
    }
}
