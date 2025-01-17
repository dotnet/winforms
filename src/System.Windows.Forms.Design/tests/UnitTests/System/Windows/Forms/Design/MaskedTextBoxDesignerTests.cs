// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public sealed class MaskedTextBoxDesignerTests : IDisposable
{
    public void Dispose() { }

    [Fact]
    public void SnapLines_WithDefaultMaskedTextBox_ShouldReturnExpectedCount()
    {
        using MaskedTextBox _maskedTextBox = new();
        using MaskedTextBoxDesigner _maskedTextBoxDesigner = new();
        _maskedTextBoxDesigner.Initialize(_maskedTextBox);

        _maskedTextBoxDesigner.SnapLines.Count.Should().Be(9);
    }

    [Fact]
    public void SelectionRules_WithDefaultMaskedTextBox_ShouldReturnExpectedValue()
    {
        using MaskedTextBox _maskedTextBox = new();
        using MaskedTextBoxDesigner _maskedTextBoxDesigner = new();
        _maskedTextBoxDesigner.Initialize(_maskedTextBox);

        SelectionRules selectionRules;
        using (new NoAssertContext())
        {
            selectionRules = _maskedTextBoxDesigner.SelectionRules;
        }

        selectionRules.Should().Be(SelectionRules.LeftSizeable | SelectionRules.RightSizeable | SelectionRules.Moveable | SelectionRules.Visible);
    }

    [Fact]
    public void Verbs_WithDefaultMaskedTextBox_ShouldReturnExpectedCount()
    {
        using MaskedTextBox _maskedTextBox = new();
        using MaskedTextBoxDesigner _maskedTextBoxDesigner = new();
        _maskedTextBoxDesigner.Initialize(_maskedTextBox);

        _maskedTextBoxDesigner.Verbs.Count.Should().Be(1);
    }

    [Fact]
    public void Verbs_WithDefaultMaskedTextBox_ShouldContainSetMaskVerb()
    {
        using MaskedTextBox _maskedTextBox = new();
        using MaskedTextBoxDesigner _maskedTextBoxDesigner = new();
        _maskedTextBoxDesigner.Initialize(_maskedTextBox);

        DesignerVerbCollection verbs = _maskedTextBoxDesigner.Verbs;

        verbs.Should().NotBeNull();
        verbs.Count.Should().BeGreaterThan(0);
        DesignerVerb? firstVerb = verbs.Count > 0 ? verbs[0] : null;
        firstVerb.Should().NotBeNull();
        firstVerb!.Text.Should().Be(SR.MaskedTextBoxDesignerVerbsSetMaskDesc);
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
        IServiceContainer serviceContainer = new Mock<IServiceContainer>().Object;
        ITypeDiscoveryService typeDiscoveryService = new Mock<ITypeDiscoveryService>().Object;
        IUIService uiService = new Mock<IUIService>().Object;
        ISite mockSite = new Mock<ISite>().Object;

        Mock<IServiceContainer> mockServiceContainer = new();
        mockServiceContainer.Setup(s => s.GetService(typeof(ITypeDiscoveryService))).Returns(typeDiscoveryService);
        mockServiceContainer.Setup(s => s.GetService(typeof(IUIService))).Returns(uiService);

        Mock<ISite> mockMockSite = new();
        mockMockSite.Setup(s => s.GetService(typeof(IServiceContainer))).Returns(mockServiceContainer.Object);
        mockMockSite.Setup(s => s.GetService(typeof(ITypeDiscoveryService))).Returns(typeDiscoveryService);
        mockMockSite.Setup(s => s.GetService(typeof(IUIService))).Returns(uiService);

        using MaskedTextBox maskedTextBox = new();
        using MaskedTextBoxDesigner maskedTextBoxDesigner = new();
        maskedTextBoxDesigner.Initialize(maskedTextBox);
        maskedTextBoxDesigner.Component.Site = mockMockSite.Object;

        DesignerActionListCollection actionLists = maskedTextBoxDesigner.ActionLists;
        actionLists.Count.Should().Be(1);
    }

    [Fact]
    public void ActionLists_WithDefaultMaskedTextBox_ShouldReturnExpectedType()
    {
        IServiceContainer serviceContainer = new Mock<IServiceContainer>().Object;
        ITypeDiscoveryService typeDiscoveryService = new Mock<ITypeDiscoveryService>().Object;
        IUIService uiService = new Mock<IUIService>().Object;
        ISite mockSite = new Mock<ISite>().Object;

        Mock<IServiceContainer> mockServiceContainer = new();
        mockServiceContainer.Setup(s => s.GetService(typeof(ITypeDiscoveryService))).Returns(typeDiscoveryService);
        mockServiceContainer.Setup(s => s.GetService(typeof(IUIService))).Returns(uiService);

        Mock<ISite> mockMockSite = new();
        mockMockSite.Setup(s => s.GetService(typeof(IServiceContainer))).Returns(mockServiceContainer.Object);
        mockMockSite.Setup(s => s.GetService(typeof(ITypeDiscoveryService))).Returns(typeDiscoveryService);
        mockMockSite.Setup(s => s.GetService(typeof(IUIService))).Returns(uiService);

        using MaskedTextBox maskedTextBox = new();
        using MaskedTextBoxDesigner maskedTextBoxDesigner = new();
        maskedTextBoxDesigner.Initialize(maskedTextBox);
        maskedTextBoxDesigner.Component.Site = mockMockSite.Object;

        DesignerActionListCollection actionLists = maskedTextBoxDesigner.ActionLists;
        actionLists[0].Should().BeOfType<MaskedTextBoxDesignerActionList>();
    }
}
