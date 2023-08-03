// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class Control
{
    /// <summary>
    ///  Used to sort controls based on tab index and z-order.
    /// </summary>
    private class ControlTabOrderComparer : IComparer<ControlTabOrderHolder>
    {
        public int Compare(ControlTabOrderHolder? x, ControlTabOrderHolder? y)
        {
            if (IComparerHelpers.CompareReturnIfNull(x, y, out int? returnValue))
            {
                return (int)returnValue;
            }

            int delta = x._newOrder - y._newOrder;
            if (delta == 0)
            {
                delta = x._oldOrder - y._oldOrder;
            }

            return delta;
        }
    }
}
