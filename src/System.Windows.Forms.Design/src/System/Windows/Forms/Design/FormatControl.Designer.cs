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
            formatGroupBox = new System.Windows.Forms.GroupBox();
            tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            explanationLabel = new System.Windows.Forms.Label();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            sampleGroupBox = new System.Windows.Forms.GroupBox();
            sampleLabel = new System.Windows.Forms.Label();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            secondRowLabel = new System.Windows.Forms.Label();
            nullValueLabel = new System.Windows.Forms.Label();
            nullValueTextBox = new System.Windows.Forms.TextBox();
            decimalPlacesUpDown = new System.Windows.Forms.NumericUpDown();
            thirdRowLabel = new System.Windows.Forms.Label();
            dateTimeFormatsListBox = new System.Windows.Forms.ListBox();
            formatTypeLabel = new System.Windows.Forms.Label();
            formatTypeListBox = new System.Windows.Forms.ListBox();
            formatGroupBox.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            sampleGroupBox.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.decimalPlacesUpDown)).BeginInit();
            SuspendLayout();
            // 
            // formatGroupBox
            // 
            resources.ApplyResources(this.formatGroupBox, "formatGroupBox");
            formatGroupBox.Controls.Add(this.tableLayoutPanel3);
            formatGroupBox.Dock = DockStyle.Fill;
            formatGroupBox.Name = "formatGroupBox";
            formatGroupBox.TabStop = false;
            formatGroupBox.Enter += new System.EventHandler(this.formatGroupBox_Enter);
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            tableLayoutPanel3.Controls.Add(this.explanationLabel, 0, 0);
            tableLayoutPanel3.Controls.Add(this.tableLayoutPanel2, 1, 1);
            tableLayoutPanel3.Controls.Add(this.formatTypeLabel, 0, 1);
            tableLayoutPanel3.Controls.Add(this.formatTypeListBox, 0, 2);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // explanationLabel
            // 
            resources.ApplyResources(this.explanationLabel, "explanationLabel");
            tableLayoutPanel3.SetColumnSpan(this.explanationLabel, 2);
            explanationLabel.Name = "explanationLabel";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            tableLayoutPanel2.Controls.Add(this.sampleGroupBox, 0, 0);
            tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 1);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel3.SetRowSpan(this.tableLayoutPanel2, 2);
            // 
            // sampleGroupBox
            // 
            resources.ApplyResources(this.sampleGroupBox, "sampleGroupBox");
            sampleGroupBox.Controls.Add(this.sampleLabel);
            sampleGroupBox.MinimumSize = new System.Drawing.Size(250, 38);
            sampleGroupBox.Name = "sampleGroupBox";
            sampleGroupBox.TabStop = false;
            // 
            // sampleLabel
            // 
            resources.ApplyResources(this.sampleLabel, "sampleLabel");
            sampleLabel.Name = "sampleLabel";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            tableLayoutPanel1.Controls.Add(this.nullValueLabel, 0, 0);
            tableLayoutPanel1.Controls.Add(this.nullValueTextBox, 1, 0);
            tableLayoutPanel1.Controls.Add(this.secondRowLabel, 0, 1);
            tableLayoutPanel1.Controls.Add(this.decimalPlacesUpDown, 1, 1);
            tableLayoutPanel1.Controls.Add(this.dateTimeFormatsListBox, 0, 2);
            tableLayoutPanel1.Controls.Add(this.thirdRowLabel, 1, 2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.AccessibleName = "";
            // 
            // secondRowLabel
            // 
            resources.ApplyResources(this.secondRowLabel, "secondRowLabel");
            secondRowLabel.MinimumSize = new System.Drawing.Size(81, 14);
            secondRowLabel.Name = "secondRowLabel";
            // 
            // nullValueLabel
            // 
            resources.ApplyResources(this.nullValueLabel, "nullValueLabel");
            nullValueLabel.MinimumSize = new System.Drawing.Size(81, 14);
            nullValueLabel.Name = "nullValueLabel";
            // 
            // nullValueTextBox
            // 
            resources.ApplyResources(this.nullValueTextBox, "nullValueTextBox");
            nullValueTextBox.Name = "nullValueTextBox";
            nullValueTextBox.TextChanged += new System.EventHandler(this.nullValueTextBox_TextChanged);
            // 
            // decimalPlacesUpDown
            // 
            resources.ApplyResources(this.decimalPlacesUpDown, "decimalPlacesUpDown");
            decimalPlacesUpDown.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            decimalPlacesUpDown.Name = "decimalPlacesUpDown";
            decimalPlacesUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            decimalPlacesUpDown.ValueChanged += new System.EventHandler(this.decimalPlacesUpDown_ValueChanged);
            // 
            // thirdRowLabel
            // 
            resources.ApplyResources(this.thirdRowLabel, "thirdRowLabel");
            thirdRowLabel.Name = "thirdRowLabel";
            // 
            // dateTimeFormatsListBox
            // 
            resources.ApplyResources(this.dateTimeFormatsListBox, "dateTimeFormatsListBox");
            dateTimeFormatsListBox.FormattingEnabled = true;
            dateTimeFormatsListBox.Name = "dateTimeFormatsListBox";
            // 
            // formatTypeLabel
            // 
            resources.ApplyResources(this.formatTypeLabel, "formatTypeLabel");
            formatTypeLabel.Name = "formatTypeLabel";
            // 
            // formatTypeListBox
            // 
            resources.ApplyResources(this.formatTypeListBox, "formatTypeListBox");
            formatTypeListBox.FormattingEnabled = true;
            formatTypeListBox.Name = "formatTypeListBox";
            formatTypeListBox.SelectedIndexChanged += new System.EventHandler(this.formatTypeListBox_SelectedIndexChanged);
            // 
            // FormatControl
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(formatGroupBox);
            MinimumSize = new System.Drawing.Size(390, 237);
            Name = "FormatControl";
            Load += new System.EventHandler(this.FormatControl_Load);
            formatGroupBox.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            sampleGroupBox.ResumeLayout(false);
            sampleGroupBox.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(decimalPlacesUpDown)).EndInit();
            ResumeLayout(false);
            PerformLayout();

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
