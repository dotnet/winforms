// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace System.Windows.Forms.Design
{
    class FormatStringDialog : Form
    {
        // we need the context for the HELP service provider
        private ITypeDescriptorContext context;

        private Button cancelButton;

        private Button okButton;

        private FormatControl formatControl1;

        private bool dirty = false;

        private DataGridViewCellStyle dgvCellStyle = null;
        private ListControl listControl = null;

        public FormatStringDialog(ITypeDescriptorContext context)
        {
            this.context = context;
            InitializeComponent();
            // vsw 532943: set right to left property according to SR.GetString(SR.RTL) value.
            string rtlString = SR.RTL;
            if (rtlString.Equals("RTL_False"))
            {
                RightToLeft = RightToLeft.No;
                RightToLeftLayout = false;
            }
            else
            {
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
            }
        }

        public DataGridViewCellStyle DataGridViewCellStyle
        {
            set
            {
                dgvCellStyle = value;
                listControl = null;
            }
        }

        public bool Dirty
        {
            get
            {
                return dirty || formatControl1.Dirty;
            }
        }

        public ListControl ListControl
        {
            set
            {
                listControl = value;
                dgvCellStyle = null;
            }
        }

        private void FormatStringDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            FormatStringDialog_HelpRequestHandled();
            e.Cancel = true;
        }

        private void FormatStringDialog_HelpRequested(object sender, HelpEventArgs e)
        {
            FormatStringDialog_HelpRequestHandled();
            e.Handled = true;
        }

        private void FormatStringDialog_HelpRequestHandled()
        {
            if (context.GetService(typeof(IHelpService)) is IHelpService helpService)
            {
                helpService.ShowHelpFromKeyword("vs.FormatStringDialog");
            }
        }

        //HACK: if we're adjusting positions after the form's loaded, we didn't set the form up correctly.
        internal void FormatControlFinishedLoading()
        {
            okButton.Top = formatControl1.Bottom + 5;
            cancelButton.Top = formatControl1.Bottom + 5;
            int formatControlRightSideOffset = GetRightSideOffset(formatControl1);
            int cancelButtonRightSideOffset = GetRightSideOffset(cancelButton);
            okButton.Left += formatControlRightSideOffset - cancelButtonRightSideOffset;
            cancelButton.Left += formatControlRightSideOffset - cancelButtonRightSideOffset;
        }

        private static int GetRightSideOffset(Control ctl)
        {
            int result = ctl.Width;
            while (ctl != null)
            {
                result += ctl.Left;
                ctl = ctl.Parent;
            }
            return result;
        }

        private void FormatStringDialog_Load(object sender, EventArgs e)
        {
            // make a reasonable guess what user control should be shown
            string formatString = dgvCellStyle != null ? dgvCellStyle.Format : listControl.FormatString;
            object nullValue = dgvCellStyle != null ? dgvCellStyle.NullValue : null;

            string formatType = string.Empty;
            if (!string.IsNullOrEmpty(formatString))
            {
                formatType = FormatControl.FormatTypeStringFromFormatString(formatString);
            }

            // the null value text box should be enabled only when editing DataGridViewCellStyle
            // when we are editing ListControl, it should be disabled
            if (dgvCellStyle != null)
            {
                formatControl1.NullValueTextBoxEnabled = true;
            }
            else
            {
                Debug.Assert(listControl != null, "we check this everywhere, but it does not hurt to check it again");
                formatControl1.NullValueTextBoxEnabled = false;
            }

            formatControl1.FormatType = formatType;

            // push the information from FormatString/FormatInfo/NullValue into the FormattingUserControl
            FormatControl.FormatTypeClass formatTypeItem = formatControl1.FormatTypeItem;
            if (formatTypeItem != null)
            {
                // parsing the FormatString uses the CultureInfo. So push the CultureInfo before push the FormatString.
                formatTypeItem.PushFormatStringIntoFormatType(formatString);
            }
            else
            {
                // make General format type the default 
                formatControl1.FormatType = SR.BindingFormattingDialogFormatTypeNoFormatting;
            }
            formatControl1.NullValue = nullValue != null ? nullValue.ToString() : "";
        }

        public void End()
        {
            // clear the tree nodes collection
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            cancelButton = new System.Windows.Forms.Button();
            okButton = new System.Windows.Forms.Button();
            formatControl1 = new FormatControl();
            SuspendLayout();
            //
            // formatControl1
            //
            formatControl1.Location = new System.Drawing.Point(10, 10);
            formatControl1.Margin = new System.Windows.Forms.Padding(0);
            formatControl1.Name = "formatControl1";
            formatControl1.Size = new System.Drawing.Size(376, 268);
            formatControl1.TabIndex = 0;
            //
            // cancelButton
            //
            cancelButton.Location = new System.Drawing.Point(299, 288);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(87, 23);
            cancelButton.TabIndex = 2;
            cancelButton.Text = SR.DataGridView_Cancel;
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Click += new System.EventHandler(cancelButton_Click);
            //
            // okButton
            //
            okButton.Location = new System.Drawing.Point(203, 288);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(87, 23);
            okButton.TabIndex = 1;
            okButton.Text = SR.DataGridView_OK;
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Click += new System.EventHandler(okButton_Click);
            // 
            // Form1
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
            ClientSize = new System.Drawing.Size(396, 295);
            AutoSize = true;
            HelpButton = true;
            MaximizeBox = false;
            MinimizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ShowInTaskbar = false;
            Icon = null;
            Name = "Form1";
            Controls.Add(okButton);
            Controls.Add(formatControl1);
            Controls.Add(cancelButton);
            Padding = new System.Windows.Forms.Padding(0);
            Text = SR.FormatStringDialogTitle;
            HelpButtonClicked += new CancelEventHandler(FormatStringDialog_HelpButtonClicked);
            HelpRequested += new HelpEventHandler(FormatStringDialog_HelpRequested);
            Load += new EventHandler(FormatStringDialog_Load);
            ResumeLayout(false);
        }

        private void cancelButton_Click(object sender, System.EventArgs e)
        {
            dirty = false;
        }

        private void okButton_Click(object sender, System.EventArgs e)
        {
            PushChanges();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if ((keyData & Keys.Modifiers) == 0)
            {
                switch (keyData & Keys.KeyCode)
                {
                    case Keys.Enter:
                        DialogResult = DialogResult.OK;
                        PushChanges();
                        Close();
                        return true;
                    case Keys.Escape:
                        dirty = false;
                        DialogResult = DialogResult.Cancel;
                        Close();
                        return true;
                    default:
                        return base.ProcessDialogKey(keyData);
                }
            }
            else
            {
                return base.ProcessDialogKey(keyData);
            }
        }

        private void PushChanges()
        {
            FormatControl.FormatTypeClass formatTypeItem = formatControl1.FormatTypeItem;
            if (formatTypeItem != null)
            {
                if (dgvCellStyle != null)
                {
                    dgvCellStyle.Format = formatTypeItem.FormatString;
                    dgvCellStyle.NullValue = formatControl1.NullValue;
                }
                else
                {
                    listControl.FormatString = formatTypeItem.FormatString;
                }
                dirty = true;
            }
        }
    }
}
