// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;

namespace WinformsControlsTest
{
    public partial class TrackBars : Form
    {
        public TrackBars()
        {
            InitializeComponent();
            UpdateValueLabel();
        }

        private void rbHorizontal_CheckedChanged(object sender, EventArgs e)
        {
            trackBar1.Orientation = Orientation.Horizontal;
        }

        private void rbVertical_CheckedChanged(object sender, EventArgs e)
        {
            trackBar1.Orientation = Orientation.Vertical;
        }

        private void chbRightToLeft_CheckedChanged(object sender, EventArgs e)
        {
            trackBar1.RightToLeft = chbRightToLeft.Checked ? RightToLeft.Yes : RightToLeft.No;
        }

        private void numericMinimum_ValueChanged(object sender, EventArgs e)
        {
            trackBar1.Minimum = Decimal.ToInt32(numericMinimum.Value);
            numericMaximum.Minimum = numericMinimum.Value;
            UpdateValueLabel();
        }

        private void numericMaximum_ValueChanged(object sender, EventArgs e)
        {
            trackBar1.Maximum = Decimal.ToInt32(numericMaximum.Value);
            numericMinimum.Maximum = numericMaximum.Value;
            UpdateValueLabel();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            UpdateValueLabel();
        }

        private void chbRightToLeftLayout_CheckedChanged(object sender, EventArgs e)
        {
            trackBar1.RightToLeftLayout = chbRightToLeftLayout.Checked;
        }

        private void UpdateValueLabel()
        {
            lblTrackBarValue.Text = $"Value {trackBar1.Value}";
        }
    }
}
