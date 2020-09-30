// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public partial class DataGridViewHeaders : Form
    {
        private static readonly Font[] s_fonts = new Font[]
            {
                new Font("Tahoma", 12F, FontStyle.Regular),
                new Font("Fira Code", 10F, FontStyle.Bold),
                new Font("Consolas", 14F, FontStyle.Italic),
                new Font("Arial", 9F, FontStyle.Bold),
                new Font("Microsoft Sans Serif", 11F, FontStyle.Regular),
            };
        private int _cellFontIndex;
        private int _columnHeaderFontIndex;
        private int _rowHeaderFontIndex;

        public DataGridViewHeaders()
        {
            InitializeComponent();

            dataGridView1.Rows.Add("DefaultCellStyle", dataGridView1.DefaultCellStyle.Font.ToString());
            dataGridView1.Rows.Add("ColumnHeadersDefaultCellStyle", dataGridView1.ColumnHeadersDefaultCellStyle.Font.ToString());
            dataGridView1.Rows.Add("RowHeadersDefaultCellStyle", dataGridView1.RowHeadersDefaultCellStyle.Font.ToString());

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].HeaderCell.Value = $"Row {i + 1}";
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            currentDPILabel1.Text = DeviceDpi.ToString();
        }

        private void DataGridView_DpiChanged(object sender, DpiChangedEventArgs e)
        {
            currentDPILabel1.Text = DeviceDpi.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _cellFontIndex++;
            if (_cellFontIndex >= s_fonts.Length)
            {
                _cellFontIndex -= s_fonts.Length;
            }

            _columnHeaderFontIndex += 2;
            if (_columnHeaderFontIndex >= s_fonts.Length)
            {
                _columnHeaderFontIndex -= s_fonts.Length;
            }

            _rowHeaderFontIndex += 3;
            if (_rowHeaderFontIndex >= s_fonts.Length)
            {
                _rowHeaderFontIndex -= s_fonts.Length;
            }

            dataGridView1.DefaultCellStyle.Font = s_fonts[_cellFontIndex];
            dataGridView1.Rows[0].Cells[1].Value = s_fonts[_cellFontIndex];

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = s_fonts[_columnHeaderFontIndex];
            dataGridView1.Rows[1].Cells[1].Value = s_fonts[_columnHeaderFontIndex];

            dataGridView1.RowHeadersDefaultCellStyle.Font = s_fonts[_rowHeaderFontIndex];
            dataGridView1.Rows[2].Cells[1].Value = s_fonts[_rowHeaderFontIndex];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.DefaultCellStyle.Font = null;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = null;
            dataGridView1.RowHeadersDefaultCellStyle.Font = null;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Font = new Font("Tahoma", (float)numericUpDown1.Value, FontStyle.Regular);
        }
    }
}
