// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;

namespace System.Windows.Forms
{
    internal class ToolStripCustomIComparer : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            if (x.GetType() == y.GetType())
            {
                return 0; // same type
            }
            if (x.GetType().IsAssignableFrom(y.GetType()))
            {
                return 1;
            }
            if (y.GetType().IsAssignableFrom(x.GetType()))
            {
                return -1;
            }
            return 0; // not the same type, not in each other inheritance chain
        }
    }
}
