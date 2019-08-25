// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Forms
{
    public partial class Control
    {
        /// <summary>
        ///  This class contains a control and associates it with a z-order.
        ///  This is used when sorting controls based on tab index first,
        ///  z-order second.
        /// </summary>
        private class ControlTabOrderHolder
        {
            internal readonly int _oldOrder;
            internal readonly int _newOrder;
            internal readonly Control _control;

            internal ControlTabOrderHolder(int oldOrder, int newOrder, Control control)
            {
                _oldOrder = oldOrder;
                _newOrder = newOrder;
                _control = control;
            }
        }

        /// <summary>
        ///  Used to sort controls based on tab index and z-order.
        /// </summary>
        private class ControlTabOrderComparer : IComparer
        {
            int IComparer.Compare(object x, object y)
            {
                ControlTabOrderHolder hx = (ControlTabOrderHolder)x;
                ControlTabOrderHolder hy = (ControlTabOrderHolder)y;

                int delta = hx._newOrder - hy._newOrder;
                if (delta == 0)
                {
                    delta = hx._oldOrder - hy._oldOrder;
                }

                return delta;
            }
        }
    }
}
