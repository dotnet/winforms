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

    [WinFormsTheory]
    [BoolData]
    public void MaskedTextBox_BeepOnError_Set_GetReturnsExpected(bool value)
    {
        using MaskedTextBox control = new()
        {
            BeepOnError = value
        };
        Assert.Equal(value, control.BeepOnError);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BeepOnError = value;
        Assert.Equal(value, control.BeepOnError);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.BeepOnError = !value;
        Assert.Equal(!value, control.BeepOnError);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void MaskedTextBox_BeepOnError_SetWithHandle_GetReturnsExpected(bool value)
    {
        using MaskedTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.BeepOnError = value;
        Assert.Equal(value, control.BeepOnError);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.BeepOnError = value;
        Assert.Equal(value, control.BeepOnError);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.BeepOnError = !value;
        Assert.Equal(!value, control.BeepOnError);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void MaskedTextBox_AsciiOnly_Set_GetReturnsExpected(bool value)
    {
        using MaskedTextBox control = new()
        {
            AsciiOnly = value
        };
        Assert.Equal(value, control.AsciiOnly);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AsciiOnly = value;
        Assert.Equal(value, control.AsciiOnly);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AsciiOnly = !value;
        Assert.Equal(!value, control.AsciiOnly);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void MaskedTextBox_AsciiOnly_SetWithHandle_GetReturnsExpected(bool value)
    {
        using MaskedTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.AsciiOnly = value;
        Assert.Equal(value, control.AsciiOnly);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AsciiOnly = value;
        Assert.Equal(value, control.AsciiOnly);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AsciiOnly = !value;
        Assert.Equal(!value, control.AsciiOnly);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
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
            Assert.Throws<ArgumentNullException>(() => control.Culture = null);
        }
        else
        {
            CultureInfo culture = new CultureInfo(cultureName);
            using MaskedTextBox control = new()
            {
                Culture = culture
            };
            Assert.Equal(culture, control.Culture);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Culture = culture;
            Assert.Equal(culture, control.Culture);
            Assert.False(control.IsHandleCreated);

            // Set different.
            CultureInfo differentCulture = new CultureInfo(cultureName == "en-US" ? "fr-FR" : "en-US");
            control.Culture = differentCulture;
            Assert.Equal(differentCulture, control.Culture);
            Assert.False(control.IsHandleCreated);
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
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Culture = culture;
        Assert.Equal(culture, control.Culture);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Culture = culture;
        Assert.Equal(culture, control.Culture);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        CultureInfo differentCulture = new CultureInfo(cultureName == "en-US" ? "fr-FR" : "es-ES");
        control.Culture = differentCulture;
        Assert.Equal(differentCulture, control.Culture);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }
}
