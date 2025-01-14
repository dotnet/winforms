// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public sealed class MaskedTextBoxDesignerTests
{
    [Fact]
    public void SnapLines_WithDefaultMaskedTextBox_ShouldReturnExpectedCount()
    {
        using MaskedTextBoxDesigner maskedTextBoxDesigner = new();
        using MaskedTextBox maskedTextBox = new();
        maskedTextBoxDesigner.Initialize(maskedTextBox);

        maskedTextBoxDesigner.SnapLines.Count.Should().Be(9);
    }

    [Fact]
    public void SelectionRules_WithDefaultMaskedTextBox_ShouldReturnExpectedValue()
    {
        using MaskedTextBoxDesigner maskedTextBoxDesigner = new();
        using MaskedTextBox maskedTextBox = new();
        maskedTextBoxDesigner.Initialize(maskedTextBox);

        SelectionRules selectionRules;
        using (new NoAssertContext())
        {
            selectionRules = maskedTextBoxDesigner.SelectionRules;
        }

        selectionRules.Should().Be(SelectionRules.LeftSizeable | SelectionRules.RightSizeable | SelectionRules.Moveable | SelectionRules.Visible);
    }

    [Fact]
    public void Verbs_WithDefaultMaskedTextBox_ShouldReturnExpectedCount()
    {
        using MaskedTextBoxDesigner maskedTextBoxDesigner = new();
        using MaskedTextBox maskedTextBox = new();
        maskedTextBoxDesigner.Initialize(maskedTextBox);

        maskedTextBoxDesigner.Verbs.Count.Should().Be(1);
    }

    [Fact]
    public void Verbs_WithDefaultMaskedTextBox_ShouldReturnExpectedVerbDescription()
    {
        using MaskedTextBoxDesigner maskedTextBoxDesigner = new();
        using MaskedTextBox maskedTextBox = new();
        maskedTextBoxDesigner.Initialize(maskedTextBox);

        DesignerVerbCollection verbs = maskedTextBoxDesigner.Verbs;
        verbs.Count.Should().Be(1);
        verbs[0].Text.Should().Be(SR.MaskedTextBoxDesignerVerbsSetMaskDesc);
    }

    [Fact]
    public void GetDesignMaskedTextBox_WithNullInput_ShouldReturnDefaultMaskedTextBox()
    {
        MaskedTextBox designMaskedTextBox = MaskedTextBoxDesigner.GetDesignMaskedTextBox(null!);

        designMaskedTextBox.Should().NotBeNull();
        designMaskedTextBox.Mask.Should().BeEmpty();
        designMaskedTextBox.Text.Should().BeEmpty();
    }

    [Fact]
    public void GetDesignMaskedTextBox_WithMaskedTextBox_ShouldCloneProperties()
    {
        using MaskedTextBox maskedTextBox = new()
        {
            Text = "123",
            ValidatingType = typeof(int),
            BeepOnError = true,
            InsertKeyMode = InsertKeyMode.Overwrite,
            RejectInputOnFirstFailure = true,
            CutCopyMaskFormat = MaskFormat.IncludePrompt,
            Culture = new CultureInfo("en-US")
        };

        MaskedTextBox designMaskedTextBox = MaskedTextBoxDesigner.GetDesignMaskedTextBox(maskedTextBox);

        designMaskedTextBox.Should().NotBeNull();
        designMaskedTextBox.Text.Should().Be("123");
        designMaskedTextBox.ValidatingType.Should().Be(typeof(int));
        designMaskedTextBox.BeepOnError.Should().BeTrue();
        designMaskedTextBox.InsertKeyMode.Should().Be(InsertKeyMode.Overwrite);
        designMaskedTextBox.RejectInputOnFirstFailure.Should().BeTrue();
        designMaskedTextBox.CutCopyMaskFormat.Should().Be(MaskFormat.IncludePrompt);
        designMaskedTextBox.Culture.Should().Be(new CultureInfo("en-US"));
    }

    [Fact]
    public void ActionLists_WithDefaultMaskedTextBox_ShouldReturnExpectedCount()
    {
        using MaskedTextBoxDesigner maskedTextBoxDesigner = new();
        using MaskedTextBox maskedTextBox = new();
        maskedTextBoxDesigner.Initialize(maskedTextBox);

        Mock<IServiceContainer> serviceContainer = new();
        Mock<ITypeDiscoveryService> typeDiscoveryService = new();
        Mock<IUIService> uiService = new();

        serviceContainer.Setup(s => s.GetService(typeof(ITypeDiscoveryService))).Returns(typeDiscoveryService.Object);
        serviceContainer.Setup(s => s.GetService(typeof(IUIService))).Returns(uiService.Object);

        Mock<ISite> mockSite = new();
        mockSite.Setup(s => s.GetService(typeof(IServiceContainer))).Returns(serviceContainer.Object);
        mockSite.Setup(s => s.GetService(typeof(ITypeDiscoveryService))).Returns(typeDiscoveryService.Object);
        mockSite.Setup(s => s.GetService(typeof(IUIService))).Returns(uiService.Object);
        maskedTextBoxDesigner.Component.Site = mockSite.Object;

        DesignerActionListCollection actionLists = maskedTextBoxDesigner.ActionLists;
        actionLists.Count.Should().Be(1);
    }

    [Fact]
    public void ActionLists_WithDefaultMaskedTextBox_ShouldReturnExpectedType()
    {
        using MaskedTextBoxDesigner maskedTextBoxDesigner = new();
        using MaskedTextBox maskedTextBox = new();
        maskedTextBoxDesigner.Initialize(maskedTextBox);

        Mock<IServiceContainer> serviceContainer = new();
        Mock<ITypeDiscoveryService> typeDiscoveryService = new();
        Mock<IUIService> uiService = new();

        serviceContainer.Setup(s => s.GetService(typeof(ITypeDiscoveryService))).Returns(typeDiscoveryService.Object);
        serviceContainer.Setup(s => s.GetService(typeof(IUIService))).Returns(uiService.Object);

        Mock<ISite> mockSite = new();
        mockSite.Setup(s => s.GetService(typeof(IServiceContainer))).Returns(serviceContainer.Object);
        mockSite.Setup(s => s.GetService(typeof(ITypeDiscoveryService))).Returns(typeDiscoveryService.Object);
        mockSite.Setup(s => s.GetService(typeof(IUIService))).Returns(uiService.Object);
        maskedTextBoxDesigner.Component.Site = mockSite.Object;

        DesignerActionListCollection actionLists = maskedTextBoxDesigner.ActionLists;
        actionLists[0].Should().BeOfType<MaskedTextBoxDesignerActionList>();
    }

    [Fact]
    public void GetDesignMaskedTextBox_WhenInputIsNull_ShouldReturnDefaultMaskedTextBox()
    {
        MaskedTextBox designMaskedTextBox = MaskedTextBoxDesigner.GetDesignMaskedTextBox(null!);

        designMaskedTextBox.Should().NotBeNull();
        designMaskedTextBox.Mask.Should().BeEmpty();
        designMaskedTextBox.Text.Should().BeEmpty();
    }
}
