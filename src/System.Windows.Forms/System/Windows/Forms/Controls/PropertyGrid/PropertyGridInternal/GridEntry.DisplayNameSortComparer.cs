// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms.PropertyGridInternal;

internal abstract partial class GridEntry
{
    public class DisplayNameSortComparer : IComparer
    {
        public int Compare(object? left, object? right)
        {
            return string.Compare(
                ((PropertyDescriptor)left!).DisplayName,
                ((PropertyDescriptor)right!).DisplayName,
                ignoreCase: true,
                CultureInfo.CurrentCulture);
        }
    }
}
