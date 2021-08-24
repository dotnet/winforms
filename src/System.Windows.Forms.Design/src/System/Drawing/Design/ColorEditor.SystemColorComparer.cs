// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Globalization;

namespace System.Drawing.Design
{
    public partial class ColorEditor
    {
        /// <summary>
        ///  Comparer for system colors.
        /// </summary>
        private class SystemColorComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return string.Compare(((Color)x).Name, ((Color)y).Name, false, CultureInfo.InvariantCulture);
            }
        }
    }
}
