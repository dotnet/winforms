// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Security;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Moq;
using Xunit;

namespace WinForms.Common.Tests
{
    public static class CommonTestHelper
    {
        // helper method to generate theory data from all values of an enum type
        internal static TheoryData<T> GetEnumTheoryData<T>() where T : Enum
        {
            var data = new TheoryData<T>();
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                data.Add(item);
            }

            return data;
        }

        public static TheoryData GetEnumTypeTheoryData(Type enumType)
        {
            var data = new TheoryData<Enum>();
            foreach (Enum item in Enum.GetValues(enumType))
            {
                data.Add(item);
            }
            return data;
        }

        // helper method to generate invalid theory data for an enum type
        // This method assumes that int.MinValue and int.MaxValue are not in the enum
        internal static TheoryData<T> GetEnumTheoryDataInvalid<T>() where T : Enum
        {
            var data = new TheoryData<T>
            {
                // This boxing is necessary because you can't cast an int to a generic,
                // even if the generic is guaranteed to be an enum
                (T)(object)int.MinValue,
                (T)(object)int.MaxValue
            };
            return data;
        }

        public static TheoryData<Enum> GetEnumTypeTheoryDataInvalid(Type enumType)
        {
            var data = new TheoryData<Enum>();
            IEnumerable<Enum> values = Enum.GetValues(enumType).Cast<Enum>().OrderBy(p => p);

            // Assumes that the enum is sequential.
            data.Add((Enum)Enum.ToObject(enumType, Convert.ToInt32(values.Min()) - 1));
            data.Add((Enum)Enum.ToObject(enumType, Convert.ToInt32(values.Max()) + 1));
            return data;
        }

        #region Primitives

        // helper method to generate theory data for all values of a boolean
        public static TheoryData<bool> GetBoolTheoryData()
        {
            var data = new TheoryData<bool>
            {
                true,
                false
            };
            return data;
        }

        // helper method to generate theory data for some values of a int
        public static TheoryData<int> GetIntTheoryData()
        {
            var data = new TheoryData<int>
            {
                int.MinValue,
                int.MaxValue,
                0,
                1,
                -1,
                int.MaxValue / 2
            };
            return data;
        }

        public static TheoryData<int> GetNonNegativeIntTheoryData()
        {
            var data = new TheoryData<int>
            {
                int.MaxValue,
                0,
                1,
                int.MaxValue / 2
            };
            return data;
        }

        // helper method to generate theory data for some values of a int
        internal static TheoryData<uint> GetUIntTheoryData()
        {
            var data = new TheoryData<uint>
            {
                int.MaxValue,
                0,
                1,
                int.MaxValue / 2
            };
            return data;
        }

        // helper method to generate theory data for some values of a int
        internal static TheoryData<int> GetNIntTheoryData()
        {
            var data = new TheoryData<int>
            {
                int.MinValue,
                -1,
                int.MinValue / 2
            };
            return data;
        }

        // helper method to generate theory data for some values of a int
        internal static TheoryData<float> GetFloatTheoryData()
        {
            var data = new TheoryData<float>
            {
                float.MaxValue,
                float.MinValue,
                float.Epsilon,
                float.Epsilon * -1,
                float.NegativeInfinity, // not sure about these two
                float.PositiveInfinity, // 2
                0,
                -1,
                1,
                float.MaxValue / 2
            };
            return data;
        }

        // helper method to generate theory data for some values of a int
        internal static TheoryData<float> GetUFloatTheoryData()
        {
            var data = new TheoryData<float>
            {
                float.MaxValue,
                float.Epsilon,
                float.PositiveInfinity, // not sure about this one
                0,
                1,
                float.MaxValue / 2
            };
            return data;
        }

        // helper method to generate theory data for a span of string values
        private const string reasonable = nameof(reasonable);

        public static TheoryData<string> GetStringTheoryData()
        {
            var data = new TheoryData<string>
            {
                string.Empty,
                reasonable
            };
            return data;
        }

        public static TheoryData<string> GetStringWithNullTheoryData()
        {
            var data = new TheoryData<string>
            {
                null,
                string.Empty,
                reasonable
            };
            return data;
        }

        public static TheoryData<string> GetNullOrEmptyStringTheoryData()
        {
            var data = new TheoryData<string>
            {
                null,
                string.Empty
            };
            return data;
        }

        public static TheoryData<string, string> GetStringNormalizedTheoryData()
        {
            var data = new TheoryData<string, string>
            {
                { null, string.Empty },
                { string.Empty, string.Empty },
                { reasonable, reasonable }
            };
            return data;
        }

        public static TheoryData<string, string, int> GetCtrlBackspaceData()
        {
            var data = new TheoryData<string, string, int>
            {
                { "aaa", "", 0 },
                { "---", "", 0 },
                { " aaa", "", 0 },
                { " ---", "", 0 },
                { "aaa---", "", 0 },
                { "---aaa", "---", 0 },
                { "aaa---aaa", "aaa---", 0 },
                { "---aaa---", "---", 0 },
                { "a-a", "a-", 0 },
                { "-a-", "", 0 },
                { "--a-", "--", 0 },
                { "abc", "c", -1 },
                { "a,1-b", "a,b", -1 }
            };
            return data;
        }

        public static TheoryData<string, string, int> GetCtrlBackspaceRepeatedData()
        {
            var data = new TheoryData<string, string, int>
            {
                { "aaa", "", 2 },
                { "---", "", 2 },
                { "aaa---aaa", "", 2 },
                { "---aaa---", "", 2 },
                { "aaa bbb", "", 2 },
                { "aaa bbb ccc", "aaa ", 2 },
                { "aaa --- ccc", "", 2 },
                { "1 2 3 4 5 6 7 8 9 0", "1 ", 9 }
            };
            return data;
        }

        public static TheoryData<char> GetCharTheoryData()
        {
            var data = new TheoryData<char>
            {
                '\0',
                'a'
            };
            return data;
        }

        public static TheoryData<IntPtr> GetIntPtrTheoryData()
        {
            var data = new TheoryData<IntPtr>
            {
                (IntPtr)(-1),
                IntPtr.Zero,
                (IntPtr)1
            };
            return data;
        }

        public static TheoryData<Guid> GetGuidTheoryData()
        {
            var data = new TheoryData<Guid>
            {
                Guid.Empty,
                Guid.NewGuid()
            };
            return data;
        }

        public static TheoryData<Color> GetColorTheoryData()
        {
            var data = new TheoryData<Color>
            {
                Color.Red,
                Color.Blue,
                Color.Black
            };
            return data;
        }

        public static TheoryData<Color> GetColorWithEmptyTheoryData()
        {
            var data = new TheoryData<Color>
            {
                Color.Red,
                Color.Empty
            };
            return data;
        }

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
                { Color.Empty, Control.DefaultForeColor }
            };
        }

        public static TheoryData<Image> GetImageTheoryData()
        {
            var data = new TheoryData<Image>
            {
                new Bitmap(10, 10),
                null
            };
            return data;
        }

        public static TheoryData<Font> GetFontTheoryData()
        {
            var data = new TheoryData<Font>
            {
                SystemFonts.MenuFont,
                null
            };
            return data;
        }

        public static TheoryData<Type> GetTypeWithNullTheoryData()
        {
            var data = new TheoryData<Type>
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

        public static TheoryData<Point> GetPointTheoryData() => GetPointTheoryData(TestIncludeType.All);

        public static TheoryData<Point> GetPointTheoryData(TestIncludeType includeType)
        {
            var data = new TheoryData<Point>();
            if (!includeType.HasFlag(TestIncludeType.NoPositives))
            {
                data.Add(new Point());
                data.Add(new Point(10));
                data.Add(new Point(1, 2));
            }
            if (!includeType.HasFlag(TestIncludeType.NoNegatives))
            {
                data.Add(new Point(int.MaxValue, int.MinValue));
                data.Add(new Point(-1, -2));
            }
            return data;
        }

        public static TheoryData<Size> GetSizeTheoryData() => GetSizeTheoryData(TestIncludeType.All);

        public static TheoryData<Size> GetSizeTheoryData(TestIncludeType includeType)
        {
            var data = new TheoryData<Size>();
            if (!includeType.HasFlag(TestIncludeType.NoPositives))
            {
                data.Add(new Size());
                data.Add(new Size(new Point(1, 1)));
                data.Add(new Size(1, 2));
            }
            if (!includeType.HasFlag(TestIncludeType.NoNegatives))
            {
                data.Add(new Size(-1, 1));
                data.Add(new Size(1, -1));
            }
            return data;
        }

        public static TheoryData<Size> GetPositiveSizeTheoryData()
        {
            var data = new TheoryData<Size>
            {
                new Size(),
                new Size(1, 2)
            };
            return data;
        }

        public static TheoryData<Rectangle> GetRectangleTheoryData()
        {
            var data = new TheoryData<Rectangle>
            {
                new Rectangle(),
                new Rectangle(1, 2, 3, 4),
                new Rectangle(-1, -2, -3, -4)
            };
            return data;
        }

        public static TheoryData<Padding> GetPaddingTheoryData()
        {
            var data = new TheoryData<Padding>
            {
                new Padding(),
                new Padding(1, 2, 3, 4),
                new Padding(1),
                new Padding(-1, -2, -3, -4)
            };
            return data;
        }

        public static TheoryData<Padding, Padding> GetPaddingNormalizedTheoryData()
        {
            var data = new TheoryData<Padding, Padding>
            {
                { new Padding(), new Padding() },
                { new Padding(1, 2, 3, 4), new Padding(1, 2, 3, 4) },
                { new Padding(1), new Padding(1) },
                { new Padding(-1, -2, -3, -4), Padding.Empty }
            };
            return data;
        }

        public static TheoryData<Type, bool> GetConvertFromTheoryData()
        {
            var data = new TheoryData<Type, bool>
            {
                { typeof(bool), false },
                { typeof(InstanceDescriptor), true },
                { typeof(int), false },
                { typeof(double), false },
                { null, false }
            };
            return data;
        }

        public static TheoryData<Cursor> GetCursorTheoryData()
        {
            var data = new TheoryData<Cursor>
            {
                null,
                new Cursor((IntPtr)1)
            };
            return data;
        }

        public static TheoryData<EventArgs> GetEventArgsTheoryData()
        {
            var data = new TheoryData<EventArgs>
            {
                null,
                new EventArgs()
            };
            return data;
        }

        public static TheoryData<PaintEventArgs> GetPaintEventArgsTheoryData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);
            return new TheoryData<PaintEventArgs>
            {
                null,
                new PaintEventArgs(graphics, Rectangle.Empty)
            };
        }

        public static TheoryData<KeyEventArgs> GetKeyEventArgsTheoryData()
        {
            var data = new TheoryData<KeyEventArgs>
            {
                null,
                new KeyEventArgs(Keys.Cancel)
            };
            return data;
        }

        public static TheoryData<KeyPressEventArgs> GetKeyPressEventArgsTheoryData()
        {
            var data = new TheoryData<KeyPressEventArgs>
            {
                null,
                new KeyPressEventArgs('1')
            };
            return data;
        }

        public static TheoryData<LayoutEventArgs> GetLayoutEventArgsTheoryData()
        {
            var data = new TheoryData<LayoutEventArgs>
            {
                null,
                new LayoutEventArgs(null, null),
                new LayoutEventArgs(new Control(), "affectedProperty")
            };
            return data;
        }

        public static TheoryData<MouseEventArgs> GetMouseEventArgsTheoryData()
        {
            return new TheoryData<MouseEventArgs>
            {
                null,
                new MouseEventArgs(MouseButtons.Left, 1, 2, 3, 4),
                new HandledMouseEventArgs(MouseButtons.Left, 1, 2, 3, 4)
            };
        }

        public static TheoryData<IServiceProvider, object> GetEditValueInvalidProviderTestData()
        {
            var nullServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            nullServiceProviderMock
                .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
                .Returns(null);
            var invalidServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            invalidServiceProviderMock
                .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
                .Returns(new object());
            var value = new object();
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

        #endregion
    }

    [Flags]
    public enum TestIncludeType
    {
        All,
        NoPositives,
        NoNegatives
    }
}
