// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Design;

public partial class ColorEditor
{
    /// <summary>
    ///  Comparer for standard colors.
    /// </summary>
    private class StandardColorComparer : IComparer<Color>
    {
        public static StandardColorComparer Instance { get; } = new();

        private StandardColorComparer() { }
        public int Compare(Color left, Color right)
        {
            if (left.A < right.A)
            {
                return -1;
            }

            if (left.A > right.A)
            {
                return 1;
            }

            if (left.GetHue() < right.GetHue())
            {
                return -1;
            }

            if (left.GetHue() > right.GetHue())
            {
                return 1;
            }

            if (left.GetSaturation() < right.GetSaturation())
            {
                return -1;
            }

            if (left.GetSaturation() > right.GetSaturation())
            {
                return 1;
            }

            if (left.GetBrightness() < right.GetBrightness())
            {
                return -1;
            }

            if (left.GetBrightness() > right.GetBrightness())
            {
                return 1;
            }

            return 0;
        }
    }
}
