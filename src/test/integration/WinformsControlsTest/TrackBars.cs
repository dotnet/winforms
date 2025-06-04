// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public partial class TrackBars : Form
{
    public TrackBars()
    {
        InitializeComponent();

        trackBar1.Minimum = -100;
        trackBar1.Maximum = 100;
        numericMinimum.Value = trackBar1.Minimum;
        numericMaximum.Value = trackBar1.Maximum;
        numericFrequency.Value = trackBar1.TickFrequency;

        UpdateSizeLabel();
        UpdateValueLabel();
    }

    private void UpdateSizeLabel()
    {
        if (trackBar1.Orientation == Orientation.Horizontal)
        {
            lblTrackBarSize.Text = $"Trackbar width: {trackBar1.Width}";
        }
        else
        {
            lblTrackBarSize.Text = $"Trackbar height: {trackBar1.Height}";
        }
    }

    private void UpdateValueLabel()
    {
        lblTrackBarValue.Text = $"Value {trackBar1.Value}";
    }

    private void rbHorizontal_CheckedChanged(object sender, EventArgs e)
    {
        trackBar1.Orientation = Orientation.Horizontal;
        trackBar1.Width = ClientRectangle.Width - trackBar1.Left * 2;
        trackBar1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
    }

    private void rbVertical_CheckedChanged(object sender, EventArgs e)
    {
        trackBar1.Orientation = Orientation.Vertical;
        trackBar1.Height = ClientRectangle.Height - trackBar1.Top * 2;
        trackBar1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
    }

    private void chbRightToLeft_CheckedChanged(object sender, EventArgs e)
    {
        trackBar1.RightToLeft = chbRightToLeft.Checked ? RightToLeft.Yes : RightToLeft.No;
    }

    private void tickstyleNone_CheckedChanged(object sender, EventArgs e)
    {
        if (trackBar1.TickStyle == TickStyle.BottomRight)
        {
            trackBar1.TickStyle = TickStyle.None;
        }
        else
        {
            trackBar1.TickStyle = TickStyle.BottomRight;
        }
    }

    private void numericMinimum_ValueChanged(object sender, EventArgs e)
    {
        trackBar1.Minimum = (int)Math.Max(numericMinimum.Value, int.MinValue);
        numericMaximum.Minimum = numericMinimum.Value;
        numericFrequency.Maximum = (int)Math.Min(numericMaximum.Value - numericMaximum.Minimum, int.MaxValue);
        if (numericFrequency.Maximum == 0)
        {
            numericFrequency.Minimum = 1;
        }

        UpdateValueLabel();
    }

    private void numericMaximum_ValueChanged(object sender, EventArgs e)
    {
        trackBar1.Maximum = (int)Math.Min(numericMaximum.Value, int.MaxValue);
        numericMinimum.Maximum = numericMaximum.Value;
        numericFrequency.Maximum = (int)Math.Min(numericMaximum.Value - numericMaximum.Minimum, int.MaxValue);
        if (numericFrequency.Maximum == 0)
        {
            numericFrequency.Minimum = 1;
        }

        UpdateValueLabel();
    }

    private void numericFrequency_ValueChanged(object sender, EventArgs e)
    {
        trackBar1.TickFrequency = (int)numericFrequency.Value;
    }

    private void trackBar1_Scroll(object sender, EventArgs e)
    {
        UpdateValueLabel();
    }

    private void chbRightToLeftLayout_CheckedChanged(object sender, EventArgs e)
    {
        trackBar1.RightToLeftLayout = chbRightToLeftLayout.Checked;
    }

    private void trackBar1_SizeChanged(object sender, EventArgs e)
    {
        UpdateSizeLabel();
    }
}
