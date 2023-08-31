// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
namespace System.Windows.Forms;

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
            cp.Style |= (int)SCROLLBAR_CONSTANTS.SB_HORZ;
            return cp;
        }
    }

    protected override Size DefaultSize
        => new(80, SystemInformation.HorizontalScrollBarHeight);
}
