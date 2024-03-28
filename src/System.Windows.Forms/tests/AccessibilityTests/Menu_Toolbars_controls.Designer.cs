using System.Drawing;
using System.Windows.Forms;

namespace Accessibility_Core_App;

partial class Menu_Toolbars_controls
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
        components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new(typeof(Menu_Toolbars_controls));
        menuStrip1 = new MenuStrip();
        contextMenuStrip1 = new ContextMenuStrip(components);
        menuStripToolStripMenuItem = new ToolStripMenuItem();
        item1ToolStripMenuItem = new ToolStripMenuItem();
        shortCutsToolStripMenuItem = new ToolStripMenuItem();
        item2ToolStripMenuItem = new ToolStripMenuItem();
        fileToolStripMenuItem = new ToolStripMenuItem();
        newToolStripMenuItem = new ToolStripMenuItem();
        openToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparator = new ToolStripSeparator();
        saveToolStripMenuItem = new ToolStripMenuItem();
        saveAsToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparator1 = new ToolStripSeparator();
        printToolStripMenuItem = new ToolStripMenuItem();
        printPreviewToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparator2 = new ToolStripSeparator();
        exitToolStripMenuItem = new ToolStripMenuItem();
        editToolStripMenuItem = new ToolStripMenuItem();
        undoToolStripMenuItem = new ToolStripMenuItem();
        redoToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparator3 = new ToolStripSeparator();
        cutToolStripMenuItem = new ToolStripMenuItem();
        copyToolStripMenuItem = new ToolStripMenuItem();
        pasteToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparator4 = new ToolStripSeparator();
        selectAllToolStripMenuItem = new ToolStripMenuItem();
        toolsToolStripMenuItem = new ToolStripMenuItem();
        customizeToolStripMenuItem = new ToolStripMenuItem();
        optionsToolStripMenuItem = new ToolStripMenuItem();
        helpToolStripMenuItem = new ToolStripMenuItem();
        contentsToolStripMenuItem = new ToolStripMenuItem();
        indexToolStripMenuItem = new ToolStripMenuItem();
        searchToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparator5 = new ToolStripSeparator();
        aboutToolStripMenuItem = new ToolStripMenuItem();
        toolStripMenuItem1 = new ToolStripMenuItem();
        uncheckedCheckOnClickToolStripMenuItem = new ToolStripMenuItem();
        checkCheckOnClickToolStripMenuItem = new ToolStripMenuItem();
        checkedCheckOnClickFToolStripMenuItem = new ToolStripMenuItem();
        indeterminateToolStripMenuItem = new ToolStripMenuItem();
        indeterminateCheckOnClickFToolStripMenuItem = new ToolStripMenuItem();
        toolStripComboBox1 = new ToolStripComboBox();
        toolStripTextBox1 = new ToolStripTextBox();
        statusStrip1 = new StatusStrip();
        toolStripDropDownButton1 = new ToolStripDropDownButton();
        toolStripSeparator9 = new ToolStripSeparator();
        toolStripTextBox3 = new ToolStripTextBox();
        toolStripMenuItem3 = new ToolStripMenuItem();
        toolStripSplitButton1 = new ToolStripSplitButton();
        toolStripTextBox2 = new ToolStripTextBox();
        toolStripSeparator8 = new ToolStripSeparator();
        toolStripComboBox2 = new ToolStripComboBox();
        toolStripMenuItem2 = new ToolStripMenuItem();
        toolStripStatusLabel1 = new ToolStripStatusLabel();
        toolStripProgressBar1 = new ToolStripProgressBar();
        toolStrip1 = new ToolStrip();
        newToolStripButton = new ToolStripButton();
        toolStripSeparator7 = new ToolStripSeparator();
        toolStripButton1 = new ToolStripButton();
        toolStripButton2 = new ToolStripButton();
        toolStripButton3 = new ToolStripButton();
        toolStripButton4 = new ToolStripButton();
        toolStripButton5 = new ToolStripButton();
        toolStripButton6 = new ToolStripButton();
        menuStrip1.SuspendLayout();
        contextMenuStrip1.SuspendLayout();
        statusStrip1.SuspendLayout();
        toolStrip1.SuspendLayout();
        SuspendLayout();
        // 
        // menuStrip1
        // 
        menuStrip1.ContextMenuStrip = contextMenuStrip1;
        menuStrip1.ImageScalingSize = new Size(20, 20);
        menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, toolsToolStripMenuItem, helpToolStripMenuItem, toolStripMenuItem1, toolStripComboBox1, toolStripTextBox1 });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Padding = new Padding(8, 3, 0, 3);
        menuStrip1.ShowItemToolTips = true;
        menuStrip1.Size = new Size(1287, 34);
        menuStrip1.TabIndex = 0;
        menuStrip1.TabStop = true;
        menuStrip1.Text = "menuStrip1";
        // 
        // contextMenuStrip1
        // 
        contextMenuStrip1.ImageScalingSize = new Size(20, 20);
        contextMenuStrip1.Items.AddRange(new ToolStripItem[] { menuStripToolStripMenuItem, shortCutsToolStripMenuItem });
        contextMenuStrip1.Name = "contextMenuStrip1";
        contextMenuStrip1.Size = new Size(147, 52);
        contextMenuStrip1.TabStop = true;
        // 
        // menuStripToolStripMenuItem
        // 
        menuStripToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { item1ToolStripMenuItem });
        menuStripToolStripMenuItem.Name = "menuStripToolStripMenuItem";
        menuStripToolStripMenuItem.Size = new Size(146, 24);
        menuStripToolStripMenuItem.Text = "MenuStrip";
        // 
        // item1ToolStripMenuItem
        // 
        item1ToolStripMenuItem.Name = "item1ToolStripMenuItem";
        item1ToolStripMenuItem.Size = new Size(130, 26);
        item1ToolStripMenuItem.Text = "Item1";
        // 
        // shortCutsToolStripMenuItem
        // 
        shortCutsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { item2ToolStripMenuItem });
        shortCutsToolStripMenuItem.Name = "shortCutsToolStripMenuItem";
        shortCutsToolStripMenuItem.Size = new Size(146, 24);
        shortCutsToolStripMenuItem.Text = "ShortCuts";
        // 
        // item2ToolStripMenuItem
        // 
        item2ToolStripMenuItem.Name = "item2ToolStripMenuItem";
        item2ToolStripMenuItem.Size = new Size(130, 26);
        item2ToolStripMenuItem.Text = "Item2";
        // 
        // fileToolStripMenuItem
        // 
        fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, toolStripSeparator, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripSeparator1, printToolStripMenuItem, printPreviewToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
        fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        fileToolStripMenuItem.Size = new Size(46, 28);
        fileToolStripMenuItem.Text = "&File";
        fileToolStripMenuItem.ToolTipText = "File";
        // 
        // newToolStripMenuItem
        // 
        newToolStripMenuItem.Image = (Image)resources.GetObject("newToolStripMenuItem.Image");
        newToolStripMenuItem.ImageTransparentColor = Color.Magenta;
        newToolStripMenuItem.Name = "newToolStripMenuItem";
        newToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
        newToolStripMenuItem.Size = new Size(181, 26);
        newToolStripMenuItem.Text = "&New";
        // 
        // openToolStripMenuItem
        // 
        openToolStripMenuItem.Image = (Image)resources.GetObject("openToolStripMenuItem.Image");
        openToolStripMenuItem.ImageTransparentColor = Color.Magenta;
        openToolStripMenuItem.Name = "openToolStripMenuItem";
        openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
        openToolStripMenuItem.Size = new Size(181, 26);
        openToolStripMenuItem.Text = "&Open";
        // 
        // toolStripSeparator
        // 
        toolStripSeparator.Name = "toolStripSeparator";
        toolStripSeparator.Size = new Size(178, 6);
        // 
        // saveToolStripMenuItem
        // 
        saveToolStripMenuItem.Image = (Image)resources.GetObject("saveToolStripMenuItem.Image");
        saveToolStripMenuItem.ImageTransparentColor = Color.Magenta;
        saveToolStripMenuItem.Name = "saveToolStripMenuItem";
        saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
        saveToolStripMenuItem.Size = new Size(181, 26);
        saveToolStripMenuItem.Text = "&Save";
        // 
        // saveAsToolStripMenuItem
        // 
        saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
        saveAsToolStripMenuItem.Size = new Size(181, 26);
        saveAsToolStripMenuItem.Text = "Save &As";
        // 
        // toolStripSeparator1
        // 
        toolStripSeparator1.Name = "toolStripSeparator1";
        toolStripSeparator1.Size = new Size(178, 6);
        // 
        // printToolStripMenuItem
        // 
        printToolStripMenuItem.Image = (Image)resources.GetObject("printToolStripMenuItem.Image");
        printToolStripMenuItem.ImageTransparentColor = Color.Magenta;
        printToolStripMenuItem.Name = "printToolStripMenuItem";
        printToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.P;
        printToolStripMenuItem.Size = new Size(181, 26);
        printToolStripMenuItem.Text = "&Print";
        // 
        // printPreviewToolStripMenuItem
        // 
        printPreviewToolStripMenuItem.Image = (Image)resources.GetObject("printPreviewToolStripMenuItem.Image");
        printPreviewToolStripMenuItem.ImageTransparentColor = Color.Magenta;
        printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
        printPreviewToolStripMenuItem.Size = new Size(181, 26);
        printPreviewToolStripMenuItem.Text = "Print Pre&view";
        // 
        // toolStripSeparator2
        // 
        toolStripSeparator2.Name = "toolStripSeparator2";
        toolStripSeparator2.Size = new Size(178, 6);
        // 
        // exitToolStripMenuItem
        // 
        exitToolStripMenuItem.Name = "exitToolStripMenuItem";
        exitToolStripMenuItem.Size = new Size(181, 26);
        exitToolStripMenuItem.Text = "E&xit";
        // 
        // editToolStripMenuItem
        // 
        editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { undoToolStripMenuItem, redoToolStripMenuItem, toolStripSeparator3, cutToolStripMenuItem, copyToolStripMenuItem, pasteToolStripMenuItem, toolStripSeparator4, selectAllToolStripMenuItem });
        editToolStripMenuItem.Name = "editToolStripMenuItem";
        editToolStripMenuItem.Size = new Size(49, 28);
        editToolStripMenuItem.Text = "&Edit";
        editToolStripMenuItem.ToolTipText = "Edit";
        // 
        // undoToolStripMenuItem
        // 
        undoToolStripMenuItem.Name = "undoToolStripMenuItem";
        undoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
        undoToolStripMenuItem.Size = new Size(179, 26);
        undoToolStripMenuItem.Text = "&Undo";
        // 
        // redoToolStripMenuItem
        // 
        redoToolStripMenuItem.Name = "redoToolStripMenuItem";
        redoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
        redoToolStripMenuItem.Size = new Size(179, 26);
        redoToolStripMenuItem.Text = "&Redo";
        // 
        // toolStripSeparator3
        // 
        toolStripSeparator3.Name = "toolStripSeparator3";
        toolStripSeparator3.Size = new Size(176, 6);
        // 
        // cutToolStripMenuItem
        // 
        cutToolStripMenuItem.Image = (Image)resources.GetObject("cutToolStripMenuItem.Image");
        cutToolStripMenuItem.ImageTransparentColor = Color.Magenta;
        cutToolStripMenuItem.Name = "cutToolStripMenuItem";
        cutToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.X;
        cutToolStripMenuItem.Size = new Size(179, 26);
        cutToolStripMenuItem.Text = "Cu&t";
        // 
        // copyToolStripMenuItem
        // 
        copyToolStripMenuItem.Image = (Image)resources.GetObject("copyToolStripMenuItem.Image");
        copyToolStripMenuItem.ImageTransparentColor = Color.Magenta;
        copyToolStripMenuItem.Name = "copyToolStripMenuItem";
        copyToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.C;
        copyToolStripMenuItem.Size = new Size(179, 26);
        copyToolStripMenuItem.Text = "&Copy";
        // 
        // pasteToolStripMenuItem
        // 
        pasteToolStripMenuItem.Image = (Image)resources.GetObject("pasteToolStripMenuItem.Image");
        pasteToolStripMenuItem.ImageTransparentColor = Color.Magenta;
        pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
        pasteToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V;
        pasteToolStripMenuItem.Size = new Size(179, 26);
        pasteToolStripMenuItem.Text = "&Paste";
        // 
        // toolStripSeparator4
        // 
        toolStripSeparator4.Name = "toolStripSeparator4";
        toolStripSeparator4.Size = new Size(176, 6);
        // 
        // selectAllToolStripMenuItem
        // 
        selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
        selectAllToolStripMenuItem.Size = new Size(179, 26);
        selectAllToolStripMenuItem.Text = "Select &All";
        // 
        // toolsToolStripMenuItem
        // 
        toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { customizeToolStripMenuItem, optionsToolStripMenuItem });
        toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
        toolsToolStripMenuItem.Size = new Size(58, 28);
        toolsToolStripMenuItem.Text = "&Tools";
        toolsToolStripMenuItem.ToolTipText = "Tools";
        // 
        // customizeToolStripMenuItem
        // 
        customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
        customizeToolStripMenuItem.Size = new Size(161, 26);
        customizeToolStripMenuItem.Text = "&Customize";
        // 
        // optionsToolStripMenuItem
        // 
        optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
        optionsToolStripMenuItem.Size = new Size(161, 26);
        optionsToolStripMenuItem.Text = "&Options";
        // 
        // helpToolStripMenuItem
        // 
        helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { contentsToolStripMenuItem, indexToolStripMenuItem, searchToolStripMenuItem, toolStripSeparator5, aboutToolStripMenuItem });
        helpToolStripMenuItem.Name = "helpToolStripMenuItem";
        helpToolStripMenuItem.Size = new Size(55, 28);
        helpToolStripMenuItem.Text = "&Help";
        helpToolStripMenuItem.ToolTipText = "Help";
        // 
        // contentsToolStripMenuItem
        // 
        contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
        contentsToolStripMenuItem.Size = new Size(150, 26);
        contentsToolStripMenuItem.Text = "&Contents";
        // 
        // indexToolStripMenuItem
        // 
        indexToolStripMenuItem.Name = "indexToolStripMenuItem";
        indexToolStripMenuItem.Size = new Size(150, 26);
        indexToolStripMenuItem.Text = "&Index";
        // 
        // searchToolStripMenuItem
        // 
        searchToolStripMenuItem.Name = "searchToolStripMenuItem";
        searchToolStripMenuItem.Size = new Size(150, 26);
        searchToolStripMenuItem.Text = "&Search";
        // 
        // toolStripSeparator5
        // 
        toolStripSeparator5.Name = "toolStripSeparator5";
        toolStripSeparator5.Size = new Size(147, 6);
        // 
        // aboutToolStripMenuItem
        // 
        aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
        aboutToolStripMenuItem.Size = new Size(150, 26);
        aboutToolStripMenuItem.Text = "&About...";
        // 
        // toolStripMenuItem1
        // 
        toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { uncheckedCheckOnClickToolStripMenuItem, checkCheckOnClickToolStripMenuItem, checkedCheckOnClickFToolStripMenuItem, indeterminateToolStripMenuItem, indeterminateCheckOnClickFToolStripMenuItem });
        toolStripMenuItem1.Name = "toolStripMenuItem1";
        toolStripMenuItem1.Size = new Size(156, 28);
        toolStripMenuItem1.Text = "CheckState Samples";
        toolStripMenuItem1.ToolTipText = "CheckState Samples";
        // 
        // uncheckedCheckOnClickToolStripMenuItem
        // 
        uncheckedCheckOnClickToolStripMenuItem.CheckOnClick = true;
        uncheckedCheckOnClickToolStripMenuItem.Name = "uncheckedCheckOnClickToolStripMenuItem";
        uncheckedCheckOnClickToolStripMenuItem.Size = new Size(298, 26);
        uncheckedCheckOnClickToolStripMenuItem.Text = "Unchecked_CheckOnClick(T)";
        // 
        // checkCheckOnClickToolStripMenuItem
        // 
        checkCheckOnClickToolStripMenuItem.Checked = true;
        checkCheckOnClickToolStripMenuItem.CheckOnClick = true;
        checkCheckOnClickToolStripMenuItem.CheckState = CheckState.Checked;
        checkCheckOnClickToolStripMenuItem.Name = "checkCheckOnClickToolStripMenuItem";
        checkCheckOnClickToolStripMenuItem.Size = new Size(298, 26);
        checkCheckOnClickToolStripMenuItem.Text = "Checked_CheckOnClick(T)";
        // 
        // checkedCheckOnClickFToolStripMenuItem
        // 
        checkedCheckOnClickFToolStripMenuItem.Checked = true;
        checkedCheckOnClickFToolStripMenuItem.CheckState = CheckState.Checked;
        checkedCheckOnClickFToolStripMenuItem.Name = "checkedCheckOnClickFToolStripMenuItem";
        checkedCheckOnClickFToolStripMenuItem.Size = new Size(298, 26);
        checkedCheckOnClickFToolStripMenuItem.Text = "Checked_CheckOnClick(F)";
        // 
        // indeterminateToolStripMenuItem
        // 
        indeterminateToolStripMenuItem.Checked = true;
        indeterminateToolStripMenuItem.CheckOnClick = true;
        indeterminateToolStripMenuItem.CheckState = CheckState.Indeterminate;
        indeterminateToolStripMenuItem.Name = "indeterminateToolStripMenuItem";
        indeterminateToolStripMenuItem.Size = new Size(298, 26);
        indeterminateToolStripMenuItem.Text = "Indeterminate_CheckOnClick(T)";
        // 
        // indeterminateCheckOnClickFToolStripMenuItem
        // 
        indeterminateCheckOnClickFToolStripMenuItem.Checked = true;
        indeterminateCheckOnClickFToolStripMenuItem.CheckState = CheckState.Indeterminate;
        indeterminateCheckOnClickFToolStripMenuItem.Name = "indeterminateCheckOnClickFToolStripMenuItem";
        indeterminateCheckOnClickFToolStripMenuItem.Size = new Size(298, 26);
        indeterminateCheckOnClickFToolStripMenuItem.Text = "Indeterminate_CheckOnClick(F)";
        // 
        // toolStripComboBox1
        // 
        toolStripComboBox1.AccessibleName = "toolStripCombo_Box";
        toolStripComboBox1.Name = "toolStripComboBox1";
        toolStripComboBox1.Size = new Size(159, 28);
        toolStripComboBox1.ToolTipText = "toolStripComboBox1";
        // 
        // toolStripTextBox1
        // 
        toolStripTextBox1.AccessibleName = "toolStripText_Box1";
        toolStripTextBox1.Name = "toolStripTextBox1";
        toolStripTextBox1.Size = new Size(132, 28);
        toolStripTextBox1.Text = "toolStripTextBox1";
        // 
        // statusStrip1
        // 
        statusStrip1.ImageScalingSize = new Size(20, 20);
        statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripDropDownButton1, toolStripSplitButton1, toolStripStatusLabel1, toolStripProgressBar1 });
        statusStrip1.Location = new Point(0, 520);
        statusStrip1.Name = "statusStrip1";
        statusStrip1.Padding = new Padding(1, 0, 18, 0);
        statusStrip1.ShowItemToolTips = true;
        statusStrip1.Size = new Size(1287, 29);
        statusStrip1.TabIndex = 2;
        statusStrip1.TabStop = true;
        statusStrip1.Text = "statusStrip1";
        // 
        // toolStripDropDownButton1
        // 
        toolStripDropDownButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
        toolStripDropDownButton1.DropDownItems.AddRange(new ToolStripItem[] { toolStripSeparator9, toolStripTextBox3, toolStripMenuItem3 });
        toolStripDropDownButton1.Image = (Image)resources.GetObject("toolStripDropDownButton1.Image");
        toolStripDropDownButton1.ImageTransparentColor = Color.Magenta;
        toolStripDropDownButton1.Name = "toolStripDropDownButton1";
        toolStripDropDownButton1.Size = new Size(34, 27);
        toolStripDropDownButton1.Text = "toolStripDropDownButton1";
        // 
        // toolStripSeparator9
        // 
        toolStripSeparator9.Name = "toolStripSeparator9";
        toolStripSeparator9.Size = new Size(222, 6);
        // 
        // toolStripTextBox3
        // 
        toolStripTextBox3.Name = "toolStripTextBox3";
        toolStripTextBox3.Size = new Size(100, 27);
        // 
        // toolStripMenuItem3
        // 
        toolStripMenuItem3.Name = "toolStripMenuItem3";
        toolStripMenuItem3.Size = new Size(225, 26);
        toolStripMenuItem3.Text = "toolStripMenuItem3";
        // 
        // toolStripSplitButton1
        // 
        toolStripSplitButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
        toolStripSplitButton1.DropDownItems.AddRange(new ToolStripItem[] { toolStripTextBox2, toolStripSeparator8, toolStripComboBox2, toolStripMenuItem2 });
        toolStripSplitButton1.Image = (Image)resources.GetObject("toolStripSplitButton1.Image");
        toolStripSplitButton1.ImageTransparentColor = Color.Magenta;
        toolStripSplitButton1.Name = "toolStripSplitButton1";
        toolStripSplitButton1.Size = new Size(39, 27);
        toolStripSplitButton1.Text = "toolStripSplitButton1";
        // 
        // toolStripTextBox2
        // 
        toolStripTextBox2.Name = "toolStripTextBox2";
        toolStripTextBox2.Size = new Size(100, 27);
        // 
        // toolStripSeparator8
        // 
        toolStripSeparator8.Name = "toolStripSeparator8";
        toolStripSeparator8.Size = new Size(222, 6);
        // 
        // toolStripComboBox2
        // 
        toolStripComboBox2.Name = "toolStripComboBox2";
        toolStripComboBox2.Size = new Size(121, 28);
        // 
        // toolStripMenuItem2
        // 
        toolStripMenuItem2.Name = "toolStripMenuItem2";
        toolStripMenuItem2.Size = new Size(225, 26);
        toolStripMenuItem2.Text = "toolStripMenuItem2";
        // 
        // toolStripStatusLabel1
        // 
        toolStripStatusLabel1.Name = "toolStripStatusLabel1";
        toolStripStatusLabel1.Size = new Size(151, 23);
        toolStripStatusLabel1.Text = "toolStripStatusLabel1";
        toolStripStatusLabel1.ToolTipText = "toolStripStatusLabel1";
        // 
        // toolStripProgressBar1
        // 
        toolStripProgressBar1.AccessibleName = "Progress_Bar";
        toolStripProgressBar1.Name = "toolStripProgressBar1";
        toolStripProgressBar1.Size = new Size(134, 21);
        // 
        // toolStrip1
        // 
        toolStrip1.ImageScalingSize = new Size(20, 20);
        toolStrip1.Items.AddRange(new ToolStripItem[] { newToolStripButton, toolStripSeparator7, toolStripButton1, toolStripButton2, toolStripButton3, toolStripButton4, toolStripButton5, toolStripButton6 });
        toolStrip1.Location = new Point(0, 34);
        toolStrip1.Name = "toolStrip1";
        toolStrip1.Size = new Size(1287, 31);
        toolStrip1.TabIndex = 1;
        toolStrip1.TabStop = true;
        toolStrip1.Text = "toolStrip1";
        // 
        // newToolStripButton
        // 
        newToolStripButton.Image = (Image)resources.GetObject("newToolStripButton.Image");
        newToolStripButton.ImageTransparentColor = Color.Magenta;
        newToolStripButton.Name = "newToolStripButton";
        newToolStripButton.Size = new Size(63, 28);
        newToolStripButton.Text = "&New";
        // 
        // toolStripSeparator7
        // 
        toolStripSeparator7.Name = "toolStripSeparator7";
        toolStripSeparator7.Size = new Size(6, 31);
        // 
        // toolStripButton1
        // 
        toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
        toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
        toolStripButton1.ImageTransparentColor = Color.Magenta;
        toolStripButton1.Name = "toolStripButton1";
        toolStripButton1.Size = new Size(197, 28);
        toolStripButton1.Text = "Unchecked_CheckOnClick(F)";
        // 
        // toolStripButton2
        // 
        toolStripButton2.CheckOnClick = true;
        toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Text;
        toolStripButton2.Image = (Image)resources.GetObject("toolStripButton2.Image");
        toolStripButton2.ImageTransparentColor = Color.Magenta;
        toolStripButton2.Name = "toolStripButton2";
        toolStripButton2.Size = new Size(198, 28);
        toolStripButton2.Text = "Unchecked_CheckOnClick(T)";
        // 
        // toolStripButton3
        // 
        toolStripButton3.Checked = true;
        toolStripButton3.CheckOnClick = true;
        toolStripButton3.CheckState = CheckState.Checked;
        toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Text;
        toolStripButton3.Image = (Image)resources.GetObject("toolStripButton3.Image");
        toolStripButton3.ImageTransparentColor = Color.Magenta;
        toolStripButton3.Name = "toolStripButton3";
        toolStripButton3.Size = new Size(182, 28);
        toolStripButton3.Text = "Checked_CheckOnClick(T)";
        // 
        // toolStripButton4
        // 
        toolStripButton4.Checked = true;
        toolStripButton4.CheckState = CheckState.Checked;
        toolStripButton4.DisplayStyle = ToolStripItemDisplayStyle.Text;
        toolStripButton4.Image = (Image)resources.GetObject("toolStripButton4.Image");
        toolStripButton4.ImageTransparentColor = Color.Magenta;
        toolStripButton4.Name = "toolStripButton4";
        toolStripButton4.Size = new Size(181, 28);
        toolStripButton4.Text = "Checked_CheckOnClick(F)";
        // 
        // toolStripButton5
        // 
        toolStripButton5.Checked = true;
        toolStripButton5.CheckOnClick = true;
        toolStripButton5.CheckState = CheckState.Indeterminate;
        toolStripButton5.DisplayStyle = ToolStripItemDisplayStyle.Text;
        toolStripButton5.Image = (Image)resources.GetObject("toolStripButton5.Image");
        toolStripButton5.ImageTransparentColor = Color.Magenta;
        toolStripButton5.Name = "toolStripButton5";
        toolStripButton5.Size = new Size(219, 28);
        toolStripButton5.Text = "Indeterminate_CheckOnClick(T)";
        // 
        // toolStripButton6
        // 
        toolStripButton6.Checked = true;
        toolStripButton6.CheckState = CheckState.Indeterminate;
        toolStripButton6.DisplayStyle = ToolStripItemDisplayStyle.Text;
        toolStripButton6.Image = (Image)resources.GetObject("toolStripButton6.Image");
        toolStripButton6.ImageTransparentColor = Color.Magenta;
        toolStripButton6.Name = "toolStripButton6";
        toolStripButton6.Size = new Size(218, 24);
        toolStripButton6.Text = "Indeterminate_CheckOnClick(F)";
        // 
        // Menu_Toolbars_controls
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1287, 549);
        Controls.Add(toolStrip1);
        Controls.Add(statusStrip1);
        Controls.Add(menuStrip1);
        MainMenuStrip = menuStrip1;
        Margin = new Padding(5, 4, 5, 4);
        Name = "Menu_Toolbars_controls";
        Text = "StripControls";
        menuStrip1.ResumeLayout(false);
        menuStrip1.PerformLayout();
        contextMenuStrip1.ResumeLayout(false);
        statusStrip1.ResumeLayout(false);
        statusStrip1.PerformLayout();
        toolStrip1.ResumeLayout(false);
        toolStrip1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

#endregion

    private MenuStrip menuStrip1;
    private ContextMenuStrip contextMenuStrip1;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem newToolStripMenuItem;
    private ToolStripMenuItem openToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator;
    private ToolStripMenuItem saveToolStripMenuItem;
    private ToolStripMenuItem saveAsToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripMenuItem printToolStripMenuItem;
    private ToolStripMenuItem printPreviewToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripMenuItem exitToolStripMenuItem;
    private ToolStripMenuItem editToolStripMenuItem;
    private ToolStripMenuItem undoToolStripMenuItem;
    private ToolStripMenuItem redoToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator3;
    private ToolStripMenuItem cutToolStripMenuItem;
    private ToolStripMenuItem copyToolStripMenuItem;
    private ToolStripMenuItem pasteToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator4;
    private ToolStripMenuItem selectAllToolStripMenuItem;
    private ToolStripMenuItem toolsToolStripMenuItem;
    private ToolStripMenuItem customizeToolStripMenuItem;
    private ToolStripMenuItem optionsToolStripMenuItem;
    private ToolStripMenuItem helpToolStripMenuItem;
    private ToolStripMenuItem contentsToolStripMenuItem;
    private ToolStripMenuItem indexToolStripMenuItem;
    private ToolStripMenuItem searchToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator5;
    private ToolStripMenuItem aboutToolStripMenuItem;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel toolStripStatusLabel1;
    private ToolStripDropDownButton toolStripDropDownButton1;
    private ToolStripSplitButton toolStripSplitButton1;
    private ToolStrip toolStrip1;
    private ToolStripButton newToolStripButton;
    private ToolStripSeparator toolStripSeparator7;
    private ToolStripMenuItem menuStripToolStripMenuItem;
    private ToolStripMenuItem shortCutsToolStripMenuItem;
    private ToolStripMenuItem toolStripMenuItem1;
    private ToolStripComboBox toolStripComboBox1;
    private ToolStripMenuItem item1ToolStripMenuItem;
    private ToolStripMenuItem item2ToolStripMenuItem;
    private ToolStripTextBox toolStripTextBox1;
    private ToolStripSeparator toolStripSeparator9;
    private ToolStripTextBox toolStripTextBox3;
    private ToolStripMenuItem toolStripMenuItem3;
    private ToolStripTextBox toolStripTextBox2;
    private ToolStripSeparator toolStripSeparator8;
    private ToolStripComboBox toolStripComboBox2;
    private ToolStripMenuItem toolStripMenuItem2;
    private ToolStripProgressBar toolStripProgressBar1;
    private ToolStripMenuItem uncheckedCheckOnClickToolStripMenuItem;
    private ToolStripMenuItem checkCheckOnClickToolStripMenuItem;
    private ToolStripMenuItem checkedCheckOnClickFToolStripMenuItem;
    private ToolStripMenuItem indeterminateToolStripMenuItem;
    private ToolStripMenuItem indeterminateCheckOnClickFToolStripMenuItem;
    private ToolStripButton toolStripButton1;
    private ToolStripButton toolStripButton2;
    private ToolStripButton toolStripButton3;
    private ToolStripButton toolStripButton4;
    private ToolStripButton toolStripButton5;
    private ToolStripButton toolStripButton6;
}
