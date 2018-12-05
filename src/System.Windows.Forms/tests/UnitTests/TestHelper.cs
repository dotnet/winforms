// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Drawing;
using System.Linq;

namespace System.Windows.Forms.Tests
{
    public static class TestHelper
    {
        // helper method to generate theory data from all values of an enum type
        public static TheoryData<T> GetEnumTheoryData<T>() where T : Enum
        {
            var data = new TheoryData<T>();
            foreach (T item in Enum.GetValues(typeof(T)))
                data.Add(item);
            return data;
        }

        // helper method to generate invalid theory data for an enum type
        // This method assumes that int.MinValue and int.MaxValue are not in the enum
        public static TheoryData<T> GetEnumTheoryDataInvalid<T>() where T : Enum
        {
            var data = new TheoryData<T>();
            // This boxing is necessary because you can't cast an int to a generic,
            // even if the generic is guaranteed to be an enum
            data.Add((T)(object)int.MinValue);
            data.Add((T)(object)int.MaxValue);
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

        // helper method to generate theory data for some values of a int
        public static TheoryData<uint> GetUIntTheoryData()
        {
            var data = new TheoryData<uint>();
            data.Add(int.MaxValue);
            data.Add(0);
            data.Add(1);
            data.Add(int.MaxValue / 2);
            return data;
        }

        // helper method to generate theory data for some values of a int
        public static TheoryData<int> GetNIntTheoryData()
        {
            var data = new TheoryData<int>();
            data.Add(int.MinValue);
            data.Add(-1);
            data.Add(int.MinValue / 2);
            return data;
        }

        // helper method to generate theory data for some values of a int
        public static TheoryData<float> GetFloatTheoryData()
        {
            var data = new TheoryData<float>();
            data.Add(float.MaxValue);
            data.Add(float.MinValue);
            data.Add(Single.Epsilon);
            data.Add(Single.Epsilon * -1);
            data.Add(Single.NegativeInfinity); // not sure about these two
            data.Add(Single.PositiveInfinity); // 2
            data.Add(0);
            data.Add(-1);
            data.Add(1);
            data.Add(float.MaxValue / 2);
            return data;
        }

        // helper method to generate theory data for some values of a int
        public static TheoryData<float> GetUFloatTheoryData()
        {
            var data = new TheoryData<float>();
            data.Add(float.MaxValue);
            data.Add(Single.Epsilon);
            data.Add(Single.PositiveInfinity); // not sure about this one
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

        #endregion

        #region System.Windows.Forms

        // helper method to generate theory data for Padding
        public static TheoryData<Padding> GetPaddingTheoryData()
        {
            var data = new TheoryData<Padding>();
            data.Add(new Padding());
            data.Add(new Padding(0));
            data.Add(new Padding(1));
            data.Add(new Padding(int.MaxValue));
            return data;
        }

        // helper method to generate invalid theory data for Padding
        public static TheoryData<Padding> GetPaddingTheoryDataInvalid()
        {
            var data = new TheoryData<Padding>();
            data.Add(new Padding(-1));
            data.Add(new Padding(int.MinValue));
            return data;
        }

        public static TheoryData<Cursor> GetCursorTheoryData()
        {

            var data = new TheoryData<Cursor>();
            foreach (System.Reflection.MethodInfo info in typeof(Cursors).GetMethods())
            {
                if (info.ReturnType == typeof(Cursor))
                {
                    data.Add(info.Invoke(null, null) as Cursor);
                }
            }
            return data;
        }

        // get some fake audio data to be used in DataObject tests
        public static TheoryData<Memory<byte>> GetMemoryBytes()
        {
            var audioData = new TheoryData<Memory<byte>>();
            audioData.Add(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }.AsMemory());
            audioData.Add(new byte[] { 10, 55, 99, 255 }.AsMemory());
            audioData.Add(Enumerable.Range(0, 255).Select(a => (byte)a).ToArray());
            return audioData;
        }

        #endregion

        #region 

        // helper method to generate theory data Point values
        public static TheoryData<Point> GetPointTheoryData()
        {
            var data = new TheoryData<Point>();
            data.Add(new Point());
            data.Add(new Point(10));
            data.Add(new Point(1, 2));
            data.Add(new Point(int.MaxValue, int.MinValue));
            return data;
        }

        // helper method to generate theory data Color values
        public static TheoryData<Color> GetColorTheoryData()
        {
            var data = new TheoryData<Color>();
            data.Add(Color.Red);
            data.Add(Color.Blue);
            data.Add(Color.Black);
            return data;
        }

        // helper method to generate invalid theory data Color value(s)
        public static TheoryData<Color> GetColorTheoryDataInvalid()
        {
            var data = new TheoryData<Color>();
            data.Add(new Color());
            data.Add(Color.Empty);
            return data;
        }

        // helper method to generate theory data Point values
        public static TheoryData<Size> GetSizeTheoryData()
        {
            var data = new TheoryData<Size>();
            data.Add(new Size());
            data.Add(new Size(new Point(1, 1)));
            data.Add(new Size(1, 2));
            data.Add(new Size(int.MaxValue, int.MinValue));
            return data;
        }

        public static TheoryData<Font> GetFontTheoryData()
        {
            var data = new TheoryData<Font>();
            foreach (Drawing.Text.GenericFontFamilies genericFontFamily in Enum.GetValues(typeof(Drawing.Text.GenericFontFamilies)))
            {
                var family = new FontFamily(genericFontFamily);
                data.Add(new Font(family, System.Single.Epsilon));
                data.Add(new Font(family, 10));
                data.Add(new Font(family, 84));
                data.Add(new Font(family, System.Single.MaxValue));
            }
            return data;
        }

        #endregion
    }
}
