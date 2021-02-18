// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms.Layout;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStrip
    {
        private class ToolStripAccessibleObjectWrapperForItemsOnOverflow : ToolStripItem.ToolStripItemAccessibleObject
        {
            public ToolStripAccessibleObjectWrapperForItemsOnOverflow(ToolStripItem item)
                : base(item)
            {
            }
            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = base.State;
                    state |= AccessibleStates.Offscreen;
                    state |= AccessibleStates.Invisible;
                    return state;
                }
            }
        }
    }
}
