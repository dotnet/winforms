using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public partial class Buttons : Form
    {
        public Buttons()
        {
            InitializeComponent();
        }

        private FlatStyle[] styles = { FlatStyle.Flat, FlatStyle.Popup, FlatStyle.Standard, FlatStyle.System };
        private void Test1_Load(object sender, EventArgs e)
        {
            RadioButton r;
            for (int i = 0; i < styles.Length; i++)
            {
                r = new RadioButton();
                r.Location = new System.Drawing.Point(20, 20 + 60 * i);
                r.AutoSize = false;
                r.Size = new System.Drawing.Size(100, 20);
                r.Text = styles[i].ToString();
                r.Checked = true;
                Controls.Add(r);
            }

            CheckBox c;
            for (int i = 0; i < styles.Length; i++)
            {
                c = new CheckBox();
                c.Location = new System.Drawing.Point(120, 20 + 60 * i);
                c.AutoSize = false;
                c.Size = new System.Drawing.Size(100, 20);
                c.Text = styles[i].ToString();
                c.Checked = true;
                Controls.Add(c);
            }

            Button b;
            for (int i = 0; i < styles.Length; i++)
            {
                b = new Button();
                b.Location = new System.Drawing.Point(220, 20 + 60 * i);
                b.AutoSize = false;
                b.Size = new System.Drawing.Size(100, 20);
                b.Text = styles[i].ToString();
                Controls.Add(b);
            }
        }
    }
}
