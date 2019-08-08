// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a standard Windows vertical scroll bar.
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [SRDescription(nameof(SR.DescriptionVScrollBar))]
    public class VScrollBar : ScrollBar
    {
        private const int DefaultHeight = 80;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= NativeMethods.SBS_VERT;
                return cp;
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                if (DpiHelper.IsScalingRequirementMet)
                {
                    return new Size(SystemInformation.GetVerticalScrollBarWidthForDpi(_deviceDpi), LogicalToDeviceUnits(DefaultHeight));
                }
                else
                {
                    return new Size(SystemInformation.VerticalScrollBarWidth, DefaultHeight);
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override RightToLeft RightToLeft
        {
            get => RightToLeft.No;
            set
            {
            }
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler RightToLeftChanged
        {
            add => base.RightToLeftChanged += value;
            remove => base.RightToLeftChanged -= value;
        }
    }
}
