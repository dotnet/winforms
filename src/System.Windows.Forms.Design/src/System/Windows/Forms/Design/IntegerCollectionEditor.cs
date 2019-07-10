// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  The <see cref="IntegerCollectionEditor"/> is a collection editor that is specifically
    ///  designed to edit collections containing Integers.  
    /// </summary>
    internal class IntegerCollectionEditor : CollectionEditor
    {
        public IntegerCollectionEditor(Type type)
            : base(type)
        {
        }

        /// <summary>
        ///  Creates a new form to show the current collection. 
        ///  You may inherit from CollectionForm to provide your own form.
        /// </summary>
        protected override CollectionForm CreateCollectionForm() => new IntegerCollectionForm(this);

        /// <summary>
        ///<para>Gets the help topic to display for the dialog help button or pressing F1. 
        ///  Override to display a different help topic.</para>
        /// </summary>
        protected override string HelpTopic => "net.ComponentModel.IntegerCollectionEditor";

        /// <summary>
        /// IntegerCollectionEditor allows visible editing of a integer array.
        /// Each line in the edit box is an array entry.
        /// </summary>
        private class IntegerCollectionForm : CollectionForm
        {
            private readonly IntegerCollectionEditor _editor;
            private readonly Label _instruction = new Label();
            private readonly TextBox _textEntry = new TextBox();
            private readonly Button _okButton = new Button();
            private readonly Button _cancelButton = new Button();
            private readonly Button _helpButton = new Button();

            /// <summary>
            /// Constructs a StringCollectionForm.
            /// </summary>
            public IntegerCollectionForm(CollectionEditor editor)
                : base(editor)
            {
                _editor = (IntegerCollectionEditor)editor;
                InitializeComponent();
            }

            private void Edit1_keyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode != Keys.Escape)
                {
                    return;
                }

                _cancelButton.PerformClick();
                e.Handled = true;
            }

            private void HelpButton_click(object sender, EventArgs e) => _editor.ShowHelp();

            private void Form_HelpRequested(object sender, HelpEventArgs e) => _editor.ShowHelp();

            /// <summary>
            ///  NOTE: The following code is required by the form designer. It can be modified using the form editor.  
            ///  Do not modify it using the code editor.
            /// </summary>
            private void InitializeComponent()
            {
                _instruction.Location = new Point(4, 7);
                _instruction.Size = new Size(422, 14);
                _instruction.TabIndex = 0;
                _instruction.TabStop = false;
                _instruction.Text = SR.IntegerCollectionEditorInstruction;

                _textEntry.Location = new Point(4, 22);
                _textEntry.Size = new Size(422, 244);
                _textEntry.TabIndex = 0;
                _textEntry.Text = "";
                _textEntry.AcceptsTab = false;
                _textEntry.AcceptsReturn = true;
                _textEntry.AutoSize = false;
                _textEntry.Multiline = true;
                _textEntry.ScrollBars = ScrollBars.Both;
                _textEntry.WordWrap = false;
                _textEntry.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
                _textEntry.KeyDown += new KeyEventHandler(Edit1_keyDown);

                _okButton.Location = new Point(185, 274);
                _okButton.Size = new Size(75, 23);
                _okButton.TabIndex = 1;
                _okButton.Text = SR.IntegerCollectionEditorOKCaption;
                _okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                _okButton.DialogResult = DialogResult.OK;
                _okButton.Click += new EventHandler(OKButton_click);

                _cancelButton.Location = new Point(264, 274);
                _cancelButton.Size = new Size(75, 23);
                _cancelButton.TabIndex = 2;
                _cancelButton.Text = SR.IntegerCollectionEditorCancelCaption;
                _cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                _cancelButton.DialogResult = DialogResult.Cancel;

                _helpButton.Location = new Point(343, 274);
                _helpButton.Size = new Size(75, 23);
                _helpButton.TabIndex = 3;
                _helpButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                _helpButton.Text = SR.IntegerCollectionEditorHelpCaption;
                _helpButton.Click += new EventHandler(HelpButton_click);
                HelpRequested += new HelpEventHandler(Form_HelpRequested);

                Location = new Point(7, 7);
                Text = SR.IntegerCollectionEditorTitle;
                AcceptButton = _okButton;
                AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                AutoScaleDimensions = new SizeF(6, 13);
                CancelButton = _cancelButton;
                ClientSize = new Size(429, 307);
                MaximizeBox = false;
                MinimizeBox = false;
                ControlBox = false;
                ShowInTaskbar = false;
                // VSWhidbey 94233: Force the dialog to open using FormStartPosition.CenterScreen.
                StartPosition = FormStartPosition.CenterScreen;
                MinimumSize = new Size(300, 200);

                Controls.Clear();
                Controls.AddRange(new Control[] {
                                        _instruction,
                                        _textEntry,
                                        _okButton,
                                        _cancelButton,
                                        _helpButton
                                        });
            }

            /// <include file='doc\IntegerCollectionEditor.uex' path='docs/doc[@for="IntegerCollectionEditor.IntegerCollectionForm.OKButton_click"]/*' />
            /// <summary>
            ///  Commits the changes to the editor.
            /// </summary>
            private void OKButton_click(object sender, EventArgs e)
            {
                char[] delims = new char[] { '\n' };
                char[] trims = new char[] { '\r' };

                string[] strings = _textEntry.Text.Split(delims);
                object[] curItems = Items;

                int nItems = strings.Length;
                // If the final line is blank, we don't want to create an item from it
                //
                if (strings.Length > 0 && strings[strings.Length - 1].Length == 0)
                {
                    nItems--;
                }
                int[] currentValues = new int[nItems];

                for (int i = 0; i < nItems; i++)
                {
                    strings[i] = strings[i].Trim(trims);
                    try
                    {
                        currentValues[i] = Int32.Parse(strings[i], CultureInfo.CurrentCulture);
                    }
                    catch (Exception ex)
                    {
                        DisplayError(ex);
                        if (ClientUtils.IsCriticalException(ex))
                        {
                            throw;
                        }
                    }

                }

                bool dirty = true;

                if (nItems == curItems.Length)
                {
                    int i;
                    for (i = 0; i < nItems; ++i)
                    {
                        if (!currentValues[i].Equals((int)curItems[i]))
                        {
                            break;
                        }
                    }

                    if (i == nItems)
                    {
                        dirty = false;
                    }
                }

                if (!dirty)
                {
                    DialogResult = DialogResult.Cancel;
                    return;
                }

                object[] values = new object[nItems];
                for (int i = 0; i < nItems; i++)
                {
                    values[i] = currentValues[i];
                }

                Items = values;
            }

            // <summary>
            //  This is called when the value property in the CollectionForm has changed.
            //  In it you should update your user interface to reflect the current value.
            // </summary>
            // </doc>
            protected override void OnEditValueChanged()
            {
                object[] items = Items;
                string text = string.Empty;

                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] is int)
                    {
                        text += ((Int32)items[i]).ToString(CultureInfo.CurrentCulture);
                        if (i != items.Length - 1)
                        {
                            text += "\r\n";
                        }
                    }
                }

                _textEntry.Text = text;
            }
        }
    }
}


