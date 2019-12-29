// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public partial class ListViewTest : Form
    {
        public ListViewTest()
        {
            InitializeComponent();
            CreateMyListView();

            listView1.LabelEdit = true;
            listView1.View = View.Tile;

            var random = new Random();

            Debug.WriteLine(listView1.TileSize);
            listView1.TileSize = new Size(250, 250);
            listView1.Click += (s, e) =>
            {
                listView1.TileSize = new Size(random.Next(100, 300), random.Next(25, 50));
            };
        }

        private void CreateMyListView()
        {
            // Create a new ListView control.
            ListView listView2 = new ListView
            {
                Bounds = new Rectangle(new Point(0, 0), new Size(400, 200)),

                // Set the view to show details.
                View = View.Details,
                // Allow the user to edit item text.
                LabelEdit = true,
                // Allow the user to rearrange columns.
                AllowColumnReorder = true,
                // Display check boxes.
                CheckBoxes = true,
                // Select the item and subitems when selection is made.
                FullRowSelect = true,
                // Display grid lines.
                GridLines = true,
                // Sort the items in the list in ascending order.
                Sorting = SortOrder.Ascending
            };
            listView2.SelectedIndexChanged += listView2_SelectedIndexChanged;
            listView2.Click += listView2_Click;

            // Create three items and three sets of subitems for each item.
            ListViewItem item1 = new ListViewItem("item1", 0)
            {
                // Place a check mark next to the item.
                Checked = true
            };
            item1.SubItems.Add("1");
            item1.SubItems.Add("2");
            item1.SubItems.Add("3");
            ListViewItem item2 = new ListViewItem("item2", 1);
            item2.SubItems.Add("4");
            item2.SubItems.Add("5");
            item2.SubItems.Add("6");
            ListViewItem item3 = new ListViewItem("item3", 0)
            {
                // Place a check mark next to the item.
                Checked = true
            };
            item3.SubItems.Add("7");
            item3.SubItems.Add("8");
            item3.SubItems.Add("9");

            // Create columns for the items and subitems.
            // Width of -2 indicates auto-size.
            listView2.Columns.Add("column1", "Item Column", -2, HorizontalAlignment.Left, 0);
            listView2.Columns.Add("Column 2", -2, HorizontalAlignment.Left);
            listView2.Columns.Add("Column 3", -2, HorizontalAlignment.Left);
            listView2.Columns.Add("Column 4", -2, HorizontalAlignment.Center);
            listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            //Add the items to the ListView.
            listView2.Items.AddRange(new ListViewItem[] { item1, item2, item3 });

            // Create two ImageList objects.
            ImageList imageListSmall = new ImageList();
            ImageList imageListLarge = new ImageList();

            // Initialize the ImageList objects with bitmaps.\
            imageListSmall.Images.Add(Bitmap.FromFile("Images\\SmallA.bmp"));
            imageListSmall.Images.Add(Bitmap.FromFile("Images\\SmallABlue.bmp"));
            imageListLarge.Images.Add(Bitmap.FromFile("Images\\LargeA.bmp"));
            imageListLarge.Images.Add(Bitmap.FromFile("Images\\LargeABlue.bmp"));

            //Assign the ImageList objects to the ListView.
            listView2.LargeImageList = imageListLarge;
            listView2.SmallImageList = imageListSmall;

            ListViewGroup listViewGroup1 = new ListViewGroup("ListViewGroup", HorizontalAlignment.Left)
            {
                Header = "ListViewGroup",
                Name = "listViewGroup1"
            };
            listView2.Groups.AddRange(new ListViewGroup[] { listViewGroup1 });

            // Add the ListView to the control collection.
            Controls.Add(listView2);
            listView2.Dock = DockStyle.Bottom;
        }

        private void listView2_Click(object sender, System.EventArgs e)
        {
            Debug.WriteLine(listView1.TileSize);
            MessageBox.Show(this, "listView1_Click", "event");
        }

        private void listView2_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // MessageBox.Show(this, "listView2_SelectedIndexChanged", "event");

            var listView2 = sender as ListView;
            if (listView2 == null)
            {
                return;
            }

            var random = new Random();
            listView2.Columns[random.Next(0, listView2.Columns.Count)].ImageIndex = random.Next(0, 2);
        }
    }
}
