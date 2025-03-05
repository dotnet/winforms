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
        private ControlTabOrderComparer() { }

        internal static ControlTabOrderComparer Instance { get; } = new();

        public int Compare(ControlTabOrderHolder x, ControlTabOrderHolder y)
        {
            if (IComparerHelpers.CompareReturnIfNull(x, y, out int? returnValue))
            {
                return (int)returnValue;
            }

            // If there is a specified tab index, use it for comparison, otherwise use the original index (which
            // would be the index in the control collection or how Windows returns children using GW_HWNDNEXT from
            // GW_HWNDCHILD).
            int delta = x.TabIndex - y.TabIndex;
            if (delta == 0)
            {
                delta = x.OriginalIndex - y.OriginalIndex;
            }

            return delta;
        }
    }
}
