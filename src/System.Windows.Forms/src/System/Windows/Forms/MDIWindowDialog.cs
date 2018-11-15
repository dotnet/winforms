// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using Microsoft.Win32;


    /// <include file='doc\MDIWindowDialog.uex' path='docs/doc[@for="MdiWindowDialog"]/*' />
    /// <devdoc>
    /// </devdoc>
    /// <internalonly/>
    [
        System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)
    ]
    internal sealed class MdiWindowDialog : Form {
        private System.Windows.Forms.ListBox itemList;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TableLayoutPanel okCancelTableLayoutPanel;
        Form active;

        public MdiWindowDialog()
            : base() {

            InitializeComponent();
        }

        public Form ActiveChildForm {
            get {
#if DEBUG
                ListItem item = (ListItem)itemList.SelectedItem;
                Debug.Assert(item != null, "No item selected!");
#endif
                return active;
            }
        }


        /// <include file='doc\MDIWindowDialog.uex' path='docs/doc[@for="MdiWindowDialog.ListItem"]/*' />
        /// <devdoc>
        /// </devdoc>
        private class ListItem {
            public Form form;

            public ListItem(Form f) {
                form = f;
            }

            public override string ToString() {
                return form.Text;
            }
        }

        public void SetItems(Form active, Form[] all) {
            int selIndex = 0;
            for (int i=0; i<all.Length; i++) {
                // Don't list non-visible windows
                if(all[i].Visible) {
                    int n = itemList.Items.Add(new ListItem(all[i]));
                    if (all[i].Equals(active)) {
                        selIndex = n;
                    }
                }
            }
            this.active = active;
            itemList.SelectedIndex = selIndex;
        }

        private void ItemList_doubleClick(object source, EventArgs e) {
            okButton.PerformClick();
        }

        private void ItemList_selectedIndexChanged(object source, EventArgs e) {
            ListItem item = (ListItem)itemList.SelectedItem;
            if (item != null) {
                active = item.form;
            }
        }

        /// <include file='doc\MDIWindowDialog.uex' path='docs/doc[@for="MdiWindowDialog.components"]/*' />
        /// <devdoc>
        ///     NOTE: The following code is required by the Windows Forms
        ///     designer.  It can be modified using the form editor.  Do not
        ///     modify it using the code editor.
        /// </devdoc>

        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MdiWindowDialog));
            this.itemList = new System.Windows.Forms.ListBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okCancelTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.okCancelTableLayoutPanel.SuspendLayout();
            this.itemList.DoubleClick += new System.EventHandler(this.ItemList_doubleClick);
            this.itemList.SelectedIndexChanged += new EventHandler(this.ItemList_selectedIndexChanged);
            this.SuspendLayout();
// 
// itemList
// 
            resources.ApplyResources(this.itemList, "itemList");
            this.itemList.FormattingEnabled = true;
            this.itemList.Name = "itemList";
// 
// okButton
// 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.okButton.Name = "okButton";
// 
// cancelButton
// 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.cancelButton.Name = "cancelButton";
// 
// okCancelTableLayoutPanel
// 
            resources.ApplyResources(this.okCancelTableLayoutPanel, "okCancelTableLayoutPanel");
            this.okCancelTableLayoutPanel.ColumnCount = 2;
            this.okCancelTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.okCancelTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.okCancelTableLayoutPanel.Controls.Add(this.okButton, 0, 0);
            this.okCancelTableLayoutPanel.Controls.Add(this.cancelButton, 1, 0);
            this.okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";
            this.okCancelTableLayoutPanel.RowCount = 1;
            this.okCancelTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
// 
// MdiWindowDialog
// 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.okCancelTableLayoutPanel);
            this.Controls.Add(this.itemList);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MdiWindowDialog";
            this.ShowIcon = false;
            this.okCancelTableLayoutPanel.ResumeLayout(false);
            this.okCancelTableLayoutPanel.PerformLayout();
            this.AcceptButton = this.okButton;
            this.CancelButton = this.cancelButton;

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
