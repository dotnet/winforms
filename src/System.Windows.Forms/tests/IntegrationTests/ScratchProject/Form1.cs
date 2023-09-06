// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace ScratchProject;

// As we can't currently design in VS in the runtime solution, mark as "Default" so this opens in code view by default.
[DesignerCategory("Default")]
public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        //ImageList imageListSmall = new ImageList();
        //ImageList imageListLarge = new ImageList();
        //imageListSmall.Images.Add(Bitmap.FromFile("C:\\school\\mySmallImage1.bmp"));
        //imageListSmall.Images.Add(Bitmap.FromFile("C:\\school\\mySmallImage2.bmp"));
        //imageListLarge.Images.Add(Bitmap.FromFile("C:\\school\\myLargeImage1.bmp"));
        //imageListLarge.Images.Add(Bitmap.FromFile("C:\\school\\myLargeImage2.bmp"));
        //listView1.LargeImageList = imageListLarge;
        //listView1.SmallImageList = imageListSmall;
        listView1.Groups[0].Items.Add(listView1.Items[0]);
        listView1.Groups[1].Items.AddRange(new ListViewItem[] { listView1.Items[1], listView1.Items[2] });
    }
}
