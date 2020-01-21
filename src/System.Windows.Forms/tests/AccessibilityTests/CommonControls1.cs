// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;

namespace AccessibilityTests
{
    public partial class Commontrol1 : Form
    {
        public Commontrol1()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ImageList imageListSmall = new ImageList();
            ImageList imageListLarge = new ImageList();
            //imageListSmall.Images.Add(Bitmap.FromFile("\\mlangfs1\\public\\Winforms\\v-jiaol\\picture\\mySmallImage1.bmp"));
            //imageListSmall.Images.Add(Bitmap.FromFile("\\mlangfs1\\public\\Winforms\\v-jiaol\\picture\\mySmallImage2.bmp"));
            //imageListLarge.Images.Add(Bitmap.FromFile("\\mlangfs1\\public\\Winforms\\v-jiaol\\picture\\myLargeImage1.bmp"));
            //imageListLarge.Images.Add(Bitmap.FromFile("\\mlangfs1\\public\\Winforms\\v-jiaol\\picture\\myLargeImage2.bmp"));
            listView1.LargeImageList = imageListLarge;
            listView1.SmallImageList = imageListSmall;

           // comboBox8.SelectedText = "row6";
            //comboBox9.SelectedItem = "selected";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listView1.View = View.LargeIcon;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listView1.View = View.SmallIcon;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.View = View.Details;
        }

        private void button3_Click(object sender, EventArgs e)
        {
           listView1.View = View.List;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            listView1.ShowGroups = true;
            listView1.Groups[0].Items.Add(listView1.Items[0]);
            listView1.Groups[1].Items.AddRange(new ListViewItem[] { listView1.Items[1], listView1.Items[2] });
        }

        private void button7_Click(object sender, EventArgs e)
        {
            listView1.ShowGroups = false;
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
        }
    }
}
