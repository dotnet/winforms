// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection;
using System.Windows.Forms.Metafiles;

namespace System.Windows.Forms.Tests;

public class NumericUpDownTests
{
    [WinFormsFact]
    public void NumericUpDown_Constructor()
    {
        using NumericUpDown nud = new();
        Assert.NotNull(nud);
        Assert.Equal("0", nud.Text);
    }

    [WinFormsFact]
    public void NumericUpDown_VisualStyles_off_BasicRendering_ControlEnabled()
    {
        if (Application.RenderWithVisualStyles)
        {
            return;
        }

        using Form form = new();
        using NumericUpDown upDown = new();

        form.Controls.Add(upDown);

        using EmfScope emf = new();
        DeviceContextState state = new(emf);

        Assert.Equal(new Rectangle(0, 0, 120, 23), upDown.Bounds);

        // The rendering here is the "fill" for the background around the child edit control, which
        // doesn't match up to the main control's bounds.
        upDown.PrintToMetafile(emf);
        emf.Validate(
            state,
            Validate.Rectangle(
                new Rectangle(1, 1, 98, 17),
                State.Pen(2, upDown.BackColor, PEN_STYLE.PS_SOLID)));

        // Printing the main control doesn't get the redraw for the child controls on the first render,
        // directly hitting the up/down button subcontrol.
        using EmfScope emfButtons = new();
        state = new DeviceContextState(emfButtons);
        upDown.Controls[0].PrintToMetafile(emfButtons);

        // This is the "fill" line under the up/down arrows
        emfButtons.Validate(
            state,
            Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_STRETCHDIBITS),
            Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_STRETCHDIBITS),
            Validate.LineTo(
                new(0, 18), new(16, 18),
                State.Pen(1, upDown.BackColor, PEN_STYLE.PS_SOLID)));
    }

    [WinFormsFact]
    public void NumericUpDown_VisualStyles_off_BasicRendering_ControlDisabled()
    {
        if (Application.RenderWithVisualStyles)
        {
            return;
        }

        using Form form = new();
        using NumericUpDown upDown = new();

        form.Controls.Add(upDown);

        // Check the disabled state
        upDown.Enabled = false;

        using EmfScope emfDisabled = new();
        DeviceContextState state = new(emfDisabled);
        upDown.PrintToMetafile(emfDisabled);

        emfDisabled.Validate(
            state,
            Validate.Rectangle(
                new Rectangle(0, 0, 99, 18),
                State.Pen(1, upDown.BackColor, PEN_STYLE.PS_SOLID)),
            Validate.Rectangle(
                new Rectangle(1, 1, 97, 16),
                State.Pen(1, SystemColors.Control, PEN_STYLE.PS_SOLID)),
            Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_STRETCHDIBITS),
            Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_STRETCHDIBITS),
            Validate.LineTo(
                new(0, 18), new(16, 18),
                State.Pen(1, upDown.BackColor, PEN_STYLE.PS_SOLID)));
    }

    [WinFormsFact(Skip = "TODO, refer to https://github.com/dotnet/winforms/issues/4212")]
    [ActiveIssue("https://github.com/dotnet/winforms/issues/4212")]
    public void NumericUpDown_VisualStyles_on_BasicRendering()
    {
        if (!Application.RenderWithVisualStyles)
        {
            return;
        }

        using Form form = new();
        using NumericUpDown upDown = new();

        form.Controls.Add(upDown);

        using EmfScope emf = new();
        DeviceContextState state = new(emf);

        Assert.Equal(new Rectangle(0, 0, 120, 23), upDown.Bounds);

        // The rendering here is the "fill" for the background around the child edit control, which
        // doesn't match up to the main control's bounds.
        upDown.PrintToMetafile(emf);

        /*

        [ENHMETAHEADER] Bounds: {0, 0, 119, 22 (LTRB)} Device Size: {Width=3840, Height=2160} Header Size: 108
        [EMRSETBKCOLOR] Color: [R=171, G=173, B=179]
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, 0, 22 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, 0, 0 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 22, 0, 22 (LTRB)} Text: ''
        [EMRSETBKCOLOR] Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMRSETBKCOLOR] Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMRSETBKCOLOR] Color: [R=171, G=173, B=179]
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, 0, 0 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {119, 0, 119, 0 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, 119, 0 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMRSETBKCOLOR] Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMRSETBKCOLOR] Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMRSETBKCOLOR] Color: [R=171, G=173, B=179]
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {119, 0, 119, 22 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {119, 0, 119, 0 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {119, 22, 119, 22 (LTRB)} Text: ''
        [EMRSETBKCOLOR] Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMRSETBKCOLOR] Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMRSETBKCOLOR] Color: [R=171, G=173, B=179]
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 22, 0, 22 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {119, 22, 119, 22 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 22, 119, 22 (LTRB)} Text: ''
        [EMRSETBKCOLOR] Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMRSETBKCOLOR] Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMRCREATEPEN] Index: 1 Style: ENDCAP_ROUND Width: {X=1,Y=0} Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMRSELECTOBJECT] Index: 1
        [EMRSETROP2] Mode: R2_COPYPEN
        [EMRSELECTOBJECT] StockObject: NULL_BRUSH
        [EMRRECTANGLE] RECT: {1, 1, 102, 21 (LTRB)}
        [EMRSELECTOBJECT] StockObject: WHITE_BRUSH
        [EMRSELECTOBJECT] StockObject: BLACK_PEN
        [EMRDELETEOBJECT] Index: 1
        [EMREOF]

         */

        // Printing the main control doesn't get the redraw for the child controls on the first render,
        // directly hitting the up/down button subcontrol.

        using EmfScope emfButtons = new();
        state = new DeviceContextState(emfButtons);
        upDown.Controls[0].PrintToMetafile(emfButtons);

        /*

        [ENHMETAHEADER] Bounds: {0, 0, 16, 20 (LTRB)} Device Size: {Width=3840, Height=2160} Header Size: 108
        [EMRINTERSECTCLIPRECT] RECT: {0, 0, 16, 10 (LTRB)}
        [EMRBITBLT] Bounds: {0, 0, 15, 9 (LTRB)} Destination: {0, 0, 16, 10 (LTRB)} Rop: SRCCOPY Source DC Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {0, 0, 16, 21 (LTRB)} Rects: 1 Rect index 0: {0, 0, 16, 21 (LTRB)}
        [EMRINTERSECTCLIPRECT] RECT: {4, 2, 11, 8 (LTRB)}
        [EMRALPHABLEND]
        [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {0, 0, 16, 21 (LTRB)} Rects: 1 Rect index 0: {0, 0, 16, 21 (LTRB)}
        [EMRINTERSECTCLIPRECT] RECT: {0, 10, 16, 20 (LTRB)}
        [EMRBITBLT] Bounds: {0, 10, 15, 19 (LTRB)} Destination: {0, 10, 16, 20 (LTRB)} Rop: SRCCOPY Source DC Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {0, 0, 16, 21 (LTRB)} Rects: 1 Rect index 0: {0, 0, 16, 21 (LTRB)}
        [EMRINTERSECTCLIPRECT] RECT: {4, 12, 11, 18 (LTRB)}
        [EMRALPHABLEND]
        [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {0, 0, 16, 21 (LTRB)} Rects: 1 Rect index 0: {0, 0, 16, 21 (LTRB)}
        [EMRSETROP2] Mode: R2_COPYPEN
        [EMRSETBKMODE] Mode: BKMODE_TRANSPARENT
        [EMRCREATEPEN] Index: 1 Style: ENDCAP_ROUND Width: {X=1,Y=0} Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMRSELECTOBJECT] Index: 1
        [EMRMOVETOEX] Point: {X=0,Y=20}
        [EMRLINETO] Point: {X=16,Y=20}
        [EMRMOVETOEX] Point: {X=0,Y=0}
        [EMRSELECTOBJECT] StockObject: BLACK_PEN
        [EMRSETBKMODE] Mode: BKMODE_OPAQUE
        [EMRDELETEOBJECT] Index: 1
        [EMREOF]

        */
    }

    [WinFormsFact]
    public void NumericUpDown_Accelerations_Get_ReturnsExpected()
    {
        using NumericUpDown upDown = new();
        NumericUpDownAccelerationCollection accelerations1 = upDown.Accelerations;
        NumericUpDownAccelerationCollection accelerations2 = upDown.Accelerations;

        accelerations1.Should().BeSameAs(accelerations2);

        accelerations1.Should().NotBeNull();
        accelerations1.Should().BeEmpty();
    }

    [WinFormsFact]
    public void NumericUpDown_Accelerations_GetAfterAddingValue_ReturnsExpected()
    {
        using NumericUpDown upDown = new();
        NumericUpDownAccelerationCollection accelerations = upDown.Accelerations;
        accelerations.Add(new NumericUpDownAcceleration(2, 1));
        accelerations.Should().HaveCount(1);
    }

    [WinFormsFact]
    public void NumericUpDown_Accelerations_GetNotNullValue_ReturnsExpected()
    {
        using NumericUpDown upDown = new();
        NumericUpDownAcceleration acceleration = new(2, 1);
        upDown.Accelerations.Add(acceleration);
        NumericUpDownAccelerationCollection accelerations = upDown.Accelerations;
        accelerations.Should().NotBeNull();
        accelerations[0].Should().Be(acceleration);
    }

    [WinFormsFact]
    public void NumericUpDown_Hexadecimal_Get_ReturnsExpected()
    {
        using NumericUpDown upDown = new();
        upDown.Hexadecimal.Should().BeFalse();
    }

    [WinFormsFact]
    public void NumericUpDown_Hexadecimal_Set_GetReturnsExpected()
    {
        using NumericUpDown upDown = new();
        upDown.Hexadecimal = true;
        upDown.Hexadecimal.Should().BeTrue();

        upDown.Hexadecimal = false;
        upDown.Hexadecimal.Should().BeFalse();
    }

    [WinFormsFact]
    public void NumericUpDown_Hexadecimal_SetWithUpdateEditText_CallsUpdateEditText()
    {
        using SubNumericUpDown subUpDown = new();
        subUpDown.Hexadecimal = true;
        subUpDown.Hexadecimal.Should().BeTrue();
        subUpDown.UpdateEditTextCallCount.Should().BeGreaterThan(0);
    }

    [WinFormsFact]
    public void NumericUpDown_Increment_Get_ReturnsExpected()
    {
        using NumericUpDown upDown = new();
        upDown.Increment.Should().Be(1);

        upDown.Increment = 2;
        upDown.Increment.Should().Be(2);

        upDown.Increment = 0;
        upDown.Increment.Should().Be(0);
    }

    [WinFormsFact]
    public void NumericUpDown_Increment_SetNegative_ThrowsArgumentOutOfRangeException()
    {
        using NumericUpDown upDown = new();
        Action act = () => upDown.Increment = -1;
        act.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("value");
    }

    [WinFormsFact]
    public void NumericUpDown_Increment_Set_GetReturnsExpected()
    {
        using NumericUpDown upDown = new();
        upDown.Increment = 2;
        upDown.Increment.Should().Be(2);

        upDown.Increment = 0;
        upDown.Increment.Should().Be(0);
    }

    [WinFormsFact]
    public void NumericUpDown_Maximum_SetLessThanMinimum_SetsMinimumToMaximum()
    {
        using NumericUpDown upDown = new();
        decimal minimumValue = 100m;
        decimal maximumValue = 50m;
        upDown.Minimum = minimumValue;

        upDown.Maximum = maximumValue;

        upDown.Maximum.Should().Be(maximumValue);
        upDown.Minimum.Should().Be(maximumValue);
    }

    [WinFormsFact]
    public void NumericUpDown_Maximum_SetGreaterThanMinimum_KeepsMinimumUnchanged()
    {
        using NumericUpDown upDown = new();
        decimal minimumValue = 50m;
        decimal maximumValue = 100m;
        upDown.Minimum = minimumValue;

        upDown.Maximum = maximumValue;

        upDown.Maximum.Should().Be(maximumValue);
        upDown.Minimum.Should().Be(minimumValue);
    }

    [WinFormsFact]
    public void NumericUpDown_Minimum_SetGreaterThanMaximum_SetsMaximumToMinimum()
    {
        using NumericUpDown upDown = new();
        upDown.Maximum = 10;
        upDown.Minimum = 20;
        upDown.Maximum.Should().Be(20);
        upDown.Minimum.Should().Be(20);
    }

    [WinFormsFact]
    public void NumericUpDown_Minimum_SetLessThanMaximum_KeepsMaximumUnchanged()
    {
        using NumericUpDown upDown = new();
        upDown.Maximum = 30;
        upDown.Minimum = 10;
        upDown.Maximum.Should().Be(30);
        upDown.Minimum.Should().Be(10);
    }

    [WinFormsFact]
    public void NumericUpDown_Padding_Set_GetReturnsExpected()
    {
        using NumericUpDown upDown = new();
        Padding padding = new(1, 2, 3, 4);
        upDown.Padding = padding;
        upDown.Padding.Should().Be(padding);
    }

    [WinFormsFact]
    public void NumericUpDown_PaddingChangedEvent_AddRemove_Success()
    {
        using NumericUpDown upDown = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().BeSameAs(upDown);
            e.Should().BeSameAs(EventArgs.Empty);
            callCount++;
        };

        upDown.PaddingChanged += handler;
        upDown.Padding = new Padding(1);
        callCount.Should().Be(1);
        upDown.Padding.Should().Be(new Padding(1));

        upDown.PaddingChanged -= handler;
        upDown.Padding = new Padding(2);
        callCount.Should().Be(1);
        upDown.Padding.Should().Be(new Padding(2));
    }

    [WinFormsFact]
    public void NumericUpDown_PaddingChangedEvent_UnsubscribeInHandler_Success()
    {
        using NumericUpDown upDown = new();
        int callCount = 0;
        EventHandler handler = null;
        handler = (sender, e) =>
        {
            sender.Should().BeSameAs(upDown);
            e.Should().BeSameAs(EventArgs.Empty);
            callCount++;
            upDown.PaddingChanged -= handler;
        };

        upDown.PaddingChanged += handler;
        upDown.Padding = new Padding(1);
        callCount.Should().Be(1);
        upDown.Padding.Should().Be(new Padding(1));

        upDown.Padding = new Padding(2);
        callCount.Should().Be(1);
        upDown.Padding.Should().Be(new Padding(2));
    }

    [WinFormsFact]
    public void NumericUpDown_TextChanged_AddRemove_Success()
    {
        using NumericUpDown upDown = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            callCount++;
            sender.Should().Be(upDown);
            e.Should().Be(EventArgs.Empty);
        };

        upDown.TextChanged += handler;
        upDown.Text = "1";
        callCount.Should().Be(1);
        upDown.TextChanged -= handler;
        upDown.Text = "2";
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void NumericUpDown_TextChanged_UnsubscribeInHandler_Success()
    {
        using NumericUpDown upDown = new();
        int callCount = 0;
        EventHandler handler = null;
        handler = (sender, e) =>
        {
            callCount++;
            sender.Should().Be(upDown);
            e.Should().Be(EventArgs.Empty);
            upDown.TextChanged -= handler;
        };

        upDown.TextChanged += handler;
        upDown.Text = "1";
        callCount.Should().Be(1);
        upDown.Text = "2";
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void NumericUpDown_ThousandsSeparator_Get_ReturnsExpected()
    {
        using SubNumericUpDown subUpDown = new();
        subUpDown.ThousandsSeparator.Should().BeFalse();
        subUpDown.UpdateEditTextCallCount.Should().Be(1);
    }

    [WinFormsFact]
    public void NumericUpDown_ThousandsSeparator_Set_GetReturnsExpected()
    {
        using SubNumericUpDown subUpDown = new();
        subUpDown.ThousandsSeparator = true;
        subUpDown.ThousandsSeparator.Should().BeTrue();
        subUpDown.UpdateEditTextCallCount.Should().Be(2);

        subUpDown.ThousandsSeparator = true;
        subUpDown.ThousandsSeparator.Should().BeTrue();
        subUpDown.UpdateEditTextCallCount.Should().Be(3);

        subUpDown.ThousandsSeparator = false;
        subUpDown.ThousandsSeparator.Should().BeFalse();
        subUpDown.UpdateEditTextCallCount.Should().Be(4);
    }

    [WinFormsFact]
    public void NumericUpDown_ThousandsSeparator_SetWithHandler_CallsUpdateEditText()
    {
        using SubNumericUpDown subUpDown = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(subUpDown);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        subUpDown.ThousandsSeparator = true;
        subUpDown.ThousandsSeparator.Should().BeTrue();
        subUpDown.UpdateEditTextCallCount.Should().Be(2);
        callCount.Should().Be(0);

        subUpDown.ThousandsSeparator = true;
        subUpDown.ThousandsSeparator.Should().BeTrue();
        subUpDown.UpdateEditTextCallCount.Should().Be(3);
        callCount.Should().Be(0);

        subUpDown.ThousandsSeparator = false;
        subUpDown.ThousandsSeparator.Should().BeFalse();
        subUpDown.UpdateEditTextCallCount.Should().Be(4);
        callCount.Should().Be(0);
    }

    [WinFormsFact]
    public void NumericUpDown_UserEdit_True_CallsValidateEditText()
    {
        using SubNumericUpDown subUpDown = new();
        subUpDown.PublicUserEdit = true;
        subUpDown.Value = 10;
        subUpDown.ResetUpdateEditTextCallCount();
        subUpDown.CallUpdateEditText();
        subUpDown.UpdateEditTextCallCount.Should().Be(1);
    }

    [WinFormsFact]
    public void NumericUpDown_UserEdit_False_DoesNotCallValidateEditText()
    {
        using SubNumericUpDown subUpDown = new();
        subUpDown.PublicUserEdit = false;
        subUpDown.Value = 10;
        subUpDown.ResetUpdateEditTextCallCount();
        subUpDown.UpdateEditTextCallCount.Should().Be(0);
    }

    [WinFormsFact]
    public void NumericUpDown_UserEdit_True_ThenFalse_DoesNotCallValidateEditText()
    {
        using SubNumericUpDown subUpDown = new();
        subUpDown.PublicUserEdit = false;
        subUpDown.Value = 10;
        subUpDown.ResetUpdateEditTextCallCount();
        subUpDown.UpdateEditTextCallCount.Should().Be(0);
    }

    [WinFormsFact]
    public void NumericUpDown_DefaultValues()
    {
        using NumericUpDown upDown = new();
        upDown.Value.Should().Be(0);
        upDown.Minimum.Should().Be(0);
        upDown.Maximum.Should().Be(100);
        upDown.Increment.Should().Be(1);
        upDown.DecimalPlaces.Should().Be(0);
        upDown.Hexadecimal.Should().BeFalse();
        upDown.ThousandsSeparator.Should().BeFalse();
    }

    [WinFormsFact]
    public void NumericUpDown_SetValue()
    {
        using NumericUpDown upDown = new();
        upDown.Value = 50;
        upDown.Value.Should().Be(50);
    }

    [WinFormsFact]
    public void NumericUpDown_SetMinimum()
    {
        using NumericUpDown upDown = new();
        upDown.Minimum = 10;
        upDown.Minimum.Should().Be(10);
    }

    [WinFormsFact]
    public void NumericUpDown_SetMaximum()
    {
        using NumericUpDown upDown = new();
        upDown.Maximum = 200;
        upDown.Maximum.Should().Be(200);
    }

    [WinFormsFact]
    public void NumericUpDown_SetIncrement()
    {
        using NumericUpDown upDown = new();
        upDown.Increment = 5;
        upDown.Increment.Should().Be(5);
    }

    [WinFormsFact]
    public void NumericUpDown_SetDecimalPlaces()
    {
        using NumericUpDown upDown = new();
        upDown.DecimalPlaces = 2;
        upDown.DecimalPlaces.Should().Be(2);
    }

    [WinFormsFact]
    public void NumericUpDown_SetHexadecimal()
    {
        using NumericUpDown upDown = new();
        upDown.Hexadecimal = true;
        upDown.Hexadecimal.Should().BeTrue();
    }

    [WinFormsFact]
    public void NumericUpDown_SetThousandsSeparator()
    {
        using NumericUpDown upDown = new();
        upDown.ThousandsSeparator = true;
        upDown.ThousandsSeparator.Should().BeTrue();
    }

    [WinFormsFact]
    public void NumericUpDown_OnValueChanged_AddRemove_Success()
    {
        using SubNumericUpDown subUpDown = new();
        bool eventWasRaised = false;

        EventHandler handler = (sender, e) =>
        {
            eventWasRaised = true;
        };

        subUpDown.ValueChanged += handler;
        subUpDown.Value = 10;
        eventWasRaised.Should().BeTrue("because the ValueChanged event should be raised when the value changes");

        eventWasRaised = false;
        subUpDown.ValueChanged -= handler;
        subUpDown.Value = 20;
        eventWasRaised.Should().BeFalse("because the ValueChanged event should not be raised after the handler is removed");
    }

    [WinFormsFact]
    public void NumericUpDown_BeginInit_SetsInitializingToTrue()
    {
        using SubNumericUpDown subUpDown = new();
        subUpDown.IsInitializing.Should().BeFalse();
        subUpDown.BeginInit();
        subUpDown.IsInitializing.Should().BeTrue();
    }

    [WinFormsFact]
    public void DownButton_ValueAtMinimum_DoesNotChangeValue()
    {
        using NumericUpDown upDown = new() { Value = 0, Minimum = 0, Increment = 1 };
        upDown.DownButton();
        upDown.Value.Should().Be(0);
    }

    [WinFormsFact]
    public void DownButton_NonNumericText_IgnoresText()
    {
        using NumericUpDown upDown = new() { Value = 10, Increment = 1, Text = "Not a number" };
        upDown.DownButton();
        upDown.Value.Should().Be(9);
    }

    [WinFormsFact]
    public void DownButton_TextOutsideRange_ParsesText()
    {
        using NumericUpDown upDown = new() { Value = 10, Minimum = 0, Maximum = 20, Increment = 1, Text = "30" };
        upDown.DownButton();
        upDown.Value.Should().Be(19);
    }

    [WinFormsFact]
    public void NumericUpDown_DownButton_ValueEqualToMinimum_ValueRemainsSame()
    {
        using NumericUpDown upDown = new()
        {
            Minimum = 10,
            Value = 10
        };

        upDown.DownButton();

        upDown.Value.Should().Be(10);
    }

    [WinFormsFact]
    public void NumericUpDown_DownButton_ValueLessThanMinimum_SetsValueToMinimum()
    {
        using SubNumericUpDown subUpDown = new()
        {
            Minimum = 10,
            Value = 15,
            Increment = 10
        };

        subUpDown.DownButton();

        subUpDown.Value.Should().Be(subUpDown.Minimum);
    }

    [WinFormsFact]
    public void NumericUpDown_DownButton_ValueGreaterThanMinimum_DecrementsValue()
    {
        using NumericUpDown upDown = new()
        {
            Minimum = 10,
            Value = 11
        };

        upDown.DownButton();

        upDown.Value.Should().Be(10);
    }

    [WinFormsFact]
    public void NumericUpDown_DownButton_SpinningTrue_StopsAcceleration()
    {
        using SubNumericUpDown upDown = new()
        {
            Minimum = 10,
            Value = 11,
            PublicUserEdit = true
        };

        upDown.DownButton();

        upDown.PublicUserEdit.Should().BeFalse();
    }

    [WinFormsFact]
    public void NumericUpDown_ToString_ReturnsExpected()
    {
        using NumericUpDown upDown = new();
        upDown.Minimum = 10;
        upDown.Maximum = 100;
        string s = upDown.ToString();
        s.Should().Contain("Minimum = 10");
        s.Should().Contain("Maximum = 100");
    }

    [WinFormsFact]
    public void UpButton_ValueAtMaximum_DoesNotChangeValue()
    {
        using NumericUpDown upDown = new() { Value = 100, Maximum = 100, Increment = 1 };
        upDown.UpButton();
        upDown.Value.Should().Be(100);
    }

    [WinFormsFact]
    public void UpButton_NonNumericText_IgnoresText()
    {
        using NumericUpDown upDown = new() { Value = 10, Increment = 1, Text = "Not a number" };
        upDown.UpButton();
        upDown.Value.Should().Be(11);
    }

    [WinFormsFact]
    public void UpButton_TextOutsideRange_ParsesText()
    {
        using NumericUpDown upDown = new() { Value = 10, Minimum = 0, Maximum = 20, Increment = 1, Text = "30" };
        upDown.UpButton();
        upDown.Value.Should().Be(20);
    }

    [WinFormsFact]
    public void UpButton_ValueEqualToMaximum_ValueRemainsSame()
    {
        using NumericUpDown upDown = new() { Value = 100, Maximum = 100 };
        upDown.UpButton();
        upDown.Value.Should().Be(100);
    }

    [WinFormsFact]
    public void UpButton_ValueLessThanMaximum_IncrementsValue()
    {
        using NumericUpDown upDown = new() { Value = 99, Maximum = 100 };
        upDown.UpButton();
        upDown.Value.Should().Be(100);
    }

    [WinFormsFact]
    public void UpButton_ValueGreaterThanMaximum_SetsValueToMaximum()
    {
        using NumericUpDown upDown = new() { Value = 100, Maximum = 100 };
        upDown.UpButton();
        upDown.Value.Should().Be(100);
    }

    [WinFormsFact]
    public void UpButton_SpinningTrue_StopsAcceleration()
    {
        using SubNumericUpDown upDown = new() { Value = 100, Maximum = 100, PublicUserEdit = true };
        upDown.UpButton();
        upDown.PublicUserEdit.Should().BeFalse();
    }

    [WinFormsFact]
    public void UpButton_ValueCausesOverflow_SetsValueToMaximum()
    {
        using NumericUpDown upDown = new();
        upDown.Maximum = decimal.MaxValue;
        upDown.Value = decimal.MaxValue;
        upDown.UpButton();
        upDown.Value.Should().Be(decimal.MaxValue);
    }

    [WinFormsFact]
    public void UpButton_IncrementCausesOverflow_SetsValueToMaximum()
    {
        using NumericUpDown upDown = new();
        upDown.Maximum = decimal.MaxValue;
        upDown.Value = decimal.MaxValue - 1;
        upDown.Increment = 2;
        upDown.UpButton();
        upDown.Value.Should().Be(decimal.MaxValue);
    }

    private class SubNumericUpDown : NumericUpDown
    {
        public int UpdateEditTextCallCount { get; private set; }

        public bool PublicUserEdit
        {
            get { return UserEdit; }
            set { UserEdit = value; }
        }

        public bool IsInitializing
        {
            get
            {
                var initializing = typeof(NumericUpDown).GetField("_initializing", BindingFlags.NonPublic | BindingFlags.Instance);
                return (bool)initializing.GetValue(this);
            }
        }

        protected override void UpdateEditText()
        {
            UpdateEditTextCallCount++;
            base.UpdateEditText();
        }

        public void ResetUpdateEditTextCallCount()
        {
            UpdateEditTextCallCount = 0;
        }

        public void CallUpdateEditText()
        {
            UpdateEditText();
        }
    }
}
