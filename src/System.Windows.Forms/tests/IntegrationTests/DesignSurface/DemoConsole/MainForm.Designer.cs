using DesignSurfaceExt;

namespace TestConsole;

partial class MainForm
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
        System.ComponentModel.ComponentResourceManager resources = new(typeof(MainForm));
        this.splitContainer = new System.Windows.Forms.SplitContainer();
        this.tabControl1 = new System.Windows.Forms.TabControl();
        this.tabPage1 = new System.Windows.Forms.TabPage();
        this.tabPage2 = new System.Windows.Forms.TabPage();
        this.tabPage3 = new System.Windows.Forms.TabPage();
        this.tabPage4 = new System.Windows.Forms.TabPage();
        this.tabPage5 = new System.Windows.Forms.TabPage();
        this.tabPage6 = new System.Windows.Forms.TabPage();
        this.propertyGrid = new PropertyGridExt();
        this.menuStrip1 = new System.Windows.Forms.MenuStrip();
        this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.ToolStripMenuItemUnDo = new System.Windows.Forms.ToolStripMenuItem();
        this.ToolStripMenuItemReDo = new System.Windows.Forms.ToolStripMenuItem();
        this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
        this.ToolStripMenuItemCut = new System.Windows.Forms.ToolStripMenuItem();
        this.ToolStripMenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
        this.ToolStripMenuItemPaste = new System.Windows.Forms.ToolStripMenuItem();
        this.ToolStripMenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
        this.ToolStripMenuItemKeyInvokeSmartTag = new System.Windows.Forms.ToolStripMenuItem();
        this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
        this.toolStripMenuItemTools = new System.Windows.Forms.ToolStripMenuItem();
        this.toolStripMenuItemTabOrder = new System.Windows.Forms.ToolStripMenuItem();
        this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.ToolStripMenuItemAbout = new System.Windows.Forms.ToolStripMenuItem();
        this.splitContainer.Panel1.SuspendLayout();
        this.splitContainer.Panel2.SuspendLayout();
        this.splitContainer.SuspendLayout();
        this.tabControl1.SuspendLayout();
        this.menuStrip1.SuspendLayout();
        this.SuspendLayout();
        // 
        // splitContainer
        // 
        this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
        this.splitContainer.Location = new System.Drawing.Point(0, 28);
        this.splitContainer.Margin = new System.Windows.Forms.Padding(4);
        this.splitContainer.Name = "splitContainer";
        // 
        // splitContainer.Panel1
        // 
        this.splitContainer.Panel1.BackColor = System.Drawing.SystemColors.Window;
        this.splitContainer.Panel1.Controls.Add(this.tabControl1);
        // 
        // splitContainer.Panel2
        // 
        this.splitContainer.Panel2.Controls.Add(this.propertyGrid);
        this.splitContainer.Size = new System.Drawing.Size(824, 502);
        this.splitContainer.SplitterDistance = 593;
        this.splitContainer.SplitterWidth = 5;
        this.splitContainer.TabIndex = 0;
        // 
        // tabControl1
        // 
        this.tabControl1.Controls.Add(this.tabPage1);
        this.tabControl1.Controls.Add(this.tabPage2);
        this.tabControl1.Controls.Add(this.tabPage3);
        this.tabControl1.Controls.Add(this.tabPage4);
        this.tabControl1.Controls.Add(this.tabPage5);
        this.tabControl1.Controls.Add(this.tabPage6);
        this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tabControl1.Location = new System.Drawing.Point(0, 0);
        this.tabControl1.Name = "tabControl1";
        this.tabControl1.SelectedIndex = 0;
        this.tabControl1.Size = new System.Drawing.Size(593, 502);
        this.tabControl1.TabIndex = 0;
        // 
        // tabPage1
        // 
        this.tabPage1.Location = new System.Drawing.Point(4, 25);
        this.tabPage1.Name = "tabPage1";
        this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
        this.tabPage1.Size = new System.Drawing.Size(585, 473);
        this.tabPage1.TabIndex = 0;
        this.tabPage1.Text = "tabPage1";
        this.tabPage1.UseVisualStyleBackColor = true;
        // 
        // tabPage2
        // 
        this.tabPage2.Location = new System.Drawing.Point(4, 25);
        this.tabPage2.Name = "tabPage2";
        this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
        this.tabPage2.Size = new System.Drawing.Size(585, 473);
        this.tabPage2.TabIndex = 1;
        this.tabPage2.Text = "tabPage2";
        this.tabPage2.UseVisualStyleBackColor = true;
        // 
        // tabPage3
        // 
        this.tabPage3.Location = new System.Drawing.Point(4, 25);
        this.tabPage3.Name = "tabPage3";
        this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
        this.tabPage3.Size = new System.Drawing.Size(585, 473);
        this.tabPage3.TabIndex = 2;
        this.tabPage3.Text = "tabPage3";
        this.tabPage3.UseVisualStyleBackColor = true;
        // 
        // tabPage4
        // 
        this.tabPage4.Location = new System.Drawing.Point(4, 25);
        this.tabPage4.Name = "tabPage4";
        this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
        this.tabPage4.Size = new System.Drawing.Size(585, 473);
        this.tabPage4.TabIndex = 3;
        this.tabPage4.Text = "tabPage4";
        this.tabPage4.UseVisualStyleBackColor = true;
        // 
        // tabPage5
        // 
        this.tabPage5.Location = new System.Drawing.Point(4, 25);
        this.tabPage5.Name = "tabPage5";
        this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
        this.tabPage5.Size = new System.Drawing.Size(585, 473);
        this.tabPage5.TabIndex = 3;
        this.tabPage5.Text = "tabPage5";
        this.tabPage5.UseVisualStyleBackColor = true;
        // 
        // tabPage6
        // 
        this.tabPage6.Location = new System.Drawing.Point(4, 25);
        this.tabPage6.Name = "tabPage6";
        this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
        this.tabPage6.Size = new System.Drawing.Size(585, 473);
        this.tabPage6.TabIndex = 3;
        this.tabPage6.Text = "tabPage6";
        this.tabPage6.UseVisualStyleBackColor = true;

        // 
        // menuStrip1
        // 
        this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.editToolStripMenuItem,
        this.toolStripMenuItemTools,
        this.helpToolStripMenuItem});
        this.menuStrip1.Location = new System.Drawing.Point(0, 0);
        this.menuStrip1.Name = "menuStrip1";
        this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
        this.menuStrip1.Size = new System.Drawing.Size(824, 28);
        this.menuStrip1.TabIndex = 1;
        this.menuStrip1.Text = "menuStrip1";
        // 
        // editToolStripMenuItem
        // 
        this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.ToolStripMenuItemUnDo,
        this.ToolStripMenuItemReDo,
        this.toolStripSeparator3,
        this.ToolStripMenuItemCut,
        this.ToolStripMenuItemCopy,
        this.ToolStripMenuItemPaste,
        this.ToolStripMenuItemDelete,
        this.ToolStripMenuItemKeyInvokeSmartTag,
        this.toolStripSeparator4});
        this.editToolStripMenuItem.Name = "editToolStripMenuItem";
        this.editToolStripMenuItem.Size = new System.Drawing.Size(47, 24);
        this.editToolStripMenuItem.Text = "&Edit";
        // 
        // ToolStripMenuItemUnDo
        // 
        this.ToolStripMenuItemUnDo.Name = "ToolStripMenuItemUnDo";
        this.ToolStripMenuItemUnDo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
        this.ToolStripMenuItemUnDo.Size = new System.Drawing.Size(165, 24);
        this.ToolStripMenuItemUnDo.Text = "Undo";
        this.ToolStripMenuItemUnDo.Click += this.undoToolStripMenuItem_Click;
        // 
        // ToolStripMenuItemReDo
        // 
        this.ToolStripMenuItemReDo.Name = "ToolStripMenuItemReDo";
        this.ToolStripMenuItemReDo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
        this.ToolStripMenuItemReDo.Size = new System.Drawing.Size(165, 24);
        this.ToolStripMenuItemReDo.Text = "Redo";
        this.ToolStripMenuItemReDo.Click += this.redoToolStripMenuItem_Click;
        // 
        // toolStripSeparator3
        // 
        this.toolStripSeparator3.Name = "toolStripSeparator3";
        this.toolStripSeparator3.Size = new System.Drawing.Size(162, 6);
        // 
        // ToolStripMenuItemCut
        // 
        this.ToolStripMenuItemCut.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripMenuItemCut.Image")));
        this.ToolStripMenuItemCut.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.ToolStripMenuItemCut.Name = "ToolStripMenuItemCut";
        this.ToolStripMenuItemCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
        this.ToolStripMenuItemCut.Size = new System.Drawing.Size(165, 24);
        this.ToolStripMenuItemCut.Text = "Cut";
        this.ToolStripMenuItemCut.Click += this.OnMenuClick;
        // 
        // ToolStripMenuItemCopy
        // 
        this.ToolStripMenuItemCopy.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripMenuItemCopy.Image")));
        this.ToolStripMenuItemCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.ToolStripMenuItemCopy.Name = "ToolStripMenuItemCopy";
        this.ToolStripMenuItemCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
        this.ToolStripMenuItemCopy.Size = new System.Drawing.Size(165, 24);
        this.ToolStripMenuItemCopy.Text = "Copy";
        this.ToolStripMenuItemCopy.Click += this.OnMenuClick;
        // 
        // ToolStripMenuItemPaste
        // 
        this.ToolStripMenuItemPaste.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripMenuItemPaste.Image")));
        this.ToolStripMenuItemPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.ToolStripMenuItemPaste.Name = "ToolStripMenuItemPaste";
        this.ToolStripMenuItemPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
        this.ToolStripMenuItemPaste.Size = new System.Drawing.Size(165, 24);
        this.ToolStripMenuItemPaste.Text = "Paste";
        this.ToolStripMenuItemPaste.Click += this.OnMenuClick;
        // 
        // ToolStripMenuItemDelete
        // 
        this.ToolStripMenuItemDelete.Name = "ToolStripMenuItemDelete";
        this.ToolStripMenuItemDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
        this.ToolStripMenuItemDelete.Size = new System.Drawing.Size(165, 24);
        this.ToolStripMenuItemDelete.Text = "Delete";
        this.ToolStripMenuItemDelete.Click += this.OnMenuClick;
        // 
        // ToolStripMenuItemKeyInvokeSmartTag
        // 
        this.ToolStripMenuItemKeyInvokeSmartTag.Name = "ToolStripMenuItemKeyInvokeSmartTag";
        this.ToolStripMenuItemKeyInvokeSmartTag.ShortcutKeys = System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F10;
        this.ToolStripMenuItemKeyInvokeSmartTag.Size = new System.Drawing.Size(165, 24);
        this.ToolStripMenuItemKeyInvokeSmartTag.Text = "InvokeSmartTag";
        this.ToolStripMenuItemKeyInvokeSmartTag.Click += this.OnMenuClick;
        // 
        // toolStripSeparator4
        // 
        this.toolStripSeparator4.Name = "toolStripSeparator4";
        this.toolStripSeparator4.Size = new System.Drawing.Size(162, 6);
        // 
        // toolStripMenuItemTools
        // 
        this.toolStripMenuItemTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.toolStripMenuItemTabOrder});
        this.toolStripMenuItemTools.Name = "toolStripMenuItemTools";
        this.toolStripMenuItemTools.Size = new System.Drawing.Size(57, 24);
        this.toolStripMenuItemTools.Text = "&Tools";
        // 
        // toolStripMenuItemTabOrder
        // 
        this.toolStripMenuItemTabOrder.Name = "toolStripMenuItemTabOrder";
        this.toolStripMenuItemTabOrder.Size = new System.Drawing.Size(145, 24);
        this.toolStripMenuItemTabOrder.Text = "Tab Order";
        this.toolStripMenuItemTabOrder.Click += this.toolStripMenuItemTabOrder_Click;
        // 
        // helpToolStripMenuItem
        // 
        this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.ToolStripMenuItemAbout});
        this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
        this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
        this.helpToolStripMenuItem.Text = "&Help";
        // 
        // ToolStripMenuItemAbout
        // 
        this.ToolStripMenuItemAbout.Name = "ToolStripMenuItemAbout";
        this.ToolStripMenuItemAbout.Size = new System.Drawing.Size(128, 24);
        this.ToolStripMenuItemAbout.Text = "About...";
        this.ToolStripMenuItemAbout.Click += this.OnAbout;
        // 
        // MainForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(824, 530);
        this.Controls.Add(this.splitContainer);
        this.Controls.Add(this.menuStrip1);
        this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        this.MainMenuStrip = this.menuStrip1;
        this.Margin = new System.Windows.Forms.Padding(4);
        this.Name = "MainForm";
        this.Text = "Tiny Form Designer";
        this.Load += this.MainForm_Load;
        this.splitContainer.Panel1.ResumeLayout(false);
        this.splitContainer.Panel2.ResumeLayout(false);
        this.splitContainer.ResumeLayout(false);
        this.tabControl1.ResumeLayout(false);
        this.menuStrip1.ResumeLayout(false);
        this.menuStrip1.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer;
    private PropertyGridExt propertyGrid;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemUnDo;
    private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemReDo;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemCut;
    private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemCopy;
    private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemPaste;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemDelete;
    private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemKeyInvokeSmartTag;
    private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemAbout;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.TabPage tabPage3;
    private System.Windows.Forms.TabPage tabPage4;
    private System.Windows.Forms.TabPage tabPage5;
    private System.Windows.Forms.TabPage tabPage6;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemTools;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemTabOrder;

}
