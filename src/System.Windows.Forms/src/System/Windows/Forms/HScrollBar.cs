// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a standard Windows horizontal scroll bar.
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [SRDescription(nameof(SR.DescriptionHScrollBar))]
    public class HScrollBar : ScrollBar
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= NativeMethods.SBS_HORZ;
                return cp;
            }
        }

        protected override Size DefaultSize
        {
            get => new Size(80, SystemInformation.HorizontalScrollBarHeight);
        }
    }
}
