// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Drawing;

namespace System.Windows.Forms.Tests
{
    public static class TestHelper
    {

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
            data.Add(new Size(new Point(1,1)));
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
