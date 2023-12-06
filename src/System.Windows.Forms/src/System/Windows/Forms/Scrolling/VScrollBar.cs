// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  Represents a standard Windows vertical scroll bar.
/// </summary>
[SRDescription(nameof(SR.DescriptionVScrollBar))]
public partial class VScrollBar : ScrollBar
{
    private const int DefaultHeight = 80;

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.Style |= (int)SCROLLBAR_CONSTANTS.SB_VERT;
            return cp;
        }
    }

    protected override Size DefaultSize
    {
        get
        {
            if (ScaleHelper.IsScalingRequirementMet)
            {
                return new Size(SystemInformation.GetVerticalScrollBarWidthForDpi(_deviceDpi), LogicalToDeviceUnits(DefaultHeight));
            }

            return new Size(SystemInformation.VerticalScrollBarWidth, DefaultHeight);
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override RightToLeft RightToLeft
    {
        get => RightToLeft.No;
        set { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? RightToLeftChanged
    {
        add => base.RightToLeftChanged += value;
        remove => base.RightToLeftChanged -= value;
    }
}
