// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Windows.Forms
{
    internal sealed class MdiWindowDialog : Form
    {
        private ListBox itemList;
        private Button okButton;
        private Button cancelButton;
        private TableLayoutPanel okCancelTableLayoutPanel;
        Form active;

        public MdiWindowDialog()
            : base()
        {
            InitializeComponent();
        }

        public Form ActiveChildForm
        {
            get
            {
#if DEBUG
                ListItem item = (ListItem)itemList.SelectedItem;
                Debug.Assert(item != null, "No item selected!");
#endif
                return active;
            }
        }

        private class ListItem
        {
            public Form form;

            public ListItem(Form f)
            {
                form = f;
            }

            public override string ToString()
            {
                return form.Text;
            }
        }

        public void SetItems(Form active, Form[] all)
        {
            int selIndex = 0;
            for (int i = 0; i < all.Length; i++)
            {
                // Don't list non-visible windows
                if (all[i].Visible)
                {
                    int n = itemList.Items.Add(new ListItem(all[i]));
                    if (all[i].Equals(active))
                    {
                        selIndex = n;
                    }
                }
            }
            this.active = active;
            itemList.SelectedIndex = selIndex;
        }

        private void ItemList_doubleClick(object source, EventArgs e)
        {
            okButton.PerformClick();
        }

        private void ItemList_selectedIndexChanged(object source, EventArgs e)
        {
            ListItem item = (ListItem)itemList.SelectedItem;
            if (item != null)
            {
                active = item.form;
            }
        }

        /// <summary>
        ///  NOTE: The following code is required by the Windows Forms
        ///  designer.  It can be modified using the form editor.  Do not
        ///  modify it using the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MdiWindowDialog));
            itemList = new ListBox();
            okButton = new Button();
            cancelButton = new Button();
            okCancelTableLayoutPanel = new TableLayoutPanel();
            okCancelTableLayoutPanel.SuspendLayout();
            itemList.DoubleClick += new EventHandler(ItemList_doubleClick);
            itemList.SelectedIndexChanged += new EventHandler(ItemList_selectedIndexChanged);
            SuspendLayout();
            //
            // itemList
            //
            resources.ApplyResources(itemList, "itemList");
            itemList.FormattingEnabled = true;
            itemList.Name = "itemList";
            //
            // okButton
            //
            resources.ApplyResources(okButton, "okButton");
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Margin = new Padding(0, 0, 3, 0);
            okButton.Name = "okButton";
            //
            // cancelButton
            //
            resources.ApplyResources(cancelButton, "cancelButton");
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Margin = new Padding(3, 0, 0, 0);
            cancelButton.Name = "cancelButton";
            //
            // okCancelTableLayoutPanel
            //
            resources.ApplyResources(okCancelTableLayoutPanel, "okCancelTableLayoutPanel");
            okCancelTableLayoutPanel.ColumnCount = 2;
            okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            okCancelTableLayoutPanel.Controls.Add(okButton, 0, 0);
            okCancelTableLayoutPanel.Controls.Add(cancelButton, 1, 0);
            okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";
            okCancelTableLayoutPanel.RowCount = 1;
            okCancelTableLayoutPanel.RowStyles.Add(new RowStyle());
            //
            // MdiWindowDialog
            //
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(okCancelTableLayoutPanel);
            Controls.Add(itemList);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MdiWindowDialog";
            ShowIcon = false;
            okCancelTableLayoutPanel.ResumeLayout(false);
            okCancelTableLayoutPanel.PerformLayout();
            AcceptButton = okButton;
            CancelButton = cancelButton;

            ResumeLayout(false);
            PerformLayout();
        }
    }
}
