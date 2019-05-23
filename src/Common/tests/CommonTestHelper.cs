// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Security;
using System.Windows.Forms;
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
                data.Add(item);
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
            var data = new TheoryData<T>();
            // This boxing is necessary because you can't cast an int to a generic,
            // even if the generic is guaranteed to be an enum
            data.Add((T)(object)int.MinValue);
            data.Add((T)(object)int.MaxValue);
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
            var data = new TheoryData<bool>();
            data.Add(true);
            data.Add(false);
            return data;
        }

        // helper method to generate theory data for some values of a int
        public static TheoryData<int> GetIntTheoryData()
        {
            var data = new TheoryData<int>();
            data.Add(int.MinValue);
            data.Add(int.MaxValue);
            data.Add(0);
            data.Add(1);
            data.Add(-1);
            data.Add(int.MaxValue / 2);
            return data;
        }

        public static TheoryData<int> GetNonNegativeIntTheoryData()
        {
            var data = new TheoryData<int>();
            data.Add(int.MaxValue);
            data.Add(0);
            data.Add(1);
            data.Add(int.MaxValue / 2);
            return data;
        }

        // helper method to generate theory data for some values of a int
        internal static TheoryData<uint> GetUIntTheoryData()
        {
            var data = new TheoryData<uint>();
            data.Add(int.MaxValue);
            data.Add(0);
            data.Add(1);
            data.Add(int.MaxValue / 2);
            return data;
        }

        // helper method to generate theory data for some values of a int
        internal static TheoryData<int> GetNIntTheoryData()
        {
            var data = new TheoryData<int>();
            data.Add(int.MinValue);
            data.Add(-1);
            data.Add(int.MinValue / 2);
            return data;
        }

        // helper method to generate theory data for some values of a int
        internal static TheoryData<float> GetFloatTheoryData()
        {
            var data = new TheoryData<float>();
            data.Add(float.MaxValue);
            data.Add(float.MinValue);
            data.Add(float.Epsilon);
            data.Add(float.Epsilon * -1);
            data.Add(float.NegativeInfinity); // not sure about these two
            data.Add(float.PositiveInfinity); // 2
            data.Add(0);
            data.Add(-1);
            data.Add(1);
            data.Add(float.MaxValue / 2);
            return data;
        }

        // helper method to generate theory data for some values of a int
        internal static TheoryData<float> GetUFloatTheoryData()
        {
            var data = new TheoryData<float>();
            data.Add(float.MaxValue);
            data.Add(float.Epsilon);
            data.Add(float.PositiveInfinity); // not sure about this one
            data.Add(0);
            data.Add(1);
            data.Add(float.MaxValue / 2);
            return data;
        }

        // helper method to generate theory data for a span of string values
        public static TheoryData<string> GetStringTheoryData()
        {
            var data = new TheoryData<string>();
            data.Add(string.Empty);
            data.Add("reasonable");
            return data;
        }

        public static TheoryData<string> GetStringWithNullTheoryData()
        {
            var data = new TheoryData<string>();
            data.Add(null);
            data.Add(string.Empty);
            data.Add("reasonable");
            return data;
        }

        public static TheoryData<string> GetNullOrEmptyStringTheoryData()
        {
            var data = new TheoryData<string>();
            data.Add(null);
            data.Add(string.Empty);
            return data;
        }

        public static TheoryData<string, string> GetStringNormalizedTheoryData()
        {
            var data = new TheoryData<string, string>();
            data.Add(null, string.Empty);
            data.Add(string.Empty, string.Empty);
            data.Add("reasonable", "reasonable");
            return data;
        }

        public static TheoryData<string, string, int> GetCtrlBackspaceData()
        {
            var data = new TheoryData<string, string, int>();
            data.Add("aaa", "", 0);
            data.Add("---", "", 0);
            data.Add(" aaa", "", 0);
            data.Add(" ---", "", 0);
            data.Add("aaa---", "", 0);
            data.Add("---aaa", "---", 0);
            data.Add("aaa---aaa", "aaa---", 0);
            data.Add("---aaa---", "---", 0);
            data.Add("a-a", "a-", 0);
            data.Add("-a-", "", 0);
            data.Add("--a-", "--", 0);
            data.Add("abc", "c", -1);
            data.Add("a,1-b", "a,b", -1);
            return data;
        }

        public static TheoryData<string, string, int> GetCtrlBackspaceRepeatedData()
        {
            var data = new TheoryData<string, string, int>();
            data.Add("aaa", "", 2);
            data.Add("---", "", 2);
            data.Add("aaa---aaa", "", 2);
            data.Add("---aaa---", "", 2);
            data.Add("aaa bbb", "", 2);
            data.Add("aaa bbb ccc", "aaa ", 2);
            data.Add("aaa --- ccc", "", 2);
            data.Add("1 2 3 4 5 6 7 8 9 0", "1 ", 9);
            return data;
        }

        public static TheoryData<char> GetCharTheoryData()
        {
            var data = new TheoryData<char>();
            data.Add('\0');
            data.Add('a');
            return data;
        }

        public static TheoryData<IntPtr> GetIntPtrTheoryData()
        {
            var data = new TheoryData<IntPtr>();
            data.Add((IntPtr)(-1));
            data.Add(IntPtr.Zero);
            data.Add((IntPtr)1);
            return data;
        }

        public static TheoryData<Guid> GetGuidTheoryData()
        {
            var data = new TheoryData<Guid>();
            data.Add(Guid.Empty);
            data.Add(Guid.NewGuid());
            return data;
        }

        public static TheoryData<Color> GetColorTheoryData()
        {
            var data = new TheoryData<Color>();
            data.Add(Color.Red);
            data.Add(Color.Blue);
            data.Add(Color.Black);
            return data;
        }

        public static TheoryData<Color> GetColorWithEmptyTheoryData()
        {
            var data = new TheoryData<Color>();
            data.Add(Color.Red);
            data.Add(Color.Empty);
            return data;
        }

        public static TheoryData<Image> GetImageTheoryData()
        {
            var data = new TheoryData<Image>();
            data.Add(new Bitmap(10, 10));
            data.Add(null);
            return data;
        }

        public static TheoryData<Font> GetFontTheoryData()
        {
            var data = new TheoryData<Font>();
            data.Add(SystemFonts.MenuFont);
            data.Add(null);
            return data;
        }

        public static TheoryData<Type> GetTypeWithNullTheoryData()
        {
            var data = new TheoryData<Type>();
            data.Add(null);
            data.Add(typeof(int));
            return data;
        }

        public static TheoryData<RightToLeft, RightToLeft> GetRightToLeftTheoryData()
        {
            var data = new TheoryData<RightToLeft, RightToLeft>();
            data.Add(RightToLeft.Inherit, RightToLeft.No);
            data.Add(RightToLeft.Yes, RightToLeft.Yes);
            data.Add(RightToLeft.No, RightToLeft.No);
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
            var data = new TheoryData<Size>();
            data.Add(new Size());
            data.Add(new Size(1, 2));
            return data;
        }

        public static TheoryData<Rectangle> GetRectangleTheoryData()
        {
            var data = new TheoryData<Rectangle>();
            data.Add(new Rectangle());
            data.Add(new Rectangle(1, 2, 3, 4));
            data.Add(new Rectangle(-1, -2, -3, -4));
            return data;
        }

        public static TheoryData<Padding> GetPaddingTheoryData()
        {
            var data = new TheoryData<Padding>();
            data.Add(new Padding());
            data.Add(new Padding(1, 2, 3, 4));
            data.Add(new Padding(1));
            data.Add(new Padding(-1, -2, -3, -4));
            return data;
        }

        public static TheoryData<Padding, Padding> GetPaddingNormalizedTheoryData()
        {
            var data = new TheoryData<Padding, Padding>();
            data.Add(new Padding(), new Padding());
            data.Add(new Padding(1, 2, 3, 4), new Padding(1, 2, 3, 4));
            data.Add(new Padding(1), new Padding(1));
            data.Add(new Padding(-1, -2, -3, -4), Padding.Empty);
            return data;
        }

        public static TheoryData<Type, bool> GetConvertFromTheoryData()
        {
            var data = new TheoryData<Type, bool>();
            data.Add(typeof(bool), false);
            data.Add(typeof(InstanceDescriptor), true);
            data.Add(typeof(int), false);
            data.Add(typeof(double), false);
            data.Add(null, false);
            return data;
        }

        public static TheoryData<Cursor> GetCursorTheoryData()
        {
            var data = new TheoryData<Cursor>();
            data.Add(null);
            data.Add(new Cursor((IntPtr)1));
            return data;
        }

        public static TheoryData<EventArgs> GetEventArgsTheoryData()
        {
            var data = new TheoryData<EventArgs>();
            data.Add(null);
            data.Add(new EventArgs());
            return data;
        }

        public static TheoryData<Exception> GetSecurityOrCriticalException()
        {
            var data = new TheoryData<Exception>();
            data.Add(new NullReferenceException());
            data.Add(new SecurityException());
            return data;
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
