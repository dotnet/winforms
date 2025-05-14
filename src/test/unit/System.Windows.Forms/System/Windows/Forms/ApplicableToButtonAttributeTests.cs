// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Controls.Buttons.Tests;

public class ApplicableToButtonAttributeTests
{
    [Fact]
    public void Ctor_Default_CreatesInstance()
    {
        ApplicableToButtonAttribute attr = new();
        attr.Should().NotBeNull();
        attr.Should().BeAssignableTo<Attribute>();
    }
}
