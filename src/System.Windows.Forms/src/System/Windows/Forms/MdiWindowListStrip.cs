// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Globalization;
    
    /// <devdoc> this is the menu that merges into the MdiWindowListItem
    ///          in an MDI parent when an MDI child is maximized.
    /// </devdoc>
    internal class MdiWindowListStrip : MenuStrip {

        private Form mdiParent = null;
        private ToolStripMenuItem mergeItem;
        private MenuStrip mergedMenu;
        
        public MdiWindowListStrip() {
        }

        protected override void Dispose(bool disposing) {
           if (disposing) {
                mdiParent = null;
           }
           base.Dispose(disposing);
        }

        internal ToolStripMenuItem MergeItem {
            get {
                if (mergeItem == null) {
                    mergeItem = new ToolStripMenuItem();
                    mergeItem.MergeAction = MergeAction.MatchOnly;
                }

                if (mergeItem.Owner == null) {
                    this.Items.Add(mergeItem);
                }
                return mergeItem;
            }
        }

        internal MenuStrip MergedMenu {
            get {
                return mergedMenu;
            }
            set {
                mergedMenu = value;
            }
        }

        /// <devdoc> Given a form, the items on this toolstrip populate with the mdi children 
        ///     with mnemonics 1-9 and More Windows menu item.
        ///     These items can then be merged into a menustrip.
        ///
        ///     Based on similar code in MenuItem.cs::PopulateMdiList(), which is unfortunately just different 
        ///     enough in its working environment that we can't readily combine the two.
        ///     But if you're fixing something here, chances are that the same issue will need scrutiny over there.
        ///</devdoc>
        public void PopulateItems(Form mdiParent, ToolStripMenuItem mdiMergeItem, bool includeSeparator) {
            this.mdiParent = mdiParent;
            this.SuspendLayout();
            MergeItem.DropDown.SuspendLayout();
            try {
                ToolStripMenuItem mergeItem = MergeItem;
                mergeItem.DropDownItems.Clear();
                mergeItem.Text = mdiMergeItem.Text;

                Form[] forms = mdiParent.MdiChildren;
                if(forms != null && forms.Length != 0){

                    if(includeSeparator) {
                        ToolStripSeparator separator = new ToolStripSeparator();
                        separator.MergeAction = MergeAction.Append;
                        separator.MergeIndex = -1;
                        mergeItem.DropDownItems.Add(separator);
                    }
                    
                    Form activeMdiChild = mdiParent.ActiveMdiChild;
                    
                    const int maxMenuForms = 9;  // max number of Window menu items for forms
                    int visibleChildren = 0;     // number of visible child forms (so we know if we need to show More Windows...
                    int accel = 1;               // prefix the form name with this digit, underlined, as an accelerator
                    int formsAddedToMenu = 0;
                    bool activeFormAdded = false;

                    for (int i = 0; i < forms.Length; i++) {
                        // we need to check close reason here because we could be getting called
                        // here in the midst of a WM_CLOSE - WM_MDIDESTROY eventually fires a WM_MDIACTIVATE
                        if (forms[i].Visible && (forms[i].CloseReason == CloseReason.None)) {
                            visibleChildren++;
                            if ((activeFormAdded && (formsAddedToMenu < maxMenuForms))     ||   // don't exceed max
                                (!activeFormAdded && (formsAddedToMenu < (maxMenuForms-1)) ||   // save room for active if it's not in yet
                                (forms[i].Equals(activeMdiChild)))){                            // there's always room for activeMdiChild
                                string text =  WindowsFormsUtils.EscapeTextWithAmpersands(mdiParent.MdiChildren[i].Text);
                                text = (text == null) ? String.Empty : text;
                                ToolStripMenuItem windowListItem = new ToolStripMenuItem(mdiParent.MdiChildren[i]);
                                windowListItem.Text = String.Format(CultureInfo.CurrentCulture, "&{0} {1}", accel, text);
                                windowListItem.MergeAction = MergeAction.Append;
                                windowListItem.MergeIndex = accel;
                                windowListItem.Click += new EventHandler(OnWindowListItemClick);
                                if (forms[i].Equals(activeMdiChild)) {  // if this the active one, check it off.
                                    windowListItem.Checked = true;
                                    activeFormAdded = true;
                                }
                                accel++;
                                formsAddedToMenu++;
                                Debug.WriteLineIf(ToolStrip.MDIMergeDebug.TraceVerbose,"\tPopulateItems: Added " + windowListItem.Text);
                                mergeItem.DropDownItems.Add(windowListItem);
                            }
                        }
                    }

                    // show the More Windows... item if necessary.
                    if (visibleChildren > maxMenuForms) {
                        ToolStripMenuItem moreWindowsMenuItem = new ToolStripMenuItem();
                        moreWindowsMenuItem.Text = SR.MDIMenuMoreWindows;
                        Debug.WriteLineIf(ToolStrip.MDIMergeDebug.TraceVerbose, "\tPopulateItems: Added " + moreWindowsMenuItem.Text);
                        moreWindowsMenuItem.Click += new EventHandler(OnMoreWindowsMenuItemClick);
                        moreWindowsMenuItem.MergeAction = MergeAction.Append;
                        mergeItem.DropDownItems.Add(moreWindowsMenuItem);
                    }
                }
            }
            finally {
                // this is an invisible toolstrip dont even bother doing layout.
                this.ResumeLayout(false);
                MergeItem.DropDown.ResumeLayout(false);
            }
        }

        /// <devdoc> handler for More Windows... This is similar to MenuItem.cs</devdoc>
        private void OnMoreWindowsMenuItemClick(object sender, EventArgs e) {

             Form[] forms = mdiParent.MdiChildren;
             
             if (forms != null) {
                // 


                IntSecurity.AllWindows.Assert();
                try {
                    using (MdiWindowDialog dialog = new MdiWindowDialog()) {
                        dialog.SetItems(mdiParent.ActiveMdiChild, forms);
                        DialogResult result = dialog.ShowDialog();
                        if (result == DialogResult.OK) {

                            // AllWindows Assert above allows this...
                            //
                            dialog.ActiveChildForm.Activate();
                            if (dialog.ActiveChildForm.ActiveControl != null && !dialog.ActiveChildForm.ActiveControl.Focused) {
                                dialog.ActiveChildForm.ActiveControl.Focus();
                            }
                        }
                    }
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }
            }
        }

        /// <devdoc> handler for 1 - 9.  This is similar to MenuItem.cs</devdoc>
        private void OnWindowListItemClick(object sender, EventArgs e) {
            ToolStripMenuItem windowListItem = sender as ToolStripMenuItem;

            if (windowListItem != null) {
                Form boundForm = windowListItem.MdiForm;
            
                if (boundForm != null) {
                    // 


                    IntSecurity.ModifyFocus.Assert();
                    try {
                        boundForm.Activate();
                        if (boundForm.ActiveControl != null && !boundForm.ActiveControl.Focused) {
                            boundForm.ActiveControl.Focus();
                        }
                    }
                    finally {
                        CodeAccessPermission.RevertAssert();
                    }
                        
                }
            }
        }
    }
}
