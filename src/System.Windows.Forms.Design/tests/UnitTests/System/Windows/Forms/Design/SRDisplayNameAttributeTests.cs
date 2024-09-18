// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public class SRDisplayNameAttributeTests
{
    [Theory]
    [InlineData("TestDisplayName")]
    [InlineData("")]
    public void DisplayName_ShouldRetrieveResourceString(string testDisplayName)
    {
        SRDisplayNameAttribute attribute = new(testDisplayName);

        string displayName = attribute.DisplayName;

        displayName.Should().Be(SR.GetResourceString(testDisplayName));
    }

    [Fact]
    public void DisplayName_ShouldBeRetrievedOnlyOnce()
    {
        string testDisplayName = "TestDisplayName";
        SRDisplayNameAttribute attribute = new(testDisplayName);

        string displayName1 = attribute.DisplayName;
        string displayName2 = attribute.DisplayName;

        displayName1.Should().Be(displayName2);
    }
}
