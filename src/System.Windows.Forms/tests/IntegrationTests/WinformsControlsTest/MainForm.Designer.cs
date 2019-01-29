// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WinformsControlsTest
{
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
            this.buttonsButton = new System.Windows.Forms.Button();
            this.calendar = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.multipleControls = new System.Windows.Forms.Button();
            this.dataGridViewButton = new System.Windows.Forms.Button();
            this.menuesButton = new System.Windows.Forms.Button();
            this.panelsButton = new System.Windows.Forms.Button();
            this.splitterButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ComboBoxesButton = new System.Windows.Forms.Button();
            this.mdiParent = new System.Windows.Forms.Button();
            this.propertyGrid = new System.Windows.Forms.Button();
            this.listViewButton = new System.Windows.Forms.Button();
            this.DateTimePickerButton = new System.Windows.Forms.Button();
            this.FolderBrowserDialogButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonsButton
            // 
            this.buttonsButton.Location = new System.Drawing.Point(13, 33);
            this.buttonsButton.Name = "buttonsButton";
            this.buttonsButton.Size = new System.Drawing.Size(259, 23);
            this.buttonsButton.TabIndex = 0;
            this.buttonsButton.Text = "Buttons";
            this.buttonsButton.UseVisualStyleBackColor = true;
            this.buttonsButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // calendar
            // 
            this.calendar.Location = new System.Drawing.Point(12, 105);
            this.calendar.Name = "calendar";
            this.calendar.Size = new System.Drawing.Size(259, 23);
            this.calendar.TabIndex = 1;
            this.calendar.Text = "Calendar";
            this.calendar.UseVisualStyleBackColor = true;
            this.calendar.Click += new System.EventHandler(this.calendar_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(302, 109);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(258, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "TreeView, ImageList";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(302, 147);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(258, 23);
            this.button4.TabIndex = 3;
            this.button4.Text = "Content alignment";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // multipleControls
            // 
            this.multipleControls.Location = new System.Drawing.Point(13, 69);
            this.multipleControls.Name = "multipleControls";
            this.multipleControls.Size = new System.Drawing.Size(259, 23);
            this.multipleControls.TabIndex = 4;
            this.multipleControls.Text = "Multiple controls";
            this.multipleControls.UseVisualStyleBackColor = true;
            this.multipleControls.Click += new System.EventHandler(this.multipleControls_Click);
            // 
            // dataGridViewButton
            // 
            this.dataGridViewButton.Location = new System.Drawing.Point(302, 33);
            this.dataGridViewButton.Name = "dataGridViewButton";
            this.dataGridViewButton.Size = new System.Drawing.Size(258, 23);
            this.dataGridViewButton.TabIndex = 5;
            this.dataGridViewButton.Text = "DataGridView";
            this.dataGridViewButton.UseVisualStyleBackColor = true;
            this.dataGridViewButton.Click += new System.EventHandler(this.dataGridViewButton_Click);
            // 
            // menuesButton
            // 
            this.menuesButton.Location = new System.Drawing.Point(302, 185);
            this.menuesButton.Name = "menuesButton";
            this.menuesButton.Size = new System.Drawing.Size(258, 23);
            this.menuesButton.TabIndex = 7;
            this.menuesButton.Text = "Menus";
            this.menuesButton.UseVisualStyleBackColor = true;
            this.menuesButton.Click += new System.EventHandler(this.menuesButton_Click);
            // 
            // panelsButton
            // 
            this.panelsButton.Location = new System.Drawing.Point(303, 223);
            this.panelsButton.Name = "panelsButton";
            this.panelsButton.Size = new System.Drawing.Size(258, 23);
            this.panelsButton.TabIndex = 8;
            this.panelsButton.Text = "Panels";
            this.panelsButton.UseVisualStyleBackColor = true;
            this.panelsButton.Click += new System.EventHandler(this.panelsButton_Click);
            // 
            // splitterButton
            // 
            this.splitterButton.Location = new System.Drawing.Point(303, 71);
            this.splitterButton.Name = "splitterButton";
            this.splitterButton.Size = new System.Drawing.Size(258, 23);
            this.splitterButton.TabIndex = 9;
            this.splitterButton.Text = "Splitter";
            this.splitterButton.UseVisualStyleBackColor = true;
            this.splitterButton.Click += new System.EventHandler(this.splitterButton_Click);
            //// 
            //// label1
            //// 
            //this.label1.AutoSize = true;
            //this.label1.Location = new System.Drawing.Point(383, 13);
            //this.label1.Name = "label1";
            //this.label1.Size = new System.Drawing.Size(44, 13);
            //this.label1.TabIndex = 10;
            //this.label1.Text = "Broken:";
            //// 
            //// label2
            //// 
            //this.label2.AutoSize = true;
            //this.label2.Location = new System.Drawing.Point(74, 12);
            //this.label2.Name = "label2";
            //this.label2.Size = new System.Drawing.Size(50, 13);
            //this.label2.TabIndex = 11;
            //this.label2.Text = "Working:";
            // 
            // ComboBoxesButton
            // 
            this.ComboBoxesButton.Location = new System.Drawing.Point(13, 141);
            this.ComboBoxesButton.Name = "ComboBoxesButton";
            this.ComboBoxesButton.Size = new System.Drawing.Size(259, 23);
            this.ComboBoxesButton.TabIndex = 12;
            this.ComboBoxesButton.Text = "ComboBoxes";
            this.ComboBoxesButton.UseVisualStyleBackColor = true;
            this.ComboBoxesButton.Click += new System.EventHandler(this.ComboBoxesButton_Click);
            // 
            // mdiParent
            // 
            this.mdiParent.Location = new System.Drawing.Point(303, 261);
            this.mdiParent.Name = "mdiParent";
            this.mdiParent.Size = new System.Drawing.Size(258, 23);
            this.mdiParent.TabIndex = 13;
            this.mdiParent.Text = "MDI Parent";
            this.mdiParent.UseVisualStyleBackColor = true;
            this.mdiParent.Click += new System.EventHandler(this.mdiParent_Click);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Location = new System.Drawing.Point(303, 299);
            this.propertyGrid.Margin = new System.Windows.Forms.Padding(2);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(258, 23);
            this.propertyGrid.TabIndex = 14;
            this.propertyGrid.Text = "PropertyGrid";
            this.propertyGrid.UseVisualStyleBackColor = true;
            this.propertyGrid.Click += new System.EventHandler(this.propertyGrid_Click);
            // 
            // listViewButton
            // 
            this.listViewButton.Location = new System.Drawing.Point(303, 354);
            this.listViewButton.Margin = new System.Windows.Forms.Padding(2);
            this.listViewButton.Name = "listViewButton";
            this.listViewButton.Size = new System.Drawing.Size(258, 23);
            this.listViewButton.TabIndex = 16;
            this.listViewButton.Text = "ListVew";
            this.listViewButton.UseVisualStyleBackColor = true;
            this.listViewButton.Click += new System.EventHandler(this.listViewButton_Click);
            // 
            // DateTimePickerButton
            // 
            this.DateTimePickerButton.Location = new System.Drawing.Point(13, 185);
            this.DateTimePickerButton.Name = "DateTimePickerButton";
            this.DateTimePickerButton.Size = new System.Drawing.Size(259, 23);
            this.DateTimePickerButton.TabIndex = 17;
            this.DateTimePickerButton.Text = "DateTimePickerButton";
            this.DateTimePickerButton.UseVisualStyleBackColor = true;
            this.DateTimePickerButton.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // FolderBrowserDialogButton
            // 
            this.FolderBrowserDialogButton.Location = new System.Drawing.Point(13, 223);
            this.FolderBrowserDialogButton.Name = "FolderBrowserDialogButton";
            this.FolderBrowserDialogButton.Size = new System.Drawing.Size(259, 23);
            this.FolderBrowserDialogButton.TabIndex = 18;
            this.FolderBrowserDialogButton.Text = "FolderBrowserDialog";
            this.FolderBrowserDialogButton.UseVisualStyleBackColor = true;
            this.FolderBrowserDialogButton.Click += new System.EventHandler(this.folderBrowserDialogButton_Click);
            // 
            // MenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(578, 402);
            this.Controls.Add(this.FolderBrowserDialogButton);
            this.Controls.Add(this.DateTimePickerButton);
            this.Controls.Add(this.listViewButton);
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.mdiParent);
            this.Controls.Add(this.ComboBoxesButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.splitterButton);
            this.Controls.Add(this.panelsButton);
            this.Controls.Add(this.menuesButton);
            this.Controls.Add(this.dataGridViewButton);
            this.Controls.Add(this.multipleControls);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.calendar);
            this.Controls.Add(this.buttonsButton);
            this.Name = "MenuForm";
            this.Text = "MenuForm";
            this.Load += new System.EventHandler(this.MenuForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonsButton;
        private System.Windows.Forms.Button calendar;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button multipleControls;
        private System.Windows.Forms.Button dataGridViewButton;

        private System.Windows.Forms.Button menuesButton;
        private System.Windows.Forms.Button panelsButton;
        private System.Windows.Forms.Button splitterButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button ComboBoxesButton;
        private System.Windows.Forms.Button mdiParent;
        private System.Windows.Forms.Button propertyGrid;
        private System.Windows.Forms.Button listViewButton;
        private System.Windows.Forms.Button DateTimePickerButton;
        private System.Windows.Forms.Button FolderBrowserDialogButton;
    }
}

