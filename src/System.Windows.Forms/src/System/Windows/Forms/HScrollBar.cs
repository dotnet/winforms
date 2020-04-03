// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a standard Windows horizontal scroll bar.
    /// </summary>
    [SRDescription(nameof(SR.DescriptionHScrollBar))]
    public partial class HScrollBar : ScrollBar
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= (int)User32.SBS.HORZ;
                return cp;
            }
        }

        protected override Size DefaultSize
            => new Size(80, SystemInformation.HorizontalScrollBarHeight);

        protected override AccessibleObject CreateAccessibilityInstance()
            => new HScrollBarAccessibleObject(this);
    }
}
