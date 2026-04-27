// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.UITests;

public class ToolStripTests : ControlTestBase
{
    private readonly ImageList _sharedImageList;

    public ToolStripTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
        _sharedImageList = new ImageList();
        _sharedImageList.Images.Add("square", Image.FromFile("./Resources/image.png"));
    }

    // Regression test for https://github.com/dotnet/winforms/issues/14489
    [WinFormsFact]
    public async Task ToolStrip_TabFromTreeView_ThenBack_ArrowKeysStillGoToTreeView()
    {
        await RunFormAsync(
            () =>
            {
                Form form = new()
                {
                    TopMost = true
                };

                TreeView treeView = new()
                {
                    Dock = DockStyle.Fill,
                    TabIndex = 0,
                    HideSelection = false
                };
                treeView.Nodes.Add("Node 1");
                treeView.Nodes.Add("Node 2");
                treeView.SelectedNode = treeView.Nodes[0];

                ToolStrip toolStrip = new()
                {
                    Dock = DockStyle.Top,
                    TabStop = true,
                    TabIndex = 1
                };
                toolStrip.Items.Add(new ToolStripButton("Button"));

                form.Controls.Add(treeView);
                form.Controls.Add(toolStrip);

                return (form, (treeView, toolStrip));
            },
            async (form, controls) =>
            {
                var (treeView, toolStrip) = controls;

                treeView.Focus();
                await WaitForIdleAsync();
                Assert.True(treeView.Focused);
                Assert.Equal(treeView.Nodes[0], treeView.SelectedNode);

                Assert.True(form.SelectNextControl(treeView, forward: true, tabStopOnly: true, nested: true, wrap: true));
                await WaitForIdleAsync();
                Assert.True(toolStrip.Focused);

                Assert.True(form.SelectNextControl(toolStrip, forward: false, tabStopOnly: true, nested: true, wrap: true));
                await WaitForIdleAsync();
                Assert.True(treeView.Focused);

                treeView.Focus();
                await WaitForIdleAsync();
                Assert.True(treeView.Focused);
                Assert.Same(treeView, form.ActiveControl);
                Assert.Equal(treeView.Nodes[0], treeView.SelectedNode);
            });
    }

    // Regression test for https://github.com/dotnet/winforms/issues/6881
    [WinFormsFact]
    public void ToolStrip_shared_imagelist_should_not_get_disposed_when_toolstrip_does()
    {
        ShowForm(_sharedImageList);
        ShowForm(_sharedImageList);

        return;

        static void ShowForm(ImageList sharedImageList)
        {
            using Form form = new();
            using ToolStripButton toolStripButton = new()
            {
                ImageKey = "square",
                ToolTipText = "Random Button"
            };
            using ToolStrip toolStrip = new()
            {
                ImageList = sharedImageList
            };
            toolStrip.Items.Add(toolStripButton);

            form.Controls.Add(toolStrip);

            form.Show();

            Assert.NotNull(sharedImageList);
#if DEBUG
            Assert.False(sharedImageList.IsDisposed);
#endif
            Assert.Single(sharedImageList.Images);

            form.Close();

            Assert.NotNull(sharedImageList);
#if DEBUG
            Assert.False(sharedImageList.IsDisposed);
#endif
            Assert.Single(sharedImageList.Images);
        }
    }

    // Regression test for https://github.com/dotnet/winforms/issues/7884
    [WinFormsFact]
    public void ToolStrip_Hiding_ToolStripMenuItem_OnDropDownClosed_ShouldNotThrow()
    {
        using Form form = new();

        using ToolStripMenuItem menu1 = new("Menu1");
        using ToolStripMenuItem menu1Item1 = new("Item1");
        menu1.DropDownItems.Add(menu1Item1);

        using ToolStripMenuItem hiddenMenu = new("hiddenMenu");
        using ToolStripMenuItem hiddenMenuItem1 = new("hiddenMenuItem1");
        hiddenMenu.DropDownItems.Add(hiddenMenuItem1);
        hiddenMenu.DropDownClosed += (object? sender, EventArgs e) => { hiddenMenu.Visible = false; };

        using MenuStrip menuStrip = new();
        menuStrip.Items.Add(menu1);
        menuStrip.Items.Add(hiddenMenu);

        form.Controls.Add(menuStrip);
        form.MainMenuStrip = menuStrip;
        form.Show();
        hiddenMenu.ShowDropDown();
        menu1.Select();
    }
}
