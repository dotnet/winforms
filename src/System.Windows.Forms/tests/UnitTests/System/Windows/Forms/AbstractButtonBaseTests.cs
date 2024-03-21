// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public abstract class AbstractButtonBaseTests
{
    protected abstract ButtonBase CreateButton();

    protected void ButtonBase_FlatStyle_ValidFlatButtonBorder(int borderSize)
    {
        using var control = CreateButton();
        control.FlatStyle = FlatStyle.Flat;

        control.Invoking(y => y.FlatAppearance.BorderColor = Color.Transparent)
            .Should().Throw<NotSupportedException>();

        if (borderSize < 0)
        {
            control.Invoking(y => y.FlatAppearance.BorderSize = borderSize)
                .Should().Throw<ArgumentOutOfRangeException>();
        }
        else
        {
            control.FlatAppearance.BorderSize = borderSize;
            control.FlatAppearance.BorderSize.Should().Be(borderSize);
        }
    }

    protected void ButtonBase_FlatStyle_ProperFlatButtonColor(int red, int green, int blue)
    {
        Color expectedColor = Color.FromArgb(red, green, blue);

        using var control = CreateButton();
        control.FlatStyle = FlatStyle.Flat;
        control.BackColor = expectedColor;

        control.FlatAppearance.CheckedBackColor = expectedColor;
        control.FlatAppearance.BorderColor = expectedColor;

        control.BackColor.Should().Be(expectedColor);
        control.FlatAppearance.BorderColor.Should().Be(expectedColor);
        control.FlatAppearance.CheckedBackColor.Should().Be(expectedColor);
    }

    protected virtual void ButtonBase_OverChangeRectangle_Get(Appearance appearance, FlatStyle flatStyle)
    {
        using dynamic control = CreateButton();

        if (control is null)
        {
            return;
        }

        control.Appearance = appearance;
        control.FlatStyle = flatStyle;

        Rectangle overChangeRectangle;

        // ButtonBase.Adapter prohibits this
        if (appearance == Appearance.Normal
            && (flatStyle != FlatStyle.Standard
                && flatStyle != FlatStyle.Popup
                && flatStyle != FlatStyle.Flat))
        {
            // Compiler requires casting lambda expression to delegate or expression
            // before using it as a dynamically dispatched operation
            Action act = () => overChangeRectangle = control.OverChangeRectangle;
            act.Should().Throw<Exception>();

            return;
        }

        overChangeRectangle = control.OverChangeRectangle;

        if (control.FlatStyle == FlatStyle.Standard)
        {
            overChangeRectangle.Should().Be(new Rectangle(-1, -1, 1, 1));
        }

        if (control.Appearance == Appearance.Button)
        {
            if (control.FlatStyle != FlatStyle.Standard)
            {
                overChangeRectangle.Should().Be(control.ClientRectangle);
            }
        }
        else if (control.FlatStyle != FlatStyle.Standard)
        {
            overChangeRectangle.Should().Be(control.Adapter.CommonLayout().Layout().CheckBounds);
        }
    }
}
