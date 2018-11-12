using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace WinformsControlsTest
{
    public partial class MultipleControls : Form
    {
        public MultipleControls()
        {
            InitializeComponent();
        }

        private void Test3_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 1; i < 100; i++)
            {
                Thread.Sleep(100);
                backgroundWorker1.ReportProgress(i);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void MenuStripScaling_Load(object sender, EventArgs e)
        {
            checkedListBox1.Items.Add("Pennsylvania", CheckState.Checked);
        }

    }
}
