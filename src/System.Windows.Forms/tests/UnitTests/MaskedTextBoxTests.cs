// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;

namespace System.Windows.Forms.Tests;

public class MaskedTextBoxTests : IDisposable
{
    private readonly MaskedTextBox _maskedTextBox;

    public MaskedTextBoxTests()
    {
        _maskedTextBox = new();
    }

    public void Dispose()
    {
        _maskedTextBox.Dispose();
    }

    [WinFormsFact]
    public void MaskedTextBox_Constructor()
    {
        _maskedTextBox.Should().NotBeNull();

        // Check default values
        _maskedTextBox.BeepOnError.Should().BeFalse();
        _maskedTextBox.AsciiOnly.Should().BeFalse();
        _maskedTextBox.Culture.Should().Be(CultureInfo.CurrentCulture);
        _maskedTextBox.AcceptsTab.Should().BeFalse();
        _maskedTextBox.CanUndo.Should().BeFalse();
        _maskedTextBox.WordWrap.Should().BeFalse();
        _maskedTextBox.Multiline.Should().BeFalse();
        _maskedTextBox.ResetOnSpace.Should().BeTrue();
        _maskedTextBox.SkipLiterals.Should().BeTrue();
        _maskedTextBox.ValidatingType.Should().BeNull();
        _maskedTextBox.TextAlign.Should().Be(HorizontalAlignment.Left);
        _maskedTextBox.FormatProvider.Should().Be(null);
        _maskedTextBox.ResetOnPrompt.Should().BeTrue();
        _maskedTextBox.ValidatingType.Should().BeNull();
        _maskedTextBox.PromptChar.Should().Be('_');
        _maskedTextBox.GetLineFromCharIndex(2).Should().Be(0);
        _maskedTextBox.GetFirstCharIndexFromLine(100).Should().Be(0);
        _maskedTextBox.GetFirstCharIndexOfCurrentLine().Should().Be(0);
        _maskedTextBox.PasswordChar.Should().Be('\0');
        _maskedTextBox.RejectInputOnFirstFailure.Should().BeFalse();
        _maskedTextBox.Text.Should().Be(string.Empty);
        _maskedTextBox.UseSystemPasswordChar.Should().BeFalse();

        _maskedTextBox.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsFact]
    public void MaskedTextBox_ConstructorString()
    {
        using MaskedTextBox maskedTextBox = new("Hello World!");

        maskedTextBox.Should().NotBeNull();
    }

    [WinFormsFact]
    public void MaskedTextBox_ConstructorMaskedTextProvider()
    {
        using MaskedTextBox maskedTextBox = new(new MaskedTextProvider("Hello World!"));

        maskedTextBox.Should().NotBeNull();
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
        control.BeepOnError.Should().Be(!value);
        ;
        control.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsTheory]
    [BoolData]
    public void MaskedTextBox_BeepOnError_SetWithHandle_GetReturnsExpected(bool value)
    {
        using MaskedTextBox control = new();
        control.Handle.Should().NotBe(IntPtr.Zero);

        // These events are tested to ensure that they are not accidentally triggered
        // when the BeepOnError property is set.
        // The Invalidated event is triggered when the control or part of it needs to be redrawn.
        // If the setting of BeepOnError causes the control to be redrawn,
        // it may trigger unnecessary performance overhead.
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;

        // The StyleChanged event is triggered when the style of the control has changed.
        // If the setting of BeepOnError causes a style change, it may affect the appearance or behavior of the control.
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;

        // The HandleCreated event is triggered when a handle to the control is created.
        // If the setting of BeepOnError causes a handle to be created, it may affect the
        // control's lifecycle or behavior.
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

    [WinFormsTheory]
    [InlineData("en-US")]
    [InlineData("fr-FR")]
    [InlineData("es-ES")]
    public void MaskedTextBox_FormatProvider_Set_GetReturnsExpected(string cultureName)
    {
        using MaskedTextBox control = new();
        CultureInfo culture = new(cultureName);
        control.FormatProvider = culture;
        control.FormatProvider.Should().Be(culture);
        control.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsFact]
    public void MaskedTextBox_Lines_Get_ReturnsExpected()
    {
        using MaskedTextBox control = new();
        control.Text = "Line1\nLine2\nLine3";
        control.Lines.Should().Equal(["Line1", "Line2", "Line3"]);
    }

    [WinFormsFact]
    public void MaskedTextBox_MaskedTextProvider_Get_ReturnsExpected()
    {
        using MaskedTextBox control = new();
        control.Mask = "000-000";
        MaskedTextProvider provider = control.MaskedTextProvider;
        provider.Should().NotBeNull();
        provider.Mask.Should().Be("000-000");
    }

    [WinFormsFact]
    public void MaskedTextBox_MaskInputRejectedEvent_AddRemove_Success()
    {
        using MaskedTextBox control = new();
        int callCount = 0;
        MaskInputRejectedEventHandler handler = (sender, e) =>
        {
            sender.Should().Be(control);
            e.Should().NotBeNull();
            callCount++;
        };

        control.MaskInputRejected += handler;
        control.MaskInputRejected -= handler;
        callCount.Should().Be(0);

        control.MaskInputRejected += handler;
        control.Mask = "000-000";

        // Test rejected input
        control.Text = "1234567";
        callCount.Should().Be(1);

        // Test accepted input
        control.Text = "123-45";
        callCount.Should().Be(1);

        control.MaskInputRejected -= handler;
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void MaskedTextBox_MaxLength_GetSet_ReturnsExpected()
    {
        using MaskedTextBox control = new();
        int originalMaxLength = control.MaxLength;

        control.MaxLength = 500;

        control.MaxLength.Should().Be(originalMaxLength);
    }

    [WinFormsFact]
    public void MaskedTextBox_ReadOnly_GetSet_ReturnsExpected()
    {
        using MaskedTextBox control = new();
        bool originalValue = control.ReadOnly;

        control.ReadOnly = !originalValue;
        control.ReadOnly.Should().Be(!originalValue);

        control.ReadOnly = originalValue;
        control.ReadOnly.Should().Be(originalValue);

        control.ReadOnly = originalValue;
        control.ReadOnly.Should().Be(originalValue);

        control.Modified.Should().BeFalse();
        control.SelectionStart.Should().Be(0);
        control.SelectionLength.Should().Be(0);
    }

    [WinFormsTheory]
    [BoolData]
    public void MaskedTextBox_ResetOnSpace_GetSet_ReturnsExpected(bool value)
    {
        using MaskedTextBox control = new();

        control.ResetOnSpace = value;
        control.ResetOnSpace.Should().Be(value);
        control.IsHandleCreated.Should().BeFalse();

        control.ResetOnSpace = value;
        control.ResetOnSpace.Should().Be(value);
        control.IsHandleCreated.Should().BeFalse();

        control.ResetOnSpace = !value;
        control.ResetOnSpace.Should().Be(!value);
        control.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsTheory]
    [BoolData]
    public void MaskedTextBox_SkipLiterals_GetSet_ReturnsExpected(bool value)
    {
        using MaskedTextBox control = new();
        control.SkipLiterals = value;
        control.SkipLiterals.Should().Be(value);
        control.IsHandleCreated.Should().BeFalse();

        // Set same.
        control.SkipLiterals = value;
        control.SkipLiterals.Should().Be(value);
        control.IsHandleCreated.Should().BeFalse();

        // Set different.
        control.SkipLiterals = !value;
        control.SkipLiterals.Should().Be(!value);
        control.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsTheory]
    [InlineData("", "")]
    [InlineData("12345", "12345")]
    [InlineData("12345", "123")]
    public void MaskedTextBox_SelectedText_GetSet_ReturnsExpected(string mask, string value)
    {
        using MaskedTextBox control = new()
        {
            Mask = mask,
        };

        control.CreateControl();
        control.Text = value;
        control.SelectionStart = 0;
        control.SelectionLength = value.Length;
        string selectedText = control.SelectedText;

        selectedText.Should().Be(value);
    }

    [WinFormsTheory]
    [InlineData(HorizontalAlignment.Left)]
    [InlineData(HorizontalAlignment.Center)]
    [InlineData(HorizontalAlignment.Right)]
    public void MaskedTextBox_TextAlign_Set_GetReturnsExpected(HorizontalAlignment alignment)
    {
        using MaskedTextBox control = new();
        control.CreateControl();
        nint handleBefore = control.Handle;
        HorizontalAlignment originalAlignment = control.TextAlign;

        control.TextAlign = alignment;
        HorizontalAlignment actualAlignment = control.TextAlign;
        nint handleAfter = control.Handle;

        actualAlignment.Should().Be(alignment);

        // If the alignment is changed, the handle should be recreated

        if (alignment != originalAlignment)
        {
            bool expectedHandleChanged = alignment != originalAlignment && !handleBefore.Equals(handleAfter);
            control.IsHandleCreated.Should().Be(expectedHandleChanged);
        }
    }

    [WinFormsFact]
    public void MaskedTextBox_TextAlignChangedEvent_AddRemove_Success()
    {
        using MaskedTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(control);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        control.TextAlignChanged += handler;
        control.TextAlign = HorizontalAlignment.Center;
        callCount.Should().Be(1);

        control.TextAlignChanged -= handler;
        control.TextAlign = HorizontalAlignment.Right;
        callCount.Should().Be(1);
    }

    [WinFormsTheory]
    [BoolData]
    public void MaskedTextBox_BooleanProperty_GetSet_ReturnsExpected(bool value)
    {
        TestProperty(_maskedTextBox, nameof(MaskedTextBox.RejectInputOnFirstFailure), value);
        TestProperty(_maskedTextBox, nameof(MaskedTextBox.ResetOnPrompt), value);
    }

    private void TestProperty(MaskedTextBox maskedTextBox, string propertyName, bool value)
    {
        PropertyInfo property = typeof(MaskedTextBox).GetProperty(propertyName);

        property.SetValue(maskedTextBox, value);
        property.GetValue(maskedTextBox).Should().Be(value);
        maskedTextBox.IsHandleCreated.Should().BeFalse();

        property.SetValue(maskedTextBox, !value);
        property.GetValue(maskedTextBox).Should().Be(!value);
        maskedTextBox.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsFact]
    public void MaskedTextBox_GetCharAndPosition_ReturnsExpected()
    {
        _maskedTextBox.Text = "Hello, World!";
        _maskedTextBox.CreateControl();

        // Test GetPositionFromCharIndex
        Point position = _maskedTextBox.GetPositionFromCharIndex(4);
        position.Should().NotBe(Point.Empty);

        // Test GetCharFromPosition
        char resultChar = _maskedTextBox.GetCharFromPosition(position);
        resultChar.Should().Be('o');

        // Test GetCharIndexFromPosition
        int resultIndex = _maskedTextBox.GetCharIndexFromPosition(position);
        resultIndex.Should().Be(4);
    }

    [WinFormsTheory]
    [InlineData(typeof(int), "12345")]
    [InlineData(typeof(string), "Hello")]
    public void MaskedTextBox_ValidatingTypeAndValidateText_ReturnsExpected(Type validatingType, string text)
    {
        int callCount = 0;
        TypeValidationEventHandler handler = (sender, e) =>
        {
            sender.Should().Be(_maskedTextBox);
            e.Should().NotBeNull();
            callCount++;
        };

        _maskedTextBox.ValidatingType = validatingType;
        _maskedTextBox.ValidatingType.Should().Be(validatingType);
        _maskedTextBox.IsHandleCreated.Should().BeFalse();

        _maskedTextBox.Text = text;
        _maskedTextBox.CreateControl();

        _maskedTextBox.TypeValidationCompleted += handler;
        object result = _maskedTextBox.ValidateText();
        result.Should().NotBeNull();
        callCount.Should().Be(1);

        _maskedTextBox.TypeValidationCompleted -= handler;
        _maskedTextBox.ValidateText();
        callCount.Should().Be(1);
    }

    [WinFormsTheory]
    [InlineData('A', 'B', true)]
    [InlineData('1', '2', true)]
    [InlineData('%', '&', true)]
    [InlineData('A', 'B', false)]
    [InlineData('1', '2', false)]
    [InlineData('%', '&', false)]
    public void MaskedTextBox_CharProperties_Set_GetReturnsExpected(char originalValue, char value, bool isPasswordChar)
    {
        if (isPasswordChar)
        {
            _maskedTextBox.PasswordChar = originalValue;
            _maskedTextBox.PasswordChar.Should().Be(originalValue);

            _maskedTextBox.PasswordChar = value;
            _maskedTextBox.PasswordChar.Should().Be(value);
        }
        else
        {
            _maskedTextBox.PromptChar = originalValue;
            _maskedTextBox.PromptChar.Should().Be(originalValue);

            _maskedTextBox.PromptChar = value;
            _maskedTextBox.PromptChar.Should().Be(value);
        }
    }

    [WinFormsFact]
    public void MaskedTextBox_PromptCharAndPasswordChar_SetInvalid_ThrowsArgumentException()
    {
        Action act = () => _maskedTextBox.PromptChar = '\0';
        act.Should().Throw<ArgumentException>();

        Action act2 = () => _maskedTextBox.PasswordChar = '\t';
        act2.Should().Throw<ArgumentException>();
    }

    [WinFormsFact]
    public void MaskedTextBox_PasswordChar_SetSameAsPromptChar_ThrowsInvalidOperationException()
    {
        _maskedTextBox.PromptChar = 'A';
        Action act = () => _maskedTextBox.PasswordChar = 'A';
        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void MaskedTextBox_PasswordChar_SetWithUseSystemPasswordChar_UpdatesMaskedTextProvider()
    {
        _maskedTextBox.Mask = "00000";
        _maskedTextBox.UseSystemPasswordChar = true;
        _maskedTextBox.PasswordChar = 'A';

        if (_maskedTextBox.MaskedTextProvider is not null)
        {
            _maskedTextBox.PasswordChar.Should().Be('●');
            _maskedTextBox.MaskedTextProvider.PasswordChar.Should().Be('●');
        }
    }
}
