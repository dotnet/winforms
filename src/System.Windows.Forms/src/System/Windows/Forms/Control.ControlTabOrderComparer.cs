// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class Control
    {
        /// <summary>
        ///  Used to sort controls based on tab index and z-order.
        /// </summary>
        private class ControlTabOrderComparer : IComparer<ControlTabOrderHolder>
        {
            public int Compare(ControlTabOrderHolder? x, ControlTabOrderHolder? y)
            {
                if (x is null && y is null)
                {
                    return 0;
                }
                else if (x is null)
                {
                    return -1;
                }
                else if (y is null)
                {
                    return 1;
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
}
