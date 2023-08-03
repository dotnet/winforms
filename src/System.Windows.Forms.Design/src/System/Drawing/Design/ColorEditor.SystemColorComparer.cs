// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Drawing.Design;

public partial class ColorEditor
{
    /// <summary>
    ///  Comparer for system colors.
    /// </summary>
    private class SystemColorComparer : IComparer<Color>
    {
        public int Compare(Color x, Color y)
            => string.Compare(x.Name, y.Name, false, CultureInfo.InvariantCulture);
    }
}
