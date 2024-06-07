// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace WinFormsControlsTest
{
    partial class ScrollableControls
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            TreeNode treeNode1 = new TreeNode("Node8");
            TreeNode treeNode2 = new TreeNode("Node7", new TreeNode[] { treeNode1 });
            TreeNode treeNode3 = new TreeNode("Node6", new TreeNode[] { treeNode2 });
            TreeNode treeNode4 = new TreeNode("Node5", new TreeNode[] { treeNode3 });
            TreeNode treeNode5 = new TreeNode("Node4", new TreeNode[] { treeNode4 });
            TreeNode treeNode6 = new TreeNode("Node3", new TreeNode[] { treeNode5 });
            TreeNode treeNode7 = new TreeNode("Node2", new TreeNode[] { treeNode6 });
            TreeNode treeNode8 = new TreeNode("Node1", new TreeNode[] { treeNode7 });
            TreeNode treeNode9 = new TreeNode("Node0", new TreeNode[] { treeNode8 });
            flowLayoutPanel1 = new FlowLayoutPanel();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            label1 = new Label();
            panel1 = new Panel();
            button9 = new Button();
            button8 = new Button();
            button7 = new Button();
            button6 = new Button();
            label2 = new Label();
            splitContainer1 = new SplitContainer();
            button16 = new Button();
            button15 = new Button();
            button10 = new Button();
            button14 = new Button();
            button13 = new Button();
            button12 = new Button();
            button11 = new Button();
            label3 = new Label();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            button17 = new Button();
            tabPage2 = new TabPage();
            label4 = new Label();
            label5 = new Label();
            textBox1 = new TextBox();
            treeView1 = new TreeView();
            label6 = new Label();
            label7 = new Label();
            listBox1 = new ListBox();
            flowLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.BorderStyle = BorderStyle.FixedSingle;
            flowLayoutPanel1.Controls.Add(button1);
            flowLayoutPanel1.Controls.Add(button2);
            flowLayoutPanel1.Controls.Add(button3);
            flowLayoutPanel1.Controls.Add(button4);
            flowLayoutPanel1.Controls.Add(button5);
            flowLayoutPanel1.Location = new Point(12, 33);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(97, 93);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(3, 3);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(3, 32);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 1;
            button2.Text = "button2";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(3, 61);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 2;
            button3.Text = "button3";
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Location = new Point(3, 90);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 3;
            button4.Text = "button4";
            button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.Location = new Point(3, 119);
            button5.Name = "button5";
            button5.Size = new Size(75, 23);
            button5.TabIndex = 4;
            button5.Text = "button5";
            button5.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(97, 15);
            label1.TabIndex = 1;
            label1.Text = "FlowLayoutPanel";
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(button9);
            panel1.Controls.Add(button8);
            panel1.Controls.Add(button7);
            panel1.Controls.Add(button6);
            panel1.Location = new Point(166, 37);
            panel1.Name = "panel1";
            panel1.Size = new Size(120, 89);
            panel1.TabIndex = 2;
            // 
            // button9
            // 
            button9.Location = new Point(24, 113);
            button9.Name = "button9";
            button9.Size = new Size(75, 23);
            button9.TabIndex = 3;
            button9.Text = "button9";
            button9.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            button8.Location = new Point(24, 87);
            button8.Name = "button8";
            button8.Size = new Size(113, 23);
            button8.TabIndex = 2;
            button8.Text = "button8";
            button8.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            button7.Location = new Point(3, 29);
            button7.Name = "button7";
            button7.Size = new Size(113, 23);
            button7.TabIndex = 1;
            button7.Text = "button7";
            button7.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            button6.Location = new Point(3, 3);
            button6.Name = "button6";
            button6.Size = new Size(113, 20);
            button6.TabIndex = 0;
            button6.Text = "button6";
            button6.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(200, 9);
            label2.Name = "label2";
            label2.Size = new Size(36, 15);
            label2.TabIndex = 3;
            label2.Text = "Panel";
            // 
            // splitContainer1
            // 
            splitContainer1.BorderStyle = BorderStyle.FixedSingle;
            splitContainer1.Location = new Point(344, 40);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.AutoScroll = true;
            splitContainer1.Panel1.Controls.Add(button16);
            splitContainer1.Panel1.Controls.Add(button15);
            splitContainer1.Panel1.Controls.Add(button10);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.AutoScroll = true;
            splitContainer1.Panel2.Controls.Add(button14);
            splitContainer1.Panel2.Controls.Add(button13);
            splitContainer1.Panel2.Controls.Add(button12);
            splitContainer1.Panel2.Controls.Add(button11);
            splitContainer1.Size = new Size(163, 108);
            splitContainer1.SplitterDistance = 54;
            splitContainer1.TabIndex = 4;
            // 
            // button16
            // 
            button16.Location = new Point(3, 87);
            button16.Name = "button16";
            button16.Size = new Size(113, 20);
            button16.TabIndex = 3;
            button16.Text = "button16";
            button16.UseVisualStyleBackColor = true;
            // 
            // button15
            // 
            button15.Location = new Point(-11, 61);
            button15.Name = "button15";
            button15.Size = new Size(113, 20);
            button15.TabIndex = 2;
            button15.Text = "button15";
            button15.UseVisualStyleBackColor = true;
            // 
            // button10
            // 
            button10.Location = new Point(-31, 31);
            button10.Name = "button10";
            button10.Size = new Size(113, 20);
            button10.TabIndex = 1;
            button10.Text = "button10";
            button10.UseVisualStyleBackColor = true;
            // 
            // button14
            // 
            button14.Location = new Point(62, 112);
            button14.Name = "button14";
            button14.Size = new Size(113, 20);
            button14.TabIndex = 4;
            button14.Text = "button14";
            button14.UseVisualStyleBackColor = true;
            // 
            // button13
            // 
            button13.Location = new Point(48, 87);
            button13.Name = "button13";
            button13.Size = new Size(113, 20);
            button13.TabIndex = 3;
            button13.Text = "button13";
            button13.UseVisualStyleBackColor = true;
            // 
            // button12
            // 
            button12.Location = new Point(31, 61);
            button12.Name = "button12";
            button12.Size = new Size(113, 20);
            button12.TabIndex = 2;
            button12.Text = "button12";
            button12.UseVisualStyleBackColor = true;
            // 
            // button11
            // 
            button11.Location = new Point(16, 31);
            button11.Name = "button11";
            button11.Size = new Size(113, 20);
            button11.TabIndex = 1;
            button11.Text = "button11";
            button11.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(371, 4);
            label3.Name = "label3";
            label3.Size = new Size(82, 15);
            label3.TabIndex = 5;
            label3.Text = "SplitContainer";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(538, 47);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(167, 101);
            tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            tabPage1.AutoScroll = true;
            tabPage1.Controls.Add(button17);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(159, 73);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // button17
            // 
            button17.Location = new Point(11, 24);
            button17.Name = "button17";
            button17.Size = new Size(194, 74);
            button17.TabIndex = 2;
            button17.Text = "button17";
            button17.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(159, 73);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "tabPage2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(593, 9);
            label4.Name = "label4";
            label4.Size = new Size(65, 15);
            label4.TabIndex = 7;
            label4.Text = "TabControl";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(29, 193);
            label5.Name = "label5";
            label5.Size = new Size(48, 15);
            label5.TabIndex = 8;
            label5.Text = "TextBox";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(16, 247);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            textBox1.Size = new Size(93, 65);
            textBox1.TabIndex = 9;
            textBox1.Text = "zdffsdfsd\r\nfsdfsdfss\r\ndfsdfsdfs\r\ndfsdfsdf\r\nsdfsdfd\r\nfsdfsdfsnfsdfsdfs";
            textBox1.WordWrap = false;
            // 
            // treeView1
            // 
            treeView1.Location = new Point(170, 229);
            treeView1.Name = "treeView1";
            treeNode1.Name = "Node8";
            treeNode1.Text = "Node8";
            treeNode2.Name = "Node7";
            treeNode2.Text = "Node7";
            treeNode3.Name = "Node6";
            treeNode3.Text = "Node6";
            treeNode4.Name = "Node5";
            treeNode4.Text = "Node5";
            treeNode5.Name = "Node4";
            treeNode5.Text = "Node4";
            treeNode6.Name = "Node3";
            treeNode6.Text = "Node3";
            treeNode7.Name = "Node2";
            treeNode7.Text = "Node2";
            treeNode8.Name = "Node1";
            treeNode8.Text = "Node1";
            treeNode9.Name = "Node0";
            treeNode9.Text = "Node0";
            treeView1.Nodes.AddRange(new TreeNode[] { treeNode9 });
            treeView1.Size = new Size(115, 83);
            treeView1.TabIndex = 10;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(181, 193);
            label6.Name = "label6";
            label6.Size = new Size(55, 15);
            label6.TabIndex = 11;
            label6.Text = "Treeview ";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(389, 193);
            label7.Name = "label7";
            label7.Size = new Size(45, 15);
            label7.TabIndex = 12;
            label7.Text = "ListBox";
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Items.AddRange(new object[] { "asdfasdf", "as", "df", "asd", "f", "a", "sd", "f", "asd", "f", "a", "asdfadsf", "sdf" });
            listBox1.Location = new Point(348, 229);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(113, 79);
            listBox1.TabIndex = 13;
            // 
            // ScrollableControls
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(759, 422);
            Controls.Add(listBox1);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(treeView1);
            Controls.Add(textBox1);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(tabControl1);
            Controls.Add(label3);
            Controls.Add(splitContainer1);
            Controls.Add(label2);
            Controls.Add(panel1);
            Controls.Add(label1);
            Controls.Add(flowLayoutPanel1);
            Name = "ScrollableControls";
            Text = "ScrollableControls";
            flowLayoutPanel1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private Button button1;
        private Button button2;
        private Button button4;
        private Button button5;
        private Label label1;
        private Button button3;
        private Panel panel1;
        private Button button9;
        private Button button8;
        private Button button7;
        private Button button6;
        private Label label2;
        private SplitContainer splitContainer1;
        private Button button10;
        private Button button11;
        private Label label3;
        private Button button16;
        private Button button15;
        private Button button14;
        private Button button13;
        private Button button12;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private Button button17;
        private TabPage tabPage2;
        private Label label4;
        private Label label5;
        private TextBox textBox1;
        private TreeView treeView1;
        private Label label6;
        private Label label7;
        private ListBox listBox1;
    }
}
