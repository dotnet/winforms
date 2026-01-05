// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace ScratchProject;

// As we can't currently design in VS in the runtime solution, mark as "Default" so this opens in code view by default.
[DesignerCategory("Default")]
public partial class Form1 : Form
{
    private TreeView treeView;
    private ImageList imageList;

    public Form1()
    {
        InitializeComponent();
        SetupTreeView();
    }

    private void SetupTreeView()
    {
        // Create ImageList with simple colored icons
        imageList = new ImageList
        {
            ImageSize = new Size(16, 16),
            ColorDepth = ColorDepth.Depth32Bit
        };

        // Create simple colored bitmaps for testing
        for (int i = 0; i < 3; i++)
        {
            Bitmap bmp = new(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Color color = i switch
                {
                    0 => Color.Green,
                    1 => Color.Blue,
                    _ => Color.Red
                };
                using var brush = new SolidBrush(color);
                g.FillEllipse(brush, 0, 0, 15, 15);
            }
            imageList.Images.Add(bmp);
        }

        // Create TreeView
        treeView = new TreeView
        {
            Dock = DockStyle.Fill,
            ImageList = imageList
        };

        // Add nodes with icons
        TreeNode node0 = new("Node0") { ImageIndex = 0, SelectedImageIndex = 0 };
        TreeNode node1 = new("Node1") { ImageIndex = 1, SelectedImageIndex = 1 };
        TreeNode node4 = new("Node4") { ImageIndex = 2, SelectedImageIndex = 2 };

        treeView.Nodes.Add(node0);
        treeView.Nodes.Add(node1);
        treeView.Nodes.Add(node4);

        Controls.Add(treeView);
    }
}
