﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;

namespace System.Drawing.Design;

public partial class ColorEditor
{
    /// <summary>
    ///  Comparer for standard colors.
    /// </summary>
    private class StandardColorComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            Color left = (Color)x;
            Color right = (Color)y;

            if (left.A < right.A)
            {
                return -1;
            }

            if (left.A > right.A)
            {
                return 1;
            }

            if ((float)left.GetHue() < (float)right.GetHue())
            {
                return -1;
            }

            if ((float)left.GetHue() > (float)right.GetHue())
            {
                return 1;
            }

            if ((float)left.GetSaturation() < (float)right.GetSaturation())
            {
                return -1;
            }

            if ((float)left.GetSaturation() > (float)right.GetSaturation())
            {
                return 1;
            }

            if ((float)left.GetBrightness() < (float)right.GetBrightness())
            {
                return -1;
            }

            if ((float)left.GetBrightness() > (float)right.GetBrightness())
            {
                return 1;
            }

            return 0;
        }
    }
}
