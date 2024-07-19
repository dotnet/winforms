// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Globalization;

namespace System.Windows.Forms.Design.Tests;

public class MaskDescriptorTests : IDisposable
{
    private readonly CultureInfo _originalCulture;

    public MaskDescriptorTests()
    {
        _originalCulture = Thread.CurrentThread.CurrentCulture;
    }

    public void Dispose()
    {
        Thread.CurrentThread.CurrentCulture = _originalCulture;
    }

    private class ConcreteMaskDescriptor : MaskDescriptor
    {
        private readonly string? _name;
        private readonly string? _sample;
        private readonly Type? _validatingType;

        public override string? Mask { get; }

        public override string? Name => _name;

        public override string? Sample => _sample;

        public override Type? ValidatingType => _validatingType;

        public ConcreteMaskDescriptor(string? mask, string? name = null, string? sample = null, Type? validatingType = null)
        {
            Mask = mask;
            _name = name;
            _sample = sample;
            _validatingType = validatingType;
        }
    }

    [WinFormsTheory]
    [InlineData("00/00/0000")]
    [InlineData(null)]
    public void Mask_Property_ShouldReturnExpectedValue(string? inputMask)
    {
        ConcreteMaskDescriptor maskDescriptor = new(inputMask);
        string? actualMask = maskDescriptor.Mask;

        actualMask.Should().Be(inputMask);
    }

    [WinFormsTheory]
    [InlineData("en-US")]
    [InlineData("fr-FR")]
    public void Culture_Property_ShouldReflectCurrentThreadCulture(string cultureName)
    {
        CultureInfo newCulture = new(cultureName);
        Thread.CurrentThread.CurrentCulture = newCulture;

        ConcreteMaskDescriptor maskDescriptor = new("00/00/0000");
        var actualCulture = maskDescriptor.Culture;

        actualCulture.Should().Be(newCulture);
    }

    [WinFormsTheory]
    [InlineData("00/00/0000", "Date", "12/31/2000", true, "")]
    [InlineData(null, null, null, false, "Descriptor cannot be null.")]
    [InlineData("", "Empty Mask", "", false, "Mask cannot be empty.")]
    [InlineData("00/00/0000", "Date", "InvalidSample", false, "Sample is invalid.")]
    [InlineData("00/00/0000", null, "12/31/2000", false, "Name cannot be null.")]
    [InlineData("00/00/0000", "Date", null, false, "Sample cannot be null.")]
    public void IsValidMaskDescriptor_VariousScenarios(string? mask, string? name, string? sample, bool expectedIsValid, string expectedErrorDescription)
    {
        ConcreteMaskDescriptor? maskDescriptor = mask is not null ? new(mask, name, sample) : null;

        bool isValid = MaskDescriptor.IsValidMaskDescriptor(maskDescriptor, out string validationErrorDescription);

        isValid.Should().Be(expectedIsValid);
        if (!expectedIsValid)
        {
            validationErrorDescription.Should().NotBeEmpty();
        }
        else
        {
            validationErrorDescription.Should().Be(expectedErrorDescription);
        }
    }

    [WinFormsTheory]
    [InlineData("", "", "", typeof(object), false)]
    [InlineData("00/00/0000", "Date", "12/31/2000", typeof(DateTime), true)]
    [InlineData("DifferentType", "", "", typeof(object), false)]
    public void Equals_VariousScenarios(string mask2, string name2, string sample2, Type type2, bool expected)
    {
        string mask1 = "00/00/0000";
        string name1 = "Date";
        string sample1 = "12/31/2000";
        Type type1 = typeof(DateTime);

        ConcreteMaskDescriptor descriptor1 = new(mask: mask1, name: name1, sample: sample1, validatingType: type1);

        object descriptor2 = mask2 != "DifferentType"
            ? new ConcreteMaskDescriptor(mask: mask2, name: name2, sample: sample2, validatingType: type2)
            : new object();

        bool result = descriptor1.Equals(descriptor2);

        result.Should().Be(expected, $"because the comparison should result in {expected}.");
    }

    [WinFormsTheory]
    [InlineData("00/00/0000", "Date", "12/31/2000", typeof(DateTime), true, "Identical descriptors should produce the same hash code.")]
    [InlineData("000-00-0000", "SSN", "123-45-6789", typeof(DateTime), false, "Descriptors with different masks should produce different hash codes.")]
    [InlineData("00/00/0000", "Date", "12/31/2000", typeof(string), false, "Descriptors with different validating types should produce different hash codes.")]
    public void GetHashCode_VariousScenarios(string mask2, string name2, string sample2, Type type2, bool shouldMatch, string reason, string culture1 = "", string culture2 = "")
    {
        try
        {
            string mask1 = "00/00/0000";
            string name1 = "Date";
            string sample1 = "12/31/2000";
            Type type1 = typeof(DateTime);

            if (!string.IsNullOrEmpty(culture1))
            {
                Thread.CurrentThread.CurrentCulture = new(culture1);
            }

            ConcreteMaskDescriptor descriptor1 = new(mask: mask1, name: name1, sample: sample1, validatingType: type1);

            Thread.CurrentThread.CurrentCulture = _originalCulture;

            if (!string.IsNullOrEmpty(culture2))
            {
                Thread.CurrentThread.CurrentCulture = new(culture2);
            }

            ConcreteMaskDescriptor descriptor2 = new(mask: mask2, name: name2, sample: sample2, validatingType: type2);

            if (shouldMatch)
            {
                descriptor1.GetHashCode().Should().Be(descriptor2.GetHashCode(), reason);
            }
            else
            {
                descriptor1.GetHashCode().Should().NotBe(descriptor2.GetHashCode(), reason);
            }
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = _originalCulture;
        }
    }

    [WinFormsTheory]
    [InlineData("00/00/0000", "Date", "12/31/2000", typeof(DateTime), "System.Windows.Forms.Design.Tests.MaskDescriptorTests+ConcreteMaskDescriptor<Name=Date, Mask=00/00/0000, ValidatingType=System.DateTime")]
    [InlineData(null, null, null, null, "System.Windows.Forms.Design.Tests.MaskDescriptorTests+ConcreteMaskDescriptor<Name=null, Mask=null, ValidatingType=null")]
    [InlineData("", "", "", null, "System.Windows.Forms.Design.Tests.MaskDescriptorTests+ConcreteMaskDescriptor<Name=, Mask=, ValidatingType=null")]
    [InlineData("L", "Letter", "A", typeof(char), "System.Windows.Forms.Design.Tests.MaskDescriptorTests+ConcreteMaskDescriptor<Name=Letter, Mask=L, ValidatingType=System.Char")]
    [InlineData(null, "NameOnly", null, null, "System.Windows.Forms.Design.Tests.MaskDescriptorTests+ConcreteMaskDescriptor<Name=NameOnly, Mask=null, ValidatingType=null")]
    [InlineData("00/00/0000", null, "12/31/2000", null, "System.Windows.Forms.Design.Tests.MaskDescriptorTests+ConcreteMaskDescriptor<Name=null, Mask=00/00/0000, ValidatingType=null")]
    [InlineData("00/00/0000", "Date", null, typeof(DateTime), "System.Windows.Forms.Design.Tests.MaskDescriptorTests+ConcreteMaskDescriptor<Name=Date, Mask=00/00/0000, ValidatingType=System.DateTime")]
    public void ConcreteMaskDescriptor_ToString_ShouldReturnExpectedFormat(string? mask, string? name, string? sample, Type? validatingType, string expectedOutput)
    {
        ConcreteMaskDescriptor descriptor = new(mask, name, sample, validatingType);
        string actualOutput = descriptor.ToString();
        actualOutput.Should().Be(expectedOutput);
    }
}
