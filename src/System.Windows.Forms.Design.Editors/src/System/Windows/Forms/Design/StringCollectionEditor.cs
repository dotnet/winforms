//------------------------------------------------------------------------------
// <copyright file="StringCollectionEditor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

#define NEEDHELPBUTTON    

/*
 */
namespace System.Windows.Forms.Design {
    using System.Design;
    using System.ComponentModel;
    using System;
    using System.Collections;
    using Microsoft.Win32;    
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;
    

    /// <include file='doc\StringCollectionEditor.uex' path='docs/doc[@for="StringCollectionEditor"]/*' />
    /// <devdoc>
    ///      The StringCollectionEditor is a collection editor that is specifically
    ///      designed to edit collections containing strings.  The collection can be
    ///      of any type that can accept a string value; we just present a string-centric
    ///      dialog for the user.
    /// </devdoc>
    internal class StringCollectionEditor : CollectionEditor {
        
        public StringCollectionEditor(Type type) : base(type) {
        }

        /// <include file='doc\StringCollectionEditor.uex' path='docs/doc[@for="StringCollectionEditor.CreateCollectionForm"]/*' />
        /// <devdoc>
        ///      Creates a new form to show the current collection.  You may inherit
        ///      from CollectionForm to provide your own form.
        /// </devdoc>
        protected override CollectionForm CreateCollectionForm() {
            return new StringCollectionForm(this);
        }

        /// <include file='doc\StringCollectionEditor.uex' path='docs/doc[@for="StringCollectionEditor.HelpTopic"]/*' />
        /// <devdoc>
        ///    <para>Gets the help topic to display for the dialog help button or pressing F1. Override to
        ///          display a different help topic.</para>
        /// </devdoc>
        protected override string HelpTopic {
            get {
                return "net.ComponentModel.StringCollectionEditor";
            }
        }
            
        /// <include file='doc\StringCollectionEditor.uex' path='docs/doc[@for="StringCollectionEditor.StringCollectionForm"]/*' />
        /// <devdoc>
        ///     StringCollectionForm allows visible editing of a string array. Each line in
        ///     the edit box is an array entry.
        /// </devdoc>
        private class StringCollectionForm : CollectionForm {
    
            private Label instruction;
            private TextBox textEntry;
            private Button okButton;
            private Button cancelButton;
            private TableLayoutPanel overarchingLayoutPanel;

            private StringCollectionEditor editor = null;

            /// <include file='doc\StringCollectionEditor.uex' path='docs/doc[@for="StringCollectionEditor.StringCollectionForm.StringCollectionForm"]/*' />
            /// <devdoc>
            ///     Constructs a StringCollectionForm.
            /// </devdoc>
            public StringCollectionForm(CollectionEditor editor) : base(editor) {
                this.editor = (StringCollectionEditor) editor;
                InitializeComponent();
                HookEvents();
            }
    
            private void Edit1_keyDown(object sender, KeyEventArgs e) {
                if (e.KeyCode == Keys.Escape) {
                    cancelButton.PerformClick();
                    e.Handled = true;
                }
            }

            private void StringCollectionEditor_HelpButtonClicked(object sender, CancelEventArgs e)
            {
                e.Cancel = true;
                editor.ShowHelp();
            }

            private void Form_HelpRequested(object sender, HelpEventArgs e) {
                editor.ShowHelp();
            }

            private void HookEvents() {
                this.textEntry.KeyDown += new KeyEventHandler(this.Edit1_keyDown);
                this.okButton.Click += new EventHandler(this.OKButton_click);
                this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.StringCollectionEditor_HelpButtonClicked);
            }

            /// <include file='doc\StringCollectionEditor.uex' path='docs/doc[@for="StringCollectionEditor.StringCollectionForm.InitializeComponent"]/*' />
            /// <devdoc>
            ///     NOTE: The following code is required by the form
            ///     designer.  It can be modified using the form editor.  Do not
            ///     modify it using the code editor.
            /// </devdoc>
            private void InitializeComponent() {
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StringCollectionEditor));
                this.instruction = new System.Windows.Forms.Label();
                this.textEntry = new System.Windows.Forms.TextBox();
                this.okButton = new System.Windows.Forms.Button();
                this.cancelButton = new System.Windows.Forms.Button();
                this.overarchingLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
                this.overarchingLayoutPanel.SuspendLayout();
                this.SuspendLayout();
                // instruction
                // 
                resources.ApplyResources(this.instruction, "instruction");
                this.overarchingLayoutPanel.SetColumnSpan(this.instruction, 2);
                this.instruction.Name = "instruction";
                // 
                // textEntry
                // 
                resources.ApplyResources(this.textEntry, "textEntry");
                this.overarchingLayoutPanel.SetColumnSpan(this.textEntry, 2);
                this.textEntry.AcceptsTab = true;
                this.textEntry.AcceptsReturn = true;
                this.textEntry.Name = "textEntry";
                // 
                // okButton
                // 
                resources.ApplyResources(this.okButton, "okButton");
                this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.okButton.Name = "okButton";
                // 
                // cancelButton
                // 
                resources.ApplyResources(this.cancelButton, "cancelButton");
                this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.cancelButton.Name = "cancelButton";
                // 
                // overarchingLayoutPanel
                // 
                resources.ApplyResources(this.overarchingLayoutPanel, "overarchingLayoutPanel");
                this.overarchingLayoutPanel.Controls.Add(this.instruction, 0, 0);
                this.overarchingLayoutPanel.Controls.Add(this.textEntry, 0, 2);
                this.overarchingLayoutPanel.Controls.Add(this.okButton, 0, 3);
                this.overarchingLayoutPanel.Controls.Add(this.cancelButton, 1, 3);
                this.overarchingLayoutPanel.Name = "overarchingLayoutPanel";
                // 
                // StringCollectionEditor
                // 
                resources.ApplyResources(this, "$this");
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.Controls.Add(this.overarchingLayoutPanel);
                this.HelpButton = true;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "StringCollectionEditor";
                this.ShowIcon = false;
                this.ShowInTaskbar = false;
                this.overarchingLayoutPanel.ResumeLayout(false);
                this.overarchingLayoutPanel.PerformLayout();
                this.HelpRequested += new HelpEventHandler(this.Form_HelpRequested);
                this.ResumeLayout(false);
                this.PerformLayout();
            }
            
            /// <include file='doc\StringCollectionEditor.uex' path='docs/doc[@for="StringCollectionEditor.StringCollectionForm.OKButton_click"]/*' />
            /// <devdoc>
            ///      Commits the changes to the editor.
            /// </devdoc>
            private void OKButton_click(object sender, EventArgs e) {
                char[] delims = new char[] {'\n'};
                char[] trims = new char[] {'\r'};
                
                string[] strings = textEntry.Text.Split(delims);
                object[] curItems = Items;

                int nItems = strings.Length;
                for (int i = 0; i < nItems; i++) {
                    strings[i] = strings[i].Trim(trims);
                }
                
                bool dirty = true;
                if (nItems == curItems.Length) {
                    int i;
                    for (i = 0; i < nItems; ++i) {
                        if (!strings[i].Equals((string)curItems[i])) {
                            break;
                        }
                    }

                    if (i == nItems)
                        dirty = false;
                }

                if (!dirty) {
                    DialogResult = DialogResult.Cancel;
                    return;
                }

                // ASURT #57372
                // If the final line is blank, we don't want to create an item from it
                //
                if (strings.Length > 0 && strings[strings.Length - 1].Length == 0) {
                    nItems--;
                }
                
                object[] values = new object[nItems];
                for (int i = 0; i < nItems; i++) {
                    values[i] = strings[i];
                }
                
                Items = values;
            }
            
            // <summary>
            //      This is called when the value property in the CollectionForm has changed.
            //      In it you should update your user interface to reflect the current value.
            // </summary>
            // </doc>
            protected override void OnEditValueChanged() {
                object[] items = Items;
                string text = string.Empty;
                
                for (int i = 0; i < items.Length; i++) {
                    if (items[i] is string) {
                        text += (string)items[i];
                        if (i != items.Length - 1) {
                            text += "\r\n";
                        }
                    }
                }
    
                textEntry.Text = text;
            }
        }
    }
}

