// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

partial class Dialogs
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
        this.resources = new System.ComponentModel.ComponentResourceManager(typeof(Dialogs));
        this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
        this.btnSaveFileDialog = new System.Windows.Forms.Button();
        this.btnOpenFileDialog = new System.Windows.Forms.Button();
        this.btnColorDialog = new System.Windows.Forms.Button();
        this.btnThreadExceptionDialog = new System.Windows.Forms.Button();
        this.btnPrintDialog = new System.Windows.Forms.Button();
        this.btnFolderBrowserDialog = new System.Windows.Forms.Button();
        this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
        this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
        this.printDialog1 = new System.Windows.Forms.PrintDialog();
        this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
        this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
        this.flowLayoutPanel1.SuspendLayout();
        this.SuspendLayout();
        // 
        // propertyGrid1
        // 
        this.propertyGrid1.CommandsBorderColor = System.Drawing.SystemColors.Control;
        this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.propertyGrid1.Location = new System.Drawing.Point(171, 0);
        this.propertyGrid1.Margin = new System.Windows.Forms.Padding(2);
        this.propertyGrid1.Name = "propertyGrid1";
        this.propertyGrid1.Size = new System.Drawing.Size(332, 430);
        this.propertyGrid1.TabIndex = 0;
        this.propertyGrid1.SelectedObjectsChanged += this.propertyGrid1_SelectedObjectsChanged;
        // 
        // btnOpenFileDialog
        // 
        this.btnOpenFileDialog.Location = new System.Drawing.Point(3, 3);
        this.btnOpenFileDialog.Name = "btnOpenFileDialog";
        this.btnOpenFileDialog.Size = new System.Drawing.Size(163, 23);
        this.btnOpenFileDialog.TabIndex = 1;
        this.btnOpenFileDialog.Text = "Open &file dialog";
        this.btnOpenFileDialog.UseVisualStyleBackColor = true;
        this.btnOpenFileDialog.Click += this.btnOpenFileDialog_Click;
        // 
        // btnColorDialog
        // 
        this.btnColorDialog.Location = new System.Drawing.Point(3, 32);
        this.btnColorDialog.Name = "btnColorDialog";
        this.btnColorDialog.Size = new System.Drawing.Size(163, 23);
        this.btnColorDialog.TabIndex = 2;
        this.btnColorDialog.Text = "&Color dialog";
        this.btnColorDialog.UseVisualStyleBackColor = true;
        this.btnColorDialog.Click += this.btnColorDialog_Click;
        // 
        // btnThreadExceptionDialog
        // 
        this.btnThreadExceptionDialog.Location = new System.Drawing.Point(3, 32);
        this.btnThreadExceptionDialog.Name = "btnThreadExceptionDialog";
        this.btnThreadExceptionDialog.Size = new System.Drawing.Size(163, 23);
        this.btnThreadExceptionDialog.TabIndex = 2;
        this.btnThreadExceptionDialog.Text = "&Thread exception dialog";
        this.btnThreadExceptionDialog.UseVisualStyleBackColor = true;
        this.btnThreadExceptionDialog.Click += this.btnThreadExceptionDialog_Click;
        // 
        // btnPrintDialog
        // 
        this.btnPrintDialog.Location = new System.Drawing.Point(3, 32);
        this.btnPrintDialog.Name = "btnPrintDialog";
        this.btnPrintDialog.Size = new System.Drawing.Size(163, 23);
        this.btnPrintDialog.TabIndex = 3;
        this.btnPrintDialog.Text = "&Print dialog";
        this.btnPrintDialog.UseVisualStyleBackColor = true;
        this.btnPrintDialog.Click += this.btnPrintDialog_Click;
        // 
        // btnFolderBrowserDialog
        // 
        this.btnFolderBrowserDialog.Location = new System.Drawing.Point(3, 61);
        this.btnFolderBrowserDialog.Name = "btnFolderBrowserDialog";
        this.btnFolderBrowserDialog.Size = new System.Drawing.Size(163, 23);
        this.btnFolderBrowserDialog.TabIndex = 4;
        this.btnFolderBrowserDialog.Text = "Folder &browser dialog";
        this.btnFolderBrowserDialog.UseVisualStyleBackColor = true;
        this.btnFolderBrowserDialog.Click += this.btnFolderBrowserDialog_Click;
        // 
        // btnSaveFileDialog
        // 
        this.btnSaveFileDialog.Location = new System.Drawing.Point(3, 3);
        this.btnSaveFileDialog.Name = "btnSaveFileDialog";
        this.btnSaveFileDialog.Size = new System.Drawing.Size(163, 23);
        this.btnSaveFileDialog.TabIndex = 5;
        this.btnSaveFileDialog.Text = "&Save file dialog";
        this.btnSaveFileDialog.UseVisualStyleBackColor = true;
        this.btnSaveFileDialog.Click += this.btnSaveFileDialog_Click;
        // 
        // flowLayoutPanel1
        // 
        this.flowLayoutPanel1.Controls.Add(this.btnColorDialog);
        this.flowLayoutPanel1.Controls.Add(this.btnThreadExceptionDialog);
        this.flowLayoutPanel1.Controls.Add(this.btnPrintDialog);
        this.flowLayoutPanel1.Controls.Add(this.btnFolderBrowserDialog);
        this.flowLayoutPanel1.Controls.Add(this.btnOpenFileDialog);
        this.flowLayoutPanel1.Controls.Add(this.btnSaveFileDialog);
        this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
        this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
        this.flowLayoutPanel1.Name = "flowLayoutPanel1";
        this.flowLayoutPanel1.Size = new System.Drawing.Size(171, 430);
        this.flowLayoutPanel1.TabIndex = 3;
        // 
        // openFileDialog1
        // 
        this.openFileDialog1.FileName = "openFileDialog1";
        // 
        // folderBrowserDialog1
        // 
        this.folderBrowserDialog1.Description = "Open global dotnet folder";
        this.folderBrowserDialog1.InitialDirectory = "C:\\Program Files\\dotnet";
        this.folderBrowserDialog1.ShowNewFolderButton = false;
        this.folderBrowserDialog1.UseDescriptionForTitle = true;
        // 
        // Dialogs
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(503, 430);
        this.Controls.Add(this.propertyGrid1);
        this.Controls.Add(this.flowLayoutPanel1);
        this.Name = "Dialogs";
        this.Text = "Dialogs Test";
        this.flowLayoutPanel1.ResumeLayout(false);
        this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.PropertyGrid propertyGrid1;
    private System.Windows.Forms.Button btnSaveFileDialog;
    private System.Windows.Forms.Button btnOpenFileDialog;
    private System.Windows.Forms.Button btnColorDialog;
    private System.Windows.Forms.Button btnThreadExceptionDialog;
    private System.Windows.Forms.Button btnPrintDialog;
    private System.Windows.Forms.Button btnFolderBrowserDialog;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.OpenFileDialog openFileDialog1;
    private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    private System.Windows.Forms.PrintDialog printDialog1;
    private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    private System.ComponentModel.ComponentResourceManager resources;
}
