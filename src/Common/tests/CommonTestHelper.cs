// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        public static TheoryData<IntPtr> GetIntPtrTheoryData()
        {
            var data = new TheoryData<IntPtr>();
            data.Add((IntPtr)(-1));
            data.Add(IntPtr.Zero);
            data.Add((IntPtr)1);
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

        public static TheoryData<Size> GetSizeTheoryData()
        {
            var data = new TheoryData<Size>();
            data.Add(new Size());
            data.Add(new Size(1, 2));
            data.Add(new Size(-1, -2));
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

        #endregion        
    }
}
