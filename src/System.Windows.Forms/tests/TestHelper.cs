// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

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
            data.Add(float.MaxValue/2);
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
            data.Add("");
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
                if(info.ReturnType == typeof(Cursor))
                {
                    data.Add(info.Invoke(null, null) as Cursor);
                }
            }
            return data;
        }

        #endregion

        #region System.Drawing

        // helper method to generate theory data Point values
        public static TheoryData<System.Drawing.Point> GetPointTheoryData()
        {
            var data = new TheoryData<System.Drawing.Point>();
            data.Add(new System.Drawing.Point());
            data.Add(new System.Drawing.Point(10));
            data.Add(new System.Drawing.Point(1, 2));
            data.Add(new System.Drawing.Point(int.MaxValue, int.MinValue));
            return data;
        }

        // helper method to generate theory data Color values
        public static TheoryData<System.Drawing.Color> GetColorTheoryData()
        {
            var data = new TheoryData<System.Drawing.Color>();
            data.Add(System.Drawing.Color.Red);
            data.Add(System.Drawing.Color.Blue);
            data.Add(System.Drawing.Color.Black);
            return data;
        }
        
        // helper method to generate invalid theory data Color value(s)
        public static TheoryData<System.Drawing.Color> GetColorTheoryDataInvalid()
        {
            var data = new TheoryData<System.Drawing.Color>();
            data.Add(new System.Drawing.Color());
            data.Add(System.Drawing.Color.Empty);
            return data;
        }

        // helper method to generate theory data Point values
        public static TheoryData<System.Drawing.Size> GetSizeTheoryData()
        {
            var data = new TheoryData<System.Drawing.Size>();
            data.Add(new System.Drawing.Size());
            data.Add(new System.Drawing.Size(new System.Drawing.Point(1,1)));
            data.Add(new System.Drawing.Size(1, 2));
            data.Add(new System.Drawing.Size(int.MaxValue, int.MinValue));
            return data;
        }

        public static TheoryData<System.Drawing.Font> GetFontTheoryData()
        {
            var data = new TheoryData<System.Drawing.Font>();
            foreach (Drawing.Text.GenericFontFamilies genericFontFamily in Enum.GetValues(typeof(Drawing.Text.GenericFontFamilies)))
            {
                var family = new Drawing.FontFamily(genericFontFamily);
                data.Add(new Drawing.Font(family, System.Single.Epsilon));
                data.Add(new Drawing.Font(family, 10));
                data.Add(new Drawing.Font(family, 84));
                data.Add(new Drawing.Font(family, System.Single.MaxValue));
            }
            return data;
        }

        #endregion
    }
}
