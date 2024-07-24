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
    public void MaskDescriptorTemplate_Constructor_Validation(string mask, string name, string sample, Type validatingType, string cultureName, bool skipValidation, bool isValid)
    {
        CultureInfo culture = new(cultureName);

        MaskDescriptorTemplate descriptor = new(mask, name, sample, validatingType, culture, skipValidation);

        if (isValid)
        {
            descriptor.Mask.Should().Be(mask);
            descriptor.Name.Should().Be(name);
            descriptor.Sample.Should().Be(sample);
            descriptor.ValidatingType.Should().Be(validatingType);
            descriptor.Culture.Should().Be(culture);
        }
        else
        {
            descriptor.Mask.Should().BeNull();
        }
    }

    [Theory]
    [InlineData("en-US", "Numeric (5-digits)", "Phone number")]
    [InlineData("fr-FR", "Numérique (5 chiffres)", "Numéro de téléphone (France)")]
    [InlineData("de-DE", "Datum kurz", "Postleitzahl")]
    public void GetLocalizedMaskDescriptors_ReturnsCorrectDescriptors(string cultureName, string expectedName1, string expectedName2)
    {
        CultureInfo culture = new(cultureName);

        var descriptors = MaskDescriptorTemplate.GetLocalizedMaskDescriptors(culture);

        descriptors.Should().NotBeEmpty();
        descriptors.Should().Contain(d => d.Name == expectedName1);
        descriptors.Should().Contain(d => d.Name == expectedName2);
    }
}
