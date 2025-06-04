// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;
using Moq;

namespace System.Windows.Forms.TestUtilities;

public static class CommonTestHelperEx
{
    public static TheoryData<Color, Color> GetBackColorTheoryData()
    {
        return new TheoryData<Color, Color>
        {
            { Color.Red, Color.Red },
            { Color.Empty, Control.DefaultBackColor }
        };
    }

    public static TheoryData<Color, Color> GetForeColorTheoryData()
    {
        return new TheoryData<Color, Color>
        {
            { Color.Red, Color.Red },
            { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3) },
            { Color.White, Color.White },
            { Color.Black, Color.Black },
            { Color.Empty, Control.DefaultForeColor }
        };
    }

    public static TheoryData<Image> GetImageTheoryData()
    {
        TheoryData<Image> data = new()
        {
            new Bitmap(10, 10),
            null
        };
        return data;
    }

    public static TheoryData<Font> GetFontTheoryData()
    {
        TheoryData<Font> data = new()
        {
            SystemFonts.MenuFont,
            null
        };
        return data;
    }

    public static TheoryData<Type> GetTypeWithNullTheoryData()
    {
        TheoryData<Type> data = new()
        {
            null,
            typeof(int)
        };
        return data;
    }

    public static TheoryData<RightToLeft, RightToLeft> GetRightToLeftTheoryData()
    {
        var data = new TheoryData<RightToLeft, RightToLeft>
        {
            { RightToLeft.Inherit, RightToLeft.No },
            { RightToLeft.Yes, RightToLeft.Yes },
            { RightToLeft.No, RightToLeft.No }
        };
        return data;
    }

    public static TheoryData<Padding> GetPaddingTheoryData()
    {
        TheoryData<Padding> data = new()
        {
            default,
            new(1, 2, 3, 4),
            new(1),
            new(-1, -2, -3, -4)
        };
        return data;
    }

    public static TheoryData<Padding, Padding> GetPaddingNormalizedTheoryData()
    {
        var data = new TheoryData<Padding, Padding>
        {
            { default, default },
            { new Padding(1, 2, 3, 4), new Padding(1, 2, 3, 4) },
            { new Padding(1), new Padding(1) },
            { new Padding(-1, -2, -3, -4), Padding.Empty }
        };
        return data;
    }

    public static TheoryData<Cursor> GetCursorTheoryData()
    {
        TheoryData<Cursor> data = new()
        {
            null,
            new(1)
        };
        return data;
    }

    public static TheoryData<PaintEventArgs> GetPaintEventArgsTheoryData()
    {
        Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        return new TheoryData<PaintEventArgs>
        {
            null,
            new(graphics, Rectangle.Empty)
        };
    }

    public static TheoryData<KeyEventArgs> GetKeyEventArgsTheoryData()
    {
        return new TheoryData<KeyEventArgs>
        {
            new(Keys.None),
            new(Keys.Cancel)
        };
    }

    public static TheoryData<KeyPressEventArgs> GetKeyPressEventArgsTheoryData()
    {
        TheoryData<KeyPressEventArgs> data = new()
        {
            null,
            new('1')
        };
        return data;
    }

    public static TheoryData<LayoutEventArgs> GetLayoutEventArgsTheoryData()
    {
        TheoryData<LayoutEventArgs> data = new()
        {
            null,
            new(null, null),
            new(new Control(), "affectedProperty")
        };
        return data;
    }

    public static TheoryData<MouseEventArgs> GetMouseEventArgsTheoryData()
    {
        return new TheoryData<MouseEventArgs>
        {
            null,
            new(MouseButtons.Left, 1, 2, 3, 4),
            new HandledMouseEventArgs(MouseButtons.Left, 1, 2, 3, 4)
        };
    }

    public static TheoryData<IServiceProvider, object> GetEditValueInvalidProviderTestData()
    {
        Mock<IServiceProvider> nullServiceProviderMock = new(MockBehavior.Strict);
        nullServiceProviderMock
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(null);
        Mock<IServiceProvider> invalidServiceProviderMock = new(MockBehavior.Strict);
        invalidServiceProviderMock
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(new object());
        object value = new();
        return new TheoryData<IServiceProvider, object>
        {
            { null, null },
            { null, value },
            { nullServiceProviderMock.Object, null },
            { nullServiceProviderMock.Object, value },
            { invalidServiceProviderMock.Object, null },
            { invalidServiceProviderMock.Object, value }
        };
    }

    public static TheoryData<ITypeDescriptorContext> GetITypeDescriptorContextTestData()
    {
        return new TheoryData<ITypeDescriptorContext>
        {
            null,
            new Mock<ITypeDescriptorContext>(MockBehavior.Strict).Object
        };
    }
}
