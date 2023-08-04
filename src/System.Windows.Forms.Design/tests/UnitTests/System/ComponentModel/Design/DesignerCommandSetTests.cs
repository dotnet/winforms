﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Moq;

namespace System.ComponentModel.Design.Tests;

public class DesignerCommandSetTests
{
    [Fact]
    public void DesignerCommandSet_Ctor_Default()
    {
        DesignerCommandSet set = new();
        Assert.Null(set.ActionLists);
        Assert.Null(set.Verbs);
    }

    [Theory]
    [StringWithNullData]
    public void DesignerCommandSet_GetCommands_Invoke_ReturnsNull(string name)
    {
        DesignerCommandSet set = new();
        Assert.Null(set.GetCommands(name));
    }

    [Fact]
    public void DesignerCommandSet_Verbs_OverridenGetCommands_ReturnsExpected()
    {
        DesignerVerbCollection collection = new();
        Mock<DesignerCommandSet> mockSet = new(MockBehavior.Strict);
        mockSet
            .Setup(s => s.GetCommands("Verbs"))
            .Returns(collection);
        Assert.Same(collection, mockSet.Object.Verbs);
    }

    [Fact]
    public void DesignerCommandSet_Verbs_InvalidOverridenGetCommands_ThrowsInvalidCastException()
    {
        Mock<DesignerCommandSet> mockSet = new(MockBehavior.Strict);
        mockSet
            .Setup(s => s.GetCommands("Verbs"))
            .Returns(Array.Empty<object>());
        Assert.Throws<InvalidCastException>(() => mockSet.Object.Verbs);
    }

    [Fact]
    public void DesignerCommandSet_ActionLists_OverridenGetCommands_ReturnsExpected()
    {
        DesignerActionListCollection collection = new();
        Mock<DesignerCommandSet> mockSet = new(MockBehavior.Strict);
        mockSet
            .Setup(s => s.GetCommands("ActionLists"))
            .Returns(collection);
        Assert.Same(collection, mockSet.Object.ActionLists);
    }

    [Fact]
    public void DesignerCommandSet_ActionLists_InvalidOverridenGetCommands_ThrowsInvalidCastException()
    {
        Mock<DesignerCommandSet> mockSet = new(MockBehavior.Strict);
        mockSet
            .Setup(s => s.GetCommands("ActionLists"))
            .Returns(Array.Empty<object>());
        Assert.Throws<InvalidCastException>(() => mockSet.Object.ActionLists);
    }
}
