// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public sealed class MaskDesignerDialogTests : IDisposable
{
    private readonly MaskedTextBox _maskedTextBox = new();
    private readonly MaskDesignerDialog _dialog;

    public MaskDesignerDialogTests()
    {
        _dialog = new(_maskedTextBox, null);
    }

    public void Dispose()
    {
        _maskedTextBox.Dispose();
        _dialog.Dispose();
    }

    [Fact]
    public void Constructor_ValidMaskedTextBox_UsesProvidedMaskedTextBox()
    {
        using TextBox txtBoxMask = _dialog.TestAccessor().Dynamic._txtBoxMask;

        _dialog.Mask.Should().Be(_maskedTextBox.Mask);
        txtBoxMask.Text.Should().Be(_maskedTextBox.Mask);
    }

    [Fact]
    public void ValidatingTypeProperty_ShouldBeSetCorrectly()
    {
        _dialog.TestAccessor().Dynamic._maskedTextBox.ValidatingType = typeof(DateTime);
        _dialog.TestAccessor().Dynamic.btnOK_Click(null, EventArgs.Empty);

        _dialog.ValidatingType.Should().Be(typeof(DateTime));
    }

    [Fact]
    public void MaskDescriptorsEnumerator_ShouldReturnCorrectDescriptors()
    {
        Collections.IEnumerator enumerator = _dialog.MaskDescriptors;

        List<MaskDescriptor> descriptors = new();
        while (enumerator.MoveNext())
        {
            descriptors.Add((MaskDescriptor)enumerator.Current);
        }

        descriptors.Should().NotBeEmpty();
    }

    [Fact]
    public void DiscoverMaskDescriptors_ShouldHandleNullTypeDiscoveryService()
    {
        List<MaskDescriptor> initialDescriptors = _dialog.TestAccessor().Dynamic._maskDescriptors;

        _dialog.DiscoverMaskDescriptors(null);

        List<MaskDescriptor> maskDescriptors = _dialog.TestAccessor().Dynamic._maskDescriptors;

        maskDescriptors.Should().Equal(initialDescriptors);
    }

    [WinFormsTheory]
    [InlineData(typeof(ValidMaskDescriptor), true)]
    [InlineData(typeof(AbstractMaskDescriptor), false)]
    [InlineData(typeof(NonPublicMaskDescriptor), false)]
    public void DiscoverMaskDescriptors_ShouldHandleVariousDescriptorTypes(Type descriptorType, bool shouldBeAdded)
    {
        Mock<ITypeDiscoveryService> mockDiscoveryService = new();
        List<Type> types = new(){ descriptorType };

        mockDiscoveryService.Setup(ds => ds.GetTypes(typeof(MaskDescriptor), false)).Returns(types);

        _dialog.DiscoverMaskDescriptors(mockDiscoveryService.Object);
        List<MaskDescriptor> maskDescriptors = _dialog.TestAccessor().Dynamic._maskDescriptors;

        if (shouldBeAdded)
        {
            maskDescriptors.Should().ContainSingle(descriptor => descriptor.GetType() == descriptorType);
        }
        else
        {
            maskDescriptors.Should().NotContain(descriptor => descriptor.GetType() == descriptorType);
        }
    }

    private abstract class AbstractMaskDescriptor : MaskDescriptor
    {
        public override string? Mask => null;
        public override string? Name => null;
        public override string? Sample => null;
        public override Type? ValidatingType => null;
    }

    private class NonPublicMaskDescriptor : MaskDescriptor
    {
        public override string? Mask => null;
        public override string? Name => null;
        public override string? Sample => null;
        public override Type? ValidatingType => null;
    }
}

public class ValidMaskDescriptor : MaskDescriptor
{
    public override string? Mask => "000-00-0000";
    public override string? Name => "Valid Mask";
    public override string? Sample => "123-45-6789";
    public override Type? ValidatingType => typeof(string);
}
