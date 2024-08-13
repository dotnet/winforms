// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

partial class ListViewTest
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new(typeof(ListViewTest));
        this.imageList1 = new System.Windows.Forms.ImageList(this.components);
        this.listView1 = new System.Windows.Forms.ListView();
        this.listView3 = new CloneTestListView();
        this.columnHeader1 = (System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader());
        this.columnHeader2 = (System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader());
        this.imageList2 = new System.Windows.Forms.ImageList(this.components);
        this.btnClearListView1 = new System.Windows.Forms.Button();
        this.btnLoadImagesListView1 = new System.Windows.Forms.Button();
        this.LargeImageList = new System.Windows.Forms.ImageList(this.components);
        this.btnReplaceImageListView1 = new System.Windows.Forms.Button();
        this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
        this.SuspendLayout();
        // 
        // imageList1
        // 
        this.imageList1.ImageStream = (System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream"));
        this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
        this.imageList1.Images.SetKeyName(0, "SmallA.bmp");
        this.imageList1.Images.SetKeyName(1, "SmallABlue.bmp");
        // 
        // columnHeader1
        // 
        this.columnHeader1.ImageIndex = 0;
        // 
        // columnHeader2
        // 
        this.columnHeader2.ImageIndex = 1;
        // 
        // listView1
        //
        this.listView1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
        this.columnHeader1,
        this.columnHeader2});
        this.listView1.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
        (System.Windows.Forms.ListViewGroup)(resources.GetObject("listView1.Groups"))});
        this.listView1.HideSelection = false;
        this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
        (System.Windows.Forms.ListViewItem)(resources.GetObject("listView1.Items")),
        (System.Windows.Forms.ListViewItem)(resources.GetObject("listView1.Items1")),
        (System.Windows.Forms.ListViewItem)(resources.GetObject("listView1.Items2"))});
        this.listView1.LargeImageList = this.imageList2;
        resources.ApplyResources(this.listView1, "listView1");
        this.listView1.Location = new System.Drawing.Point(12, 33);
        this.listView1.Name = "listView1";
        this.listView1.Size = new System.Drawing.Size(439, 159);
        this.listView1.SmallImageList = this.imageList1;
        this.listView1.TabIndex = 0;
        this.listView1.UseCompatibleStateImageBehavior = false;
        this.listView1.Dock = DockStyle.Fill;
        // 
        // listView3
        // 
        this.listView3.CheckBoxes = true;
        this.listView3.HideSelection = false;
        this.listView3.Name = "listView1";
        this.listView3.TabIndex = 1;
        this.listView3.UseCompatibleStateImageBehavior = false;
        this.listView3.VirtualListSize = 3;
        this.listView3.VirtualMode = true;
        this.listView1.Size = new System.Drawing.Size(439, 40);
        this.listView3.Dock = DockStyle.Bottom;
        this.listView3.Click += new System.EventHandler(this.listView3_Click);
        // 
        // imageList2
        // 
        this.imageList2.ImageStream = (System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream"));
        this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
        this.imageList2.Images.SetKeyName(0, "LargeA.bmp");
        this.imageList2.Images.SetKeyName(1, "LargeABlue.bmp");
        // 
        // btnClearListView1
        // 
        this.btnClearListView1.Location = new System.Drawing.Point(13, 4);
        this.btnClearListView1.Name = "btnClearListView1";
        this.btnClearListView1.Size = new System.Drawing.Size(75, 23);
        this.btnClearListView1.TabIndex = 1;
        this.btnClearListView1.Text = "Clear";
        this.btnClearListView1.UseVisualStyleBackColor = true;
        this.btnClearListView1.Click += this.btnClearListView1_Click;
        // 
        // btnLoadImagesListView1
        // 
        this.btnLoadImagesListView1.Location = new System.Drawing.Point(95, 4);
        this.btnLoadImagesListView1.Name = "btnLoadImagesListView1";
        this.btnLoadImagesListView1.Size = new System.Drawing.Size(75, 23);
        this.btnLoadImagesListView1.TabIndex = 2;
        this.btnLoadImagesListView1.Text = "Load images";
        this.btnLoadImagesListView1.UseVisualStyleBackColor = true;
        this.btnLoadImagesListView1.Click += this.btnLoadImagesListView1_Click;
        // 
        // LargeImageList
        // 
        this.LargeImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
        this.LargeImageList.ImageSize = new System.Drawing.Size(256, 256);
        this.LargeImageList.TransparentColor = System.Drawing.Color.Transparent;
        // 
        // openFileDialog1
        // 
        this.openFileDialog1.FileName = "openFileDialog1";
        this.openFileDialog1.Multiselect = true;
        this.openFileDialog1.SupportMultiDottedExtensions = true;
        // 
        // btnReplaceImageListView1
        // 
        this.btnReplaceImageListView1.Location = new System.Drawing.Point(176, 4);
        this.btnReplaceImageListView1.Name = "btnReplaceImageListView1";
        this.btnReplaceImageListView1.Size = new System.Drawing.Size(87, 23);
        this.btnReplaceImageListView1.TabIndex = 3;
        this.btnReplaceImageListView1.Text = "Replace image";
        this.btnReplaceImageListView1.UseVisualStyleBackColor = true;
        this.btnReplaceImageListView1.Click += this.btnReplaceImageListView1_Click;
        // 
        // ListViewTest
        // 
        resources.ApplyResources(this, "$this");
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(this.btnReplaceImageListView1);
        this.Controls.Add(this.btnLoadImagesListView1);
        this.Controls.Add(this.btnClearListView1);
        this.Controls.Add(this.listView1);
        this.Controls.Add(this.listView3);
        this.Name = "ListViewTest";
        this.Text = "ListView Test";
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.ListView listView1;
    private CloneTestListView listView3;
    private System.Windows.Forms.ImageList imageList1;
    private System.Windows.Forms.ImageList imageList2;
    private System.Windows.Forms.ImageList LargeImageList;
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private System.Windows.Forms.ColumnHeader columnHeader2;
    private System.Windows.Forms.Button btnClearListView1;
    private System.Windows.Forms.Button btnLoadImagesListView1;
    private System.Windows.Forms.Button btnReplaceImageListView1;
    private System.Windows.Forms.OpenFileDialog openFileDialog1;
}
