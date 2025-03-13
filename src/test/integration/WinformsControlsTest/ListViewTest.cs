// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public partial class ListViewTest : Form
{
    public ListViewTest()
    {
        InitializeComponent();
        CreateMyListView();

        listView1.LabelEdit = true;
        listView1.View = View.Tile;

        Random random = new();
        int i = random.Next(100, 300);

        listView1.TileSize = new Size(200, 50);
        listView1.Items[0].ImageIndex = 0;
        listView1.Items[1].ImageIndex = 1;
        listView1.Items[2].ImageIndex = 2;
        listView1.Click += (s, e) =>
        {
            // listView1.TileSize = new Size(random.Next(100, 300), random.Next(25, 50));

            Point pos = Cursor.Position;
            pos = PointToClient(pos);
            int index = listView1.InsertionMark.NearestIndex(pos);
            Console.WriteLine($"nearest index: {index}");
        };

        AddCollapsibleGroupToListView();
        AddGroupTasks();

        // Manual test for https://github.com/dotnet/winforms/issues/11658
        string[] TestItems = ["Item 1", "Item 2", "Item 3"];
        listView3.RetrieveVirtualItem += (s, e) =>
        {
            e.Item = e.ItemIndex switch
            {
                0 => new ListViewItem(TestItems[0]),
                1 => new ListViewItem(TestItems[1]),
                2 => new ListViewItem(TestItems[2]),
                _ => throw new ArgumentOutOfRangeException(),
            };
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
            Sorting = SortOrder.Ascending,

            VirtualMode = true,
            VirtualListSize = 3,
        };

        listView2.SelectedIndexChanged += listView2_SelectedIndexChanged;
        listView2.Click += listView2_Click;

        // Create three items and three sets of subitems for each item.
        ListViewItem item1 = new("item1", 0)
        {
            // Place a check mark next to the item.
            Checked = true
        };
        item1.SubItems.Add("1");
        item1.SubItems.Add("2");
        item1.SubItems.Add("3");
        ListViewItem item2 = new("item2", 1);
        item2.SubItems.Add("4");
        item2.SubItems.Add("5");
        item2.SubItems.Add("6");
        ListViewItem item3 = new("item3")
        {
            // Place a check mark next to the item.
            Checked = true
        };
        item3.SubItems.Add("7");
        item3.SubItems.Add("8");
        item3.SubItems.Add("9");

        // Add the items to the ListView, but because the listview is in Virtual Mode, we have to manage items ourselves
        // and thus, we can't call the following:
        //      listView2.Items.AddRange(new ListViewItem[] { item1, item2, item3 });
        listView2.RetrieveVirtualItem += (s, e) =>
        {
            e.Item = e.ItemIndex switch
            {
                0 => item1,
                1 => item2,
                2 => item3,
                _ => throw new ArgumentOutOfRangeException(),
            };
        };

        // Create columns for the items and subitems.
        // Width of -2 indicates auto-size.
        listView2.Columns.Add("column1", "Item Column", -2, HorizontalAlignment.Left, 0);
        listView2.Columns.Add("Column 2", -2, HorizontalAlignment.Left);
        listView2.Columns.Add("Column 3", -2, HorizontalAlignment.Left);
        listView2.Columns.Add("Column 4", -2, HorizontalAlignment.Center);
        listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

        // Create two ImageList objects.
        ImageList imageListSmall = new(components);
        ImageList imageListLarge = new(components);

        // Initialize the ImageList objects with bitmaps.
        imageListSmall.Images.Add(Image.FromFile("Images\\SmallA.bmp"));
        imageListSmall.Images.Add(Image.FromFile("Images\\SmallABlue.bmp"));
        imageListLarge.Images.Add(Image.FromFile("Images\\LargeA.bmp"));
        imageListLarge.Images.Add(Image.FromFile("Images\\LargeABlue.bmp"));

        // Assign the ImageList objects to the ListView.
        listView2.LargeImageList = imageListLarge;
        listView2.SmallImageList = imageListSmall;

        // Add the ListView to the control collection.
        Controls.Add(listView2);
        listView2.Dock = DockStyle.Bottom;
    }

    private void AddCollapsibleGroupToListView()
    {
        ListViewGroup lvgroup1 = new()
        {
            Header = "CollapsibleGroup1",
            CollapsedState = ListViewGroupCollapsedState.Expanded
        };

        listView1.Groups.Add(lvgroup1);
        listView1.Items.Add(new ListViewItem
        {
            Text = "Item4",
            Group = lvgroup1
        });

        ListViewGroup lvgroup2 = new()
        {
            Header = "CollapsibleGroup2",
            CollapsedState = ListViewGroupCollapsedState.Collapsed
        };

        listView1.Groups.Add(lvgroup2);
        listView1.Items.Add(new ListViewItem
        {
            Text = "Item5",
            Group = lvgroup2
        });

        listView1.GroupCollapsedStateChanged += listView1_GroupCollapsedStateChanged;
    }

    private void listView1_GroupCollapsedStateChanged(object sender, ListViewGroupEventArgs e)
    {
        MessageBox.Show($"CollapsedState changed at group with index {e.GroupIndex}");
    }

    private void AddGroupTasks()
    {
        listView1.Groups[0].TaskLink = "Task";
        listView1.Groups[0].Subtitle = "Subtitle";
        listView1.GroupTaskLinkClick += listView1_GroupTaskLinkClick;

        ListViewGroup lvgroup1 = new()
        {
            Header = "TaskGroup",
            TaskLink = "Task2"
        };

        listView1.Groups.Add(lvgroup1);
        listView1.Items.Add(new ListViewItem
        {
            Text = "Item6",
            Group = lvgroup1
        });
    }

    private void listView1_GroupTaskLinkClick(object sender, ListViewGroupEventArgs e)
    {
        MessageBox.Show(this, $"Task at group index {e.GroupIndex} was clicked", "GroupClick Event");
    }

    private void listView2_Click(object sender, EventArgs e)
    {
        Debug.WriteLine(listView1.TileSize);
        MessageBox.Show(this, "listView2_Click", "event");
    }

    private void listView3_Click(object sender, EventArgs e)
    {
        var item = ((ListView)sender).FocusedItem;
        var clone = (ListViewItem)item.Clone();
        clone.Checked = true;
        listView3.InvokeOnItemChecked(new ItemCheckedEventArgs(clone));
        MessageBox.Show(this, $"Click {clone.Text} in ListView3", "Click Event");
    }

    private void listView2_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (sender is not ListView listView2)
        {
            return;
        }

        Random random = new();
        listView2.Columns[random.Next(0, listView2.Columns.Count)].ImageIndex = random.Next(0, 2);
    }

    private void btnClearListView1_Click(object sender, EventArgs e)
    {
        listView1.Clear();
        LargeImageList.Images.Clear();

        listView1.LargeImageList = LargeImageList;
        listView1.View = View.LargeIcon;
    }

    private void btnLoadImagesListView1_Click(object sender, EventArgs e)
    {
        if (openFileDialog1.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        foreach (string file in openFileDialog1.FileNames)
        {
            Bitmap bitmap = (Bitmap)Image.FromFile(file);
            LargeImageList.Images.Add(file, bitmap);

            ListViewItem item = new ListViewItem
            {
                Text = Path.GetFileName(file),
                Name = file,
                ImageKey = file,
                Checked = true
            };
            listView1.Items.Add(item);
        }
    }

    private void btnReplaceImageListView1_Click(object sender, EventArgs e)
    {
        if (listView1.SelectedIndices.Count != 1)
        {
            return;
        }

        openFileDialog1.Multiselect = false;
        DialogResult result = openFileDialog1.ShowDialog();
        openFileDialog1.Multiselect = true;

        if (result != DialogResult.OK)
        {
            return;
        }

        string file = openFileDialog1.FileName;
        Bitmap bitmap = (Bitmap)Image.FromFile(file);
        LargeImageList.Images[listView1.SelectedIndices[0]] = bitmap;

        listView1.Refresh();
    }
}
