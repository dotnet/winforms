// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

partial class MdiChild
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
        this.panel1 = new System.Windows.Forms.Panel();
        this.btnOpenChild = new System.Windows.Forms.Button();
        this.chkSetParentMenustrip = new System.Windows.Forms.CheckBox();
        this.chkSetMenustrip = new System.Windows.Forms.CheckBox();
        this.chkAddParentMenustrip = new System.Windows.Forms.CheckBox();
        this.chkAddMenustrip = new System.Windows.Forms.CheckBox();
        this.chkChildAlign = new System.Windows.Forms.CheckBox();
        this.chkRightToLeft = new System.Windows.Forms.CheckBox();
        this.panel1.SuspendLayout();
        this.SuspendLayout();
        // 
        // panel1
        //
        this.panel1.Controls.Add(this.chkRightToLeft);
        this.panel1.Controls.Add(this.chkChildAlign);
        this.panel1.Controls.Add(this.chkAddMenustrip);
        this.panel1.Controls.Add(this.chkAddParentMenustrip);
        this.panel1.Controls.Add(this.chkSetMenustrip);
        this.panel1.Controls.Add(this.chkSetParentMenustrip);
        this.panel1.Controls.Add(this.btnOpenChild);
        this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.panel1.Location = new System.Drawing.Point(0, 0);
        this.panel1.Name = "panel1";
        this.panel1.Size = new System.Drawing.Size(480, 221);
        this.panel1.TabIndex = 0;
        this.panel1.DoubleClick += this.panel1_DoubleClick;
        // 
        // chkChildAlign
        // 
        this.chkChildAlign.AutoSize = true;
        this.chkChildAlign.Location = new System.Drawing.Point(43, 121);
        this.chkChildAlign.Name = "chkChildAlign";
        this.chkChildAlign.Size = new System.Drawing.Size(162, 17);
        this.chkChildAlign.TabIndex = 3;
        this.chkChildAlign.Text = "Top align minimized child";
        this.chkChildAlign.UseVisualStyleBackColor = true;
        this.chkChildAlign.CheckedChanged += this.chkChildAlign_CheckedChanged;
        // 
        // chkRightToLeft
        // 
        this.chkRightToLeft.AutoSize = true;
        this.chkRightToLeft.Location = new System.Drawing.Point(228, 121);
        this.chkRightToLeft.Name = "chkRightToLeft";
        this.chkRightToLeft.Size = new System.Drawing.Size(162, 17);
        this.chkRightToLeft.TabIndex = 4;
        this.chkRightToLeft.Text = "Right to left mode";
        this.chkRightToLeft.UseVisualStyleBackColor = true;
        this.chkRightToLeft.CheckedChanged += this.chkRightToLeft_CheckedChanged;
        // 
        // btnOpenChild
        // 
        this.btnOpenChild.AutoSize = true;
        this.btnOpenChild.Location = new System.Drawing.Point(40, 22);
        this.btnOpenChild.Name = "btnOpenChild";
        this.btnOpenChild.Size = new System.Drawing.Size(138, 23);
        this.btnOpenChild.TabIndex = 0;
        this.btnOpenChild.Text = "Open new child";
        this.btnOpenChild.UseVisualStyleBackColor = true;
        this.btnOpenChild.Click += this.btnOpenChild_Click;
        // 
        // chkSetParentMenustrip
        // 
        this.chkSetParentMenustrip.AutoSize = true;
        this.chkSetParentMenustrip.Location = new System.Drawing.Point(43, 73);
        this.chkSetParentMenustrip.Name = "chkSetParentMenustrip";
        this.chkSetParentMenustrip.Size = new System.Drawing.Size(123, 17);
        this.chkSetParentMenustrip.TabIndex = 1;
        this.chkSetParentMenustrip.Text = "Set parent menustrip";
        this.chkSetParentMenustrip.UseVisualStyleBackColor = true;
        this.chkSetParentMenustrip.CheckedChanged += this.chkSetParentMenustrip_CheckedChanged;
        // 
        // chkSetMenustrip
        // 
        this.chkSetMenustrip.AutoSize = true;
        this.chkSetMenustrip.Location = new System.Drawing.Point(228, 73);
        this.chkSetMenustrip.Name = "chkSetMenustrip";
        this.chkSetMenustrip.Size = new System.Drawing.Size(115, 17);
        this.chkSetMenustrip.TabIndex = 3;
        this.chkSetMenustrip.Text = "Set child menustrip";
        this.chkSetMenustrip.UseVisualStyleBackColor = true;
        this.chkSetMenustrip.CheckedChanged += this.chkSetMenustrip_CheckedChanged;
        // 
        // chkAddParentMenustrip
        // 
        this.chkAddParentMenustrip.AutoSize = true;
        this.chkAddParentMenustrip.Location = new System.Drawing.Point(43, 97);
        this.chkAddParentMenustrip.Name = "chkAddParentMenustrip";
        this.chkAddParentMenustrip.Size = new System.Drawing.Size(179, 17);
        this.chkAddParentMenustrip.TabIndex = 2;
        this.chkAddParentMenustrip.Text = "Add parent menustrip to Controls";
        this.chkAddParentMenustrip.UseVisualStyleBackColor = true;
        this.chkAddParentMenustrip.CheckedChanged += this.chkAddParentMenustrip_CheckedChanged;
        // 
        // chkAddMenustrip
        // 
        this.chkAddMenustrip.AutoSize = true;
        this.chkAddMenustrip.Location = new System.Drawing.Point(228, 97);
        this.chkAddMenustrip.Name = "chkAddMenustrip";
        this.chkAddMenustrip.Size = new System.Drawing.Size(146, 17);
        this.chkAddMenustrip.TabIndex = 4;
        this.chkAddMenustrip.Text = "Add menustrip to Controls";
        this.chkAddMenustrip.UseVisualStyleBackColor = true;
        this.chkAddMenustrip.CheckedChanged += this.chkAddMenustrip_CheckedChanged;
        // 
        // ChildForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(480, 221);
        this.Controls.Add(this.panel1);
        this.Name = "ChildForm";
        this.Text = "ChildForm";
        this.panel1.ResumeLayout(false);
        this.panel1.PerformLayout();
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnOpenChild;
    private System.Windows.Forms.CheckBox chkAddMenustrip;
    private System.Windows.Forms.CheckBox chkAddParentMenustrip;
    private System.Windows.Forms.CheckBox chkSetMenustrip;
    private System.Windows.Forms.CheckBox chkSetParentMenustrip;
    private System.Windows.Forms.CheckBox chkChildAlign;
    private System.Windows.Forms.CheckBox chkRightToLeft;
}
