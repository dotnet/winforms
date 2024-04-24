﻿// Licensed to the .NET Foundation under one or more agreements.
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

        // Check default values
        mtb.BeepOnError.Should().BeFalse();
        mtb.AsciiOnly.Should().BeFalse();
        mtb.Culture.Should().Be(CultureInfo.CurrentCulture);
        mtb.AcceptsTab.Should().BeFalse();
        mtb.CanUndo.Should().BeFalse();
        mtb.WordWrap.Should().BeFalse();

        mtb.IsHandleCreated.Should().BeFalse();
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
    [InlineData("en-US", false)]
    [InlineData("fr-FR", false)]
    [InlineData("es-ES", false)]
    [InlineData(null, false)]
    [InlineData("en-US", true)]
    [InlineData("fr-FR", true)]
    [InlineData("es-ES", true)]
    public void MaskedTextBox_Culture_SetCulture_UpdatesMaskedTextProvider(string cultureName, bool createHandle)
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

            if (createHandle)
            {
                control.Handle.Should().NotBe(IntPtr.Zero);
            }

            control.Culture.Should().Be(culture);
            control.IsHandleCreated.Should().Be(createHandle);

            // Set same.
            control.Culture = culture;
            control.Culture.Should().Be(culture);
            control.IsHandleCreated.Should().Be(createHandle);

            // Set different.
            CultureInfo differentCulture = new CultureInfo(cultureName == "en-US" ? "fr-FR" : "en-US");
            control.Culture = differentCulture;
            control.Culture.Should().Be(differentCulture);
            control.IsHandleCreated.Should().Be(createHandle);

            if (createHandle)
            {
                int invalidatedCallCount = 0;
                control.Invalidated += (sender, e) => invalidatedCallCount++;
                int styleChangedCallCount = 0;
                control.StyleChanged += (sender, e) => styleChangedCallCount++;
                int createdCallCount = 0;
                control.HandleCreated += (sender, e) => createdCallCount++;

                invalidatedCallCount.Should().Be(0);
                styleChangedCallCount.Should().Be(0);
                createdCallCount.Should().Be(0);
            }
        }
    }

    [WinFormsTheory]
    [BoolData]
    public void MaskedTextBox_AllowPromptAsInput_Set_GetReturnsExpected(bool createHandle)
    {
        using MaskedTextBox control = new();

        if (createHandle)
        {
            control.Handle.Should().NotBe(IntPtr.Zero);
        }

        bool value = true;

        control.AllowPromptAsInput = value;
        control.AllowPromptAsInput.Should().Be(value);
        control.IsHandleCreated.Should().Be(createHandle);

        // Set same.
        control.AllowPromptAsInput = value;
        control.AllowPromptAsInput.Should().Be(value);
        control.IsHandleCreated.Should().Be(createHandle);

        // Set different.
        control.AllowPromptAsInput = !value;
        control.AllowPromptAsInput.Should().Be(!value);
        control.IsHandleCreated.Should().Be(createHandle);

        if (createHandle)
        {
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            invalidatedCallCount.Should().Be(0);
            styleChangedCallCount.Should().Be(0);
            createdCallCount.Should().Be(0);
        }
    }

    [WinFormsTheory]
    [InlineData(MaskFormat.IncludeLiterals)]
    [InlineData(MaskFormat.IncludePrompt)]
    [InlineData(MaskFormat.IncludePromptAndLiterals)]
    [InlineData(MaskFormat.ExcludePromptAndLiterals)]
    public void MaskedTextBox_CutCopyAndTextMaskFormat_Set_GetReturnsExpected(MaskFormat value)
    {
        using MaskedTextBox control = new();

        control.CutCopyMaskFormat = value;
        control.TextMaskFormat = value;
        control.CutCopyMaskFormat.Should().Be(value);
        control.TextMaskFormat.Should().Be(value);

        // Set same.
        control.CutCopyMaskFormat = value;
        control.TextMaskFormat = value;
        control.CutCopyMaskFormat.Should().Be(value);
        control.TextMaskFormat.Should().Be(value);

        // Test invalid value
        control.Invoking(c => c.CutCopyMaskFormat = (MaskFormat)4)
        .Should().Throw<InvalidEnumArgumentException>()
        .WithMessage("The value of argument 'value' (4) is invalid for Enum type 'MaskFormat'.*");

        control.Invoking(c => c.TextMaskFormat = (MaskFormat)4)
            .Should().Throw<InvalidEnumArgumentException>()
            .WithMessage("The value of argument 'value' (4) is invalid for Enum type 'MaskFormat'.*");
    }

    [WinFormsTheory]
    [InlineData(true, false)]
    [InlineData(false, false)]
    [InlineData(true, true)]
    [InlineData(false, true)]
    public void MaskedTextBox_HidePromptOnLeave_Set_GetReturnsExpected(bool value, bool createHandle)
    {
        using MaskedTextBox control = new();
        if (createHandle)
        {
            control.Handle.Should().NotBe(IntPtr.Zero);
        }

        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.HidePromptOnLeave = value;
        control.HidePromptOnLeave.Should().Be(value);
        control.IsHandleCreated.Should().Be(createHandle);
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Set same.
        control.HidePromptOnLeave = value;
        control.HidePromptOnLeave.Should().Be(value);
        control.IsHandleCreated.Should().Be(createHandle);
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Set different.
        control.HidePromptOnLeave = !value;
        control.HidePromptOnLeave.Should().Be(!value);
        control.IsHandleCreated.Should().Be(createHandle);
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);
    }

    [WinFormsTheory]
    [InlineData(InsertKeyMode.Default)]
    [InlineData(InsertKeyMode.Insert)]
    [InlineData(InsertKeyMode.Overwrite)]
    public void MaskedTextBox_InsertKeyMode_Set_GetReturnsExpected(InsertKeyMode value)
    {
        using MaskedTextBox control = new();
        control.InsertKeyMode = value;
        control.CreateControl();

        control.IsHandleCreated.Should().BeTrue();
        control.InsertKeyMode.Should().Be(value);

        // Set same.
        control.InsertKeyMode = value;
        control.IsHandleCreated.Should().BeTrue();

        // Set different.
        if (value != InsertKeyMode.Default)
        {
            control.InsertKeyMode = InsertKeyMode.Default;
            control.IsHandleCreated.Should().BeTrue();
            control.InsertKeyMode.Should().Be(InsertKeyMode.Default);
        }
    }

    [WinFormsTheory]
    [InlineData("", "", true, true)]
    [InlineData("00000", "12345", true, true)]
    [InlineData("00000", "123", false, false)]
    [InlineData("00-00", "12-34", true, true)]
    [InlineData("00-00", "12-", false, false)]
    [InlineData("(000) 000-0000", "(123) 456-7890", true, true)]
    [InlineData("(000) 000-0000", "(123) 456", false, false)]
    public void MaskedTextBox_MaskCompletedAndMaskFull_Get_ReturnsExpected(string mask, string text, bool expectedMaskCompleted, bool expectedMaskFull)
    {
        using MaskedTextBox control = new();
        control.Mask = mask;
        control.Text = text;
        control.MaskCompleted.Should().Be(expectedMaskCompleted);
        control.MaskFull.Should().Be(expectedMaskFull);
    }

    [WinFormsTheory]
    [InlineData(InsertKeyMode.Overwrite, "00000", true)]
    [InlineData(InsertKeyMode.Insert, "00000", false)]
    [InlineData(InsertKeyMode.Default, "00000", false)]
    public void MaskedTextBox_IsOverwriteMode_Get_ReturnsExpected(InsertKeyMode insertMode, string mask, bool expected)
    {
        using MaskedTextBox control = new();
        control.Mask = mask;
        control.InsertKeyMode = insertMode;
        control.IsOverwriteMode.Should().Be(expected);

        // Exception Handling
        if (!Enum.IsDefined(typeof(InsertKeyMode), insertMode))
        {
            control.Invoking(c => c.InsertKeyMode = insertMode)
                   .Should().Throw<InvalidEnumArgumentException>();
        }
    }

    [WinFormsFact]
    public void MaskedTextBox_IsOverwriteModeChangedEvent_AddRemove_Success()
    {
        using MaskedTextBox control = new();
        control.Mask = "00000"; // Add a mask
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(control);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        // Add and remove the event handler and assert the event does not get fired.
        control.IsOverwriteModeChanged += handler;
        control.IsOverwriteModeChanged -= handler;
        control.InsertKeyMode = InsertKeyMode.Insert;
        callCount.Should().Be(0);

        // Add the event handler and assert the event gets fired exactly once.
        control.IsOverwriteModeChanged += handler;
        control.InsertKeyMode = InsertKeyMode.Overwrite;
        callCount.Should().Be(1);

        // Remove the event handler and assert the event does not get fired.
        control.IsOverwriteModeChanged -= handler;
        control.InsertKeyMode = InsertKeyMode.Insert;
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void MaskedTextBox_MaskChangedEvent_AddRemove_Success()
    {
        using MaskedTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(control);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        // Add and remove the event handler and assert the event does not get fired.
        control.MaskChanged += handler;
        control.MaskChanged -= handler;
        control.Mask = "00000";
        callCount.Should().Be(0);

        // Add the event handler and assert the event gets fired exactly once.
        control.MaskChanged += handler;
        control.Mask = "(000) 000-0000";
        callCount.Should().Be(1);

        // Remove the event handler and assert the event does not get fired.
        control.MaskChanged -= handler;
        control.Mask = "00-00";
        callCount.Should().Be(1);
    }
}
