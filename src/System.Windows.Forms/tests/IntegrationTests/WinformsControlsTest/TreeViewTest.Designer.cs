// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WinformsControlsTest
{
    partial class TreeViewTest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TreeViewTest));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.treeView1_TreeNode1 = new System.Windows.Forms.TreeNode();
            this.treeView1_TreeNode1_ChildNode1 = new System.Windows.Forms.TreeNode();
            this.treeView1_TreeNode1_ChildNode2 = new System.Windows.Forms.TreeNode();
            this.treeView1_TreeNode2 = new System.Windows.Forms.TreeNode();
            this.treeView1_TreeNode3 = new System.Windows.Forms.TreeNode();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.CheckBoxes = true;
            resources.ApplyResources(this.treeView1, "treeView1");
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Name = "treeView1";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            this.treeView1_TreeNode1,
            this.treeView1_TreeNode2,
            this.treeView1_TreeNode3});
            this.treeView1.ShowNodeToolTips = true;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "SmallA.bmp");
            this.imageList1.Images.SetKeyName(1, "SmallABlue.bmp");
            //
            // treeView1_TreeNode1_ChildNode1
            //
            this.treeView1_TreeNode1_ChildNode1.Name = "ChildNode1";
            this.treeView1_TreeNode1_ChildNode1.Text = "ChildNode1";
            this.treeView1_TreeNode1_ChildNode1.ToolTipText = "childnode1 tool tip text";
            //
            // treeView1_TreeNode1_ChildNode2
            //
            this.treeView1_TreeNode1_ChildNode2.Name = "ChildNode2";
            this.treeView1_TreeNode1_ChildNode2.Text = "ChildNode2";
            //
            // treeView1_TreeNode1
            //
            this.treeView1_TreeNode1.Name = "Node1";
            this.treeView1_TreeNode1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
                treeView1_TreeNode1_ChildNode1, treeView1_TreeNode1_ChildNode2 });
            this.treeView1_TreeNode1.Text = "Node1";
            //
            // treeView1_TreeNode2
            //
            this.treeView1_TreeNode2.BackColor = System.Drawing.Color.FromArgb(255, 255, 255, 128);
            this.treeView1_TreeNode2.Checked = false;
            this.treeView1_TreeNode2.ForeColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Blue);
            this.treeView1_TreeNode2.Name = "Node2";
            this.treeView1_TreeNode2.Text = "Node2";
            //
            // treeView1_TreeNode3
            //
            this.treeView1_TreeNode3.Checked = true;
            this.treeView1_TreeNode3.NodeFont = new System.Drawing.Font("Microsoft Tai Le", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.treeView1_TreeNode3.Name = "Node3";
            this.treeView1_TreeNode3.Text = "Node3";
            this.treeView1_TreeNode3.ToolTipText = "node3 tool tip text";
            // 
            // TreeViewTest
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb((int)(byte)0, (int)(byte)192, (int)(byte)0);
            this.Controls.Add(this.treeView1);
            this.Name = "TreeViewTest";
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TreeNode treeView1_TreeNode1;
        private System.Windows.Forms.TreeNode treeView1_TreeNode1_ChildNode1;
        private System.Windows.Forms.TreeNode treeView1_TreeNode1_ChildNode2;
        private System.Windows.Forms.TreeNode treeView1_TreeNode2;
        private System.Windows.Forms.TreeNode treeView1_TreeNode3;
    }
}
