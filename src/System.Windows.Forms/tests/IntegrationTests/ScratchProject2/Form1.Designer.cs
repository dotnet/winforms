namespace ScratchProject2
{
    partial class Form1
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
            TreeNode treeNode1 = new TreeNode("Node1");
            TreeNode treeNode2 = new TreeNode("Node0", new TreeNode[] { treeNode1 });
            TreeNode treeNode3 = new TreeNode("Node3");
            TreeNode treeNode4 = new TreeNode("Node4");
            TreeNode treeNode5 = new TreeNode("Node5");
            TreeNode treeNode6 = new TreeNode("Node2", new TreeNode[] { treeNode3, treeNode4, treeNode5 });
            TreeNode treeNode7 = new TreeNode("Node6");
            ListViewItem listViewItem1 = new ListViewItem(new string[] { "1", "q", "w", "e" }, -1);
            ListViewItem listViewItem2 = new ListViewItem(new string[] { "2", "a", "s", "d" }, -1);
            ListViewItem listViewItem3 = new ListViewItem(new string[] { "3", "z", "x", "c" }, -1);
            treeView1 = new TreeView();
            listView1 = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            SuspendLayout();
            // 
            // treeView1
            // 
            treeView1.LabelEdit = true;
            treeView1.Location = new Point(24, 59);
            treeView1.Name = "treeView1";
            treeNode1.Name = "Node1";
            treeNode1.Text = "Node1";
            treeNode2.Name = "Node0";
            treeNode2.Text = "Node0";
            treeNode3.Name = "Node3";
            treeNode3.Text = "Node3";
            treeNode4.Name = "Node4";
            treeNode4.Text = "Node4";
            treeNode5.Name = "Node5";
            treeNode5.Text = "Node5";
            treeNode6.Name = "Node2";
            treeNode6.Text = "Node2";
            treeNode7.Name = "Node6";
            treeNode7.Text = "Node6";
            treeView1.Nodes.AddRange(new TreeNode[] { treeNode2, treeNode6, treeNode7 });
            treeView1.Size = new Size(161, 199);
            treeView1.TabIndex = 0;

            
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });
            listView1.Items.AddRange(new ListViewItem[] { listViewItem1, listViewItem2, listViewItem3 });
            listView1.LabelEdit = true;
            listView1.Location = new Point(254, 61);
            listView1.Name = "listView1";
            listView1.Size = new Size(159, 160);
            listView1.TabIndex = 1;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(591, 334);
            Controls.Add(listView1);
            Controls.Add(treeView1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private TreeView treeView1;
        private ListView listView1;
        
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
    }
}
