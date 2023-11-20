// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Globalization;

namespace System.Windows.Forms;

public partial class DomainUpDown
{
    private sealed class DomainUpDownItemCompare : IComparer
    {
        public int Compare(object? p, object? q)
        {
            if (p == q)
            {
                return 0;
            }

            if (p is null || q is null)
            {
                return 0;
            }

            return string.Compare(p.ToString(), q.ToString(), false, CultureInfo.CurrentCulture);
        }
    }
}
