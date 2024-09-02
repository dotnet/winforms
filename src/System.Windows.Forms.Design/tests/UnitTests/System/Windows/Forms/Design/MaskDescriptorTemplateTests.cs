// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Globalization;

namespace System.Windows.Forms.Design.Tests;

public class MaskDescriptorTemplateTests
{
    [Theory]
    [InlineData("00000", "Numeric (5-digits)", "12345", typeof(int), "en-US", false, true)]
    [InlineData("invalid-mask", "Invalid Mask", "invalid", typeof(int), "en-US", false, false)]
    [InlineData("invalid-mask", "Invalid Mask", "invalid", typeof(int), "en-US", true, true)]
    [InlineData("00000", null, "sample", typeof(int), "en-US", false, false)]
    [InlineData("00000", "Numeric (5-digits)", null, typeof(int), "en-US", false, false)]
    [InlineData("00000", "Numeric (5-digits)", "12345", null, "en-US", false, false)]
    public void MaskDescriptorTemplate_Constructor_Validation(string? mask, string? name, string? sample, Type? validatingType, string? cultureName, bool skipValidation, bool isValid)
    {
        if (name is null || sample is null || validatingType is null || cultureName is null)
        {
            isValid.Should().BeFalse();
            return;
        }

        CultureInfo culture = new(cultureName);

        MaskDescriptorTemplate descriptor = new(mask, name, sample, validatingType, culture, skipValidation);

        descriptor.Name.Should().Be(name);
        descriptor.Sample.Should().Be(sample);
        descriptor.ValidatingType.Should().Be(validatingType);
        descriptor.Culture.Should().Be(culture);

        if (isValid)
        {
            descriptor.Culture.Should().Be(culture);
        }
        else
        {
            descriptor.Mask.Should().BeNull();
        }
    }

    [Theory]
    [InlineData("en-US", 9)]
    [InlineData("fr-FR", 7)]
    [InlineData("de-DE", 4)]
    [InlineData("ko-KR", 12)]
    [InlineData("zh-CHT", 10)]
    [InlineData("zh-CHS", 13)]
    [InlineData("ja-JP", 14)]
    [InlineData("es-ES", 9)]
    [InlineData("it-IT", 7)]
    [InlineData("ar-SA", 5)]
    public void GetLocalizedMaskDescriptors_ReturnsCorrectDescriptors(string cultureName, int templateCount)
    {
        CultureInfo culture = new(cultureName);

        var descriptors = MaskDescriptorTemplate.GetLocalizedMaskDescriptors(culture);

        descriptors.Should().NotBeEmpty();
        descriptors.Count.Should().Be(templateCount);
    }
}
