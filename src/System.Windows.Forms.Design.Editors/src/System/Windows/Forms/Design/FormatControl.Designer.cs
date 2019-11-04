// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    internal partial class FormatControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormatControl));
            this.formatGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.explanationLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.sampleGroupBox = new System.Windows.Forms.GroupBox();
            this.sampleLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.secondRowLabel = new System.Windows.Forms.Label();
            this.nullValueLabel = new System.Windows.Forms.Label();
            this.nullValueTextBox = new System.Windows.Forms.TextBox();
            this.decimalPlacesUpDown = new System.Windows.Forms.NumericUpDown();
            this.thirdRowLabel = new System.Windows.Forms.Label();
            this.dateTimeFormatsListBox = new System.Windows.Forms.ListBox();
            this.formatTypeLabel = new System.Windows.Forms.Label();
            this.formatTypeListBox = new System.Windows.Forms.ListBox();
            this.formatGroupBox.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.sampleGroupBox.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.decimalPlacesUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // formatGroupBox
            // 
            resources.ApplyResources(this.formatGroupBox, "formatGroupBox");
            this.formatGroupBox.Controls.Add(this.tableLayoutPanel3);
            this.formatGroupBox.Dock = DockStyle.Fill;
            this.formatGroupBox.Name = "formatGroupBox";
            this.formatGroupBox.TabStop = false;
            this.formatGroupBox.Enter += new System.EventHandler(this.formatGroupBox_Enter);
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.explanationLabel, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel2, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.formatTypeLabel, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.formatTypeListBox, 0, 2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // explanationLabel
            // 
            resources.ApplyResources(this.explanationLabel, "explanationLabel");
            this.tableLayoutPanel3.SetColumnSpan(this.explanationLabel, 2);
            this.explanationLabel.Name = "explanationLabel";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.sampleGroupBox, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel3.SetRowSpan(this.tableLayoutPanel2, 2);
            // 
            // sampleGroupBox
            // 
            resources.ApplyResources(this.sampleGroupBox, "sampleGroupBox");
            this.sampleGroupBox.Controls.Add(this.sampleLabel);
            this.sampleGroupBox.MinimumSize = new System.Drawing.Size(250, 38);
            this.sampleGroupBox.Name = "sampleGroupBox";
            this.sampleGroupBox.TabStop = false;
            // 
            // sampleLabel
            // 
            resources.ApplyResources(this.sampleLabel, "sampleLabel");
            this.sampleLabel.Name = "sampleLabel";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.nullValueLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.nullValueTextBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.secondRowLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.decimalPlacesUpDown, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.dateTimeFormatsListBox, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.thirdRowLabel, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.AccessibleName = "";
            // 
            // secondRowLabel
            // 
            resources.ApplyResources(this.secondRowLabel, "secondRowLabel");
            this.secondRowLabel.MinimumSize = new System.Drawing.Size(81, 14);
            this.secondRowLabel.Name = "secondRowLabel";
            // 
            // nullValueLabel
            // 
            resources.ApplyResources(this.nullValueLabel, "nullValueLabel");
            this.nullValueLabel.MinimumSize = new System.Drawing.Size(81, 14);
            this.nullValueLabel.Name = "nullValueLabel";
            // 
            // nullValueTextBox
            // 
            resources.ApplyResources(this.nullValueTextBox, "nullValueTextBox");
            this.nullValueTextBox.Name = "nullValueTextBox";
            this.nullValueTextBox.TextChanged += new System.EventHandler(this.nullValueTextBox_TextChanged);
            // 
            // decimalPlacesUpDown
            // 
            resources.ApplyResources(this.decimalPlacesUpDown, "decimalPlacesUpDown");
            this.decimalPlacesUpDown.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.decimalPlacesUpDown.Name = "decimalPlacesUpDown";
            this.decimalPlacesUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.decimalPlacesUpDown.ValueChanged += new System.EventHandler(this.decimalPlacesUpDown_ValueChanged);
            // 
            // thirdRowLabel
            // 
            resources.ApplyResources(this.thirdRowLabel, "thirdRowLabel");
            this.thirdRowLabel.Name = "thirdRowLabel";
            // 
            // dateTimeFormatsListBox
            // 
            resources.ApplyResources(this.dateTimeFormatsListBox, "dateTimeFormatsListBox");
            this.dateTimeFormatsListBox.FormattingEnabled = true;
            this.dateTimeFormatsListBox.Name = "dateTimeFormatsListBox";
            // 
            // formatTypeLabel
            // 
            resources.ApplyResources(this.formatTypeLabel, "formatTypeLabel");
            this.formatTypeLabel.Name = "formatTypeLabel";
            // 
            // formatTypeListBox
            // 
            resources.ApplyResources(this.formatTypeListBox, "formatTypeListBox");
            this.formatTypeListBox.FormattingEnabled = true;
            this.formatTypeListBox.Name = "formatTypeListBox";
            this.formatTypeListBox.SelectedIndexChanged += new System.EventHandler(this.formatTypeListBox_SelectedIndexChanged);
            // 
            // FormatControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.formatGroupBox);
            this.MinimumSize = new System.Drawing.Size(390, 237);
            this.Name = "FormatControl";
            this.Load += new System.EventHandler(this.FormatControl_Load);
            this.formatGroupBox.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.sampleGroupBox.ResumeLayout(false);
            this.sampleGroupBox.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.decimalPlacesUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox formatGroupBox;
        private System.Windows.Forms.Label explanationLabel;
        private System.Windows.Forms.Label formatTypeLabel;
        private System.Windows.Forms.ListBox formatTypeListBox;
        private System.Windows.Forms.GroupBox sampleGroupBox;
        private System.Windows.Forms.Label sampleLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label nullValueLabel;
        private System.Windows.Forms.Label secondRowLabel;
        private System.Windows.Forms.TextBox nullValueTextBox;
        private System.Windows.Forms.Label thirdRowLabel;
        private System.Windows.Forms.ListBox dateTimeFormatsListBox;
        private System.Windows.Forms.NumericUpDown decimalPlacesUpDown;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
    }
}
