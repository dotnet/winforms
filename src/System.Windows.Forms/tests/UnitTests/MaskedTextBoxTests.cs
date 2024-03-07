// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms.Tests;

public class MaskedTextBoxTests
{
    [WinFormsFact]
    public void MaskedTextBox_Constructor()
    {
        using MaskedTextBox mtb = new();

        Assert.NotNull(mtb);
    }

    [WinFormsFact]
    public void MaskedTextBox_ConstructorString()
    {
        using MaskedTextBox mtb = new("Hello World!");

        Assert.NotNull(mtb);
    }

    [WinFormsFact]
    public void MaskedTextBox_ConstructorMaskedTextProvider()
    {
        using MaskedTextBox mtb = new(new MaskedTextProvider("Hello World!"));

        Assert.NotNull(mtb);
    }

    [WinFormsFact]
    public void MaskedTextBox_DefaultValues()
    {
        using MaskedTextBox control = new();

        // Check default values
        control.BeepOnError.Should().BeFalse();
        control.AsciiOnly.Should().BeFalse();
        control.Culture.Should().Be(CultureInfo.CurrentCulture);

        control.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsTheory]
    [BoolData]
    public void MaskedTextBox_BeepOnError_Set_GetReturnsExpected(bool value)
    {
        using MaskedTextBox control = new()
        {
            BeepOnError = value
        };

        control.BeepOnError.Should().Be(value);
        control.IsHandleCreated.Should().BeFalse();

        // Set same.
        control.BeepOnError = value;
        control.BeepOnError.Should().Be(value);
        control.IsHandleCreated.Should().BeFalse();

        // Set different.
        control.BeepOnError = !value;
        control.BeepOnError.Should().Be(!value);;
        control.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsTheory]
    [BoolData]
    public void MaskedTextBox_BeepOnError_SetWithHandle_GetReturnsExpected(bool value)
    {
        using MaskedTextBox control = new();
        control.Handle.Should().NotBe(IntPtr.Zero);

        // These events are tested to ensure that they are not accidentally triggered when the BeepOnError property is set.
        // The Invalidated event is triggered when the control or part of it needs to be redrawn. If the setting of BeepOnError causes the control to be redrawn, it may trigger unnecessary performance overhead.
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;

        // The StyleChanged event is triggered when the style of the control has changed. If the setting of BeepOnError causes a style change, it may affect the appearance or behavior of the control.
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;

        // The HandleCreated event is triggered when a handle to the control is created. If the setting of BeepOnError causes a handle to be created, it may affect the control's lifecycle or behavior.
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.BeepOnError = value;
        control.BeepOnError.Should().Be(value);
        control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Set same.
        control.BeepOnError = value;
        control.BeepOnError.Should().Be(value);
        control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Set different.
        control.BeepOnError = !value;
        control.BeepOnError.Should().Be(!value);
        control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);
    }

    [WinFormsTheory]
    [BoolData]
    public void MaskedTextBox_AsciiOnly_Set_GetReturnsExpected(bool value)
    {
        using MaskedTextBox control = new()
        {
            AsciiOnly = value
        };

        control.AsciiOnly.Should().Be(value);
        control.IsHandleCreated.Should().BeFalse();

        // Set same.
        control.AsciiOnly = value;
        control.AsciiOnly.Should().Be(value);
        control.IsHandleCreated.Should().BeFalse();

        // Set different.
        control.AsciiOnly = !value;
        control.AsciiOnly.Should().Be(!value);
        control.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsTheory]
    [BoolData]
    public void MaskedTextBox_AsciiOnly_SetWithHandle_GetReturnsExpected(bool value)
    {
        using MaskedTextBox control = new();
        control.Handle.Should().NotBe(IntPtr.Zero);

        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.AsciiOnly = value;
        control.AsciiOnly.Should().Be(value);
        control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Set same.
        control.AsciiOnly = value;
        control.AsciiOnly.Should().Be(value);
        control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Set different.
        control.AsciiOnly = !value;
        control.AsciiOnly.Should().Be(!value);
        control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);
    }

    [WinFormsTheory]
    [InlineData("en-US")]
    [InlineData("fr-FR")]
    [InlineData("es-ES")]
    [InlineData(null)]
    public void MaskedTextBox_Culture_SetCulture_UpdatesMaskedTextProvider(string cultureName)
    {
        if (cultureName is null)
        {
            using MaskedTextBox control = new();
            control.Invoking(y => y.Culture = null)
                .Should().Throw<ArgumentNullException>();
        }
        else
        {
            CultureInfo culture = new CultureInfo(cultureName);
            using MaskedTextBox control = new()
            {
                Culture = culture
            };

            control.Culture.Should().Be(culture);
            control.IsHandleCreated.Should().BeFalse();

            // Set same.
            control.Culture = culture;
            control.Culture.Should().Be(culture);
            control.IsHandleCreated.Should().BeFalse();

            // Set different.
            CultureInfo differentCulture = new CultureInfo(cultureName == "en-US" ? "fr-FR" : "en-US");
            control.Culture = differentCulture;
            control.Culture.Should().Be(differentCulture);
            control.IsHandleCreated.Should().BeFalse();
        }
    }

    [WinFormsTheory]
    [InlineData("en-US")]
    [InlineData("fr-FR")]
    [InlineData("es-ES")]
    public void MaskedTextBox_Culture_SetWithHandle_GetReturnsExpected(string cultureName)
    {
        CultureInfo culture = new CultureInfo(cultureName);
        using MaskedTextBox control = new();
        control.Handle.Should().NotBe(IntPtr.Zero);

        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Culture = culture;
        control.Culture.Should().Be(culture);
        control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Set same.
        control.Culture = culture;
        control.Culture.Should().Be(culture);
        control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Set different.
        CultureInfo differentCulture = new CultureInfo(cultureName == "en-US" ? "fr-FR" : "es-ES");
        control.Culture = differentCulture;
        control.Culture.Should().Be(differentCulture);
        control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);
    }
}
