// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using static System.Windows.Forms.Design.UnsafeNativeMethods;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  The StringCollectionEditor is a collection editor that is specifically
    ///  designed to edit collections containing strings.  The collection can be
    ///  of any type that can accept a string value; we just present a string-centric
    ///  dialog for the user.
    /// </summary>
    internal class StringCollectionEditor : CollectionEditor
    {
        public StringCollectionEditor(Type type)
            : base(type)
        {
        }

        /// <summary>
        ///  Creates a new form to show the current collection.
        ///  You may inherit from CollectionForm to provide your own form.
        /// </summary>
        protected override CollectionForm CreateCollectionForm() => new StringCollectionForm(this);

        /// <summary>
        ///  Gets the help topic to display for the dialog help button or pressing F1.
        ///  Override to display a different help topic.
        /// </summary>
        protected override string HelpTopic => "net.ComponentModel.StringCollectionEditor";

        /// <summary>
        ///  StringCollectionForm allows visible editing of a string array.
        ///  Each line in the edit box is an array entry.
        /// </summary>
        private class StringCollectionForm : CollectionForm
        {
            private Label _instruction;
            private TextBox _textEntry;
            private Button _okButton;
            private Button _cancelButton;
            private TableLayoutPanel _overarchingLayoutPanel;

            private StringCollectionEditor _editor = null;

            /// <summary>
            ///  Constructs a StringCollectionForm.
            /// </summary>
            public StringCollectionForm(CollectionEditor editor)
                : base(editor)
            {
                _editor = (StringCollectionEditor)editor;
                InitializeComponent();
                HookEvents();
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

            private void StringCollectionEditor_HelpButtonClicked(object sender, CancelEventArgs e)
            {
                e.Cancel = true;
                _editor.ShowHelp();
            }

            private void Form_HelpRequested(object sender, HelpEventArgs e)
            {
                _editor.ShowHelp();
            }

            private void HookEvents()
            {
                _textEntry.KeyDown += new KeyEventHandler(Edit1_keyDown);
                _okButton.Click += new EventHandler(OKButton_click);
                HelpButtonClicked += new CancelEventHandler(StringCollectionEditor_HelpButtonClicked);
            }

            /// <summary>
            ///  NOTE: The following code is required by the form designer.
            ///  It can be modified using the form editor.  Do not modify it using the code editor.
            /// </summary>
            private void InitializeComponent()
            {
                ComponentResourceManager resources = new ComponentResourceManager(typeof(StringCollectionEditor));
                _instruction = new Label();
                _textEntry = new TextBox();
                _okButton = new Button();
                _cancelButton = new Button();
                _overarchingLayoutPanel = new TableLayoutPanel();
                _overarchingLayoutPanel.SuspendLayout();
                SuspendLayout();
                // instruction
                //
                resources.ApplyResources(_instruction, "instruction");
                _overarchingLayoutPanel.SetColumnSpan(_instruction, 2);
                _instruction.Name = "instruction";
                //
                // textEntry
                //
                resources.ApplyResources(_textEntry, "textEntry");
                _overarchingLayoutPanel.SetColumnSpan(_textEntry, 2);
                _textEntry.AcceptsTab = true;
                _textEntry.AcceptsReturn = true;
                _textEntry.Name = "textEntry";
                //
                // okButton
                //
                resources.ApplyResources(_okButton, "okButton");
                _okButton.DialogResult = Forms.DialogResult.OK;
                _okButton.Name = "okButton";
                //
                // cancelButton
                //
                resources.ApplyResources(_cancelButton, "cancelButton");
                _cancelButton.DialogResult = Forms.DialogResult.Cancel;
                _cancelButton.Name = "cancelButton";
                //
                // overarchingLayoutPanel
                //
                resources.ApplyResources(_overarchingLayoutPanel, "overarchingLayoutPanel");
                _overarchingLayoutPanel.Controls.Add(_instruction, 0, 0);
                _overarchingLayoutPanel.Controls.Add(_textEntry, 0, 2);
                _overarchingLayoutPanel.Controls.Add(_okButton, 0, 3);
                _overarchingLayoutPanel.Controls.Add(_cancelButton, 1, 3);
                _overarchingLayoutPanel.Name = "overarchingLayoutPanel";
                //
                // StringCollectionEditor
                //
                resources.ApplyResources(this, "$this");
                AutoScaleMode = Forms.AutoScaleMode.Font;
                Controls.Add(_overarchingLayoutPanel);
                HelpButton = true;
                MaximizeBox = false;
                MinimizeBox = false;
                Name = "StringCollectionEditor";
                ShowIcon = false;
                ShowInTaskbar = false;
                _overarchingLayoutPanel.ResumeLayout(false);
                _overarchingLayoutPanel.PerformLayout();
                HelpRequested += new HelpEventHandler(Form_HelpRequested);
                ResumeLayout(false);
                PerformLayout();
            }

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
                for (int i = 0; i < nItems; i++)
                {
                    strings[i] = strings[i].Trim(trims);
                }

                bool dirty = true;
                if (nItems == curItems.Length)
                {
                    int i;
                    for (i = 0; i < nItems; ++i)
                    {
                        if (!strings[i].Equals((string)curItems[i]))
                        {
                            break;
                        }
                    }

                    if (i == nItems)
                        dirty = false;
                }

                if (!dirty)
                {
                    DialogResult = DialogResult.Cancel;
                    return;
                }

                // If the final line is blank, we don't want to create an item from it
                if (strings.Length > 0 && strings[strings.Length - 1].Length == 0)
                {
                    nItems--;
                }

                object[] values = new object[nItems];
                for (int i = 0; i < nItems; i++)
                {
                    values[i] = strings[i];
                }

                Items = values;
            }

            // <summary>
            //      This is called when the value property in the CollectionForm has changed.
            //      In it you should update your user interface to reflect the current value.
            // </summary>
            // </doc>
            protected override void OnEditValueChanged()
            {
                _textEntry.Text = string.Join("\r\n", Items);
            }

            public class TextBoxUiaProvider : IRawElementProviderSimple
            {
                private TextBox _owningTextBox;
                private AccessibleObject _defaultAccessibilityObject;
                private bool notificationEventAvailable = true;

                public TextBoxUiaProvider(TextBox owningTextBox, AccessibleObject accessibilityObject)
                {
                    _owningTextBox = owningTextBox;
                    _defaultAccessibilityObject = accessibilityObject;
                }

                public IRawElementProviderSimple HostRawElementProvider
                {
                    get
                    {
                        IRawElementProviderSimple provider;
                        UnsafeNativeMethods.UiaHostProviderFromHwnd(new Runtime.InteropServices.HandleRef(this, _owningTextBox.Handle), out provider);
                        return provider;
                    }
                }

                public ProviderOptions ProviderOptions
                {
                    get
                    {
                        return ProviderOptions.ServerSideProvider;
                    }
                }

                public object GetPatternProvider(int patternId)
                {
                    return null;
                }

                public object GetPropertyValue(int propertyId)
                {
                    switch (propertyId)
                    {
                        case NativeMethods.UIA_ControlTypePropertyId:
                            return NativeMethods.UIA_EditControlTypeId;
                        case NativeMethods.UIA_NamePropertyId:
                            return _defaultAccessibilityObject.Name;
                        case NativeMethods.UIA_AccessKeyPropertyId:
                            return _defaultAccessibilityObject.KeyboardShortcut ?? string.Empty;
                        case NativeMethods.UIA_HasKeyboardFocusPropertyId:
                            return (_defaultAccessibilityObject.State & AccessibleStates.Focused) == AccessibleStates.Focused;
                        case NativeMethods.UIA_IsKeyboardFocusablePropertyId:
                            return (_defaultAccessibilityObject.State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                        case NativeMethods.UIA_IsEnabledPropertyId:
                            return _owningTextBox.Enabled;
                        case NativeMethods.UIA_HelpTextPropertyId:
                            return _defaultAccessibilityObject.Help ?? string.Empty;
                        case NativeMethods.UIA_IsPasswordPropertyId:
                            return false;
                        case NativeMethods.UIA_IsOffscreenPropertyId:
                            return (_defaultAccessibilityObject.State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
                    }

                    return null;
                }

                public int[] GetRuntimeId()
                {
                    var runtimeId = new int[2];
                    runtimeId[0] = 0x2a;
                    runtimeId[1] = (int)(long)_owningTextBox.Handle;
                    return runtimeId;
                }

                public bool RaiseAutomationEvent(int eventId)
                {
                    if (UnsafeNativeMethods.UiaClientsAreListening())
                    {
                        int result = UnsafeNativeMethods.UiaRaiseAutomationEvent(this, eventId);
                        return result == NativeMethods.S_OK;
                    }

                    return false;
                }

                public bool RaiseAutomationNotification(
                    NativeMethods.AutomationNotificationKind notificationKind,
                    NativeMethods.AutomationNotificationProcessing notificationProcessing,
                    string notificationText)
                {
                    if (!notificationEventAvailable)
                    {
                        return false;
                    }

                    if (!UnsafeNativeMethods.UiaClientsAreListening())
                    {
                        return false;
                    }

                    int result = NativeMethods.S_FALSE;
                    try
                    {
                        // The activityId can be any string. It cannot be null. It isn’t used currently.
                        result = UnsafeNativeMethods.UiaRaiseNotificationEvent(
                        this,
                        notificationKind,
                        notificationProcessing,
                        notificationText,
                        String.Empty);
                    }
                    catch (EntryPointNotFoundException)
                    {
                        // The UIA Notification event is not available, so don't attempt to raise it again.
                        notificationEventAvailable = false;
                    }

                    return result == NativeMethods.S_OK;
                }
            }

            private sealed class StringCollectionEditorTextBox : TextBox
            {
                private int currentRowIndex = 0;

                private IRawElementProviderSimple uiaProvider;

                public IRawElementProviderSimple UiaProvider
                {
                    get
                    {
                        if (uiaProvider == null)
                        {
                            uiaProvider = new TextBoxUiaProvider(this, this.CreateAccessibilityInstance());
                        }

                        return uiaProvider;
                    }
                }

                protected override void OnGotFocus(EventArgs e)
                {
                    base.OnGotFocus(e);

                    SelectAndAnnounceCurrentString();
                }

                protected override void OnKeyUp(KeyEventArgs e)
                {
                    base.OnKeyUp(e);

                    if ((e.KeyCode == Keys.Up ||
                        e.KeyCode == Keys.Down ||
                        e.KeyCode == Keys.PageUp ||
                        e.KeyCode == Keys.PageDown) &&
                        (e.KeyData & Keys.Shift) == 0 &&
                        (e.KeyData & Keys.Control) == 0)
                    {
                        // Announce the string and update the current row index.
                        SelectAndAnnounceCurrentString();
                    }
                    else if (e.KeyCode == Keys.Enter ||
                        e.KeyCode == Keys.Back)
                    {
                        // Update the current row index on enter/backspace
                        // (current line can be changed in such cases)
                        int selectionStart = GetFirstCharIndexOfCurrentLine();
                        currentRowIndex = GetLineFromCharIndex(selectionStart);
                    }
                }

                protected override void OnMouseUp(MouseEventArgs mevent)
                {
                    base.OnMouseUp(mevent);

                    SelectAndAnnounceCurrentString();
                }

                public void SelectAndAnnounceCurrentString()
                {
                    if (SelectedText.Length > 0)
                    {
                        // Do not announce lines when there is a selection.
                        return;
                    }

                    var textBoxUiaProvider = UiaProvider as TextBoxUiaProvider;
                    textBoxUiaProvider.RaiseAutomationEvent(NativeMethods.UIA_AutomationFocusChangedEventId);

                    int selectionStart = GetFirstCharIndexOfCurrentLine();

                    if (currentRowIndex == GetLineFromCharIndex(selectionStart))
                    {
                        return;
                    }

                    int selectionEnd = selectionStart;
                    int position = selectionStart;
                    while (position < Text.Length && Text[position] != '\r' && Text[position] != '\n')
                    {
                        selectionEnd = position++;
                    }

                    string stringEntryAnnouncement = (selectionEnd - selectionStart == 0)
                        ? SR.StringCollectionEditor_EmptyText
                        : Text.Substring(selectionStart, selectionEnd - selectionStart + 1);

                    // NOTE: What is better UI experience: to select the string
                    // on up/down navigating to another line or not? Disable selecting for now.
                    // Select(selectionStart, selectionEnd - selectionStart + 1);

                    textBoxUiaProvider.RaiseAutomationNotification(
                        NativeMethods.AutomationNotificationKind.Other,
                        NativeMethods.AutomationNotificationProcessing.All,
                        stringEntryAnnouncement);

                    currentRowIndex = GetLineFromCharIndex(selectionStart);
                }

                protected override AccessibleObject CreateAccessibilityInstance()
                {
                    return base.CreateAccessibilityInstance();
                }

                protected override void WndProc(ref Message m)
                {
                    if (m.Msg == Interop.WindowMessages.WM_GETOBJECT && m.LParam == (IntPtr)(NativeMethods.UiaRootObjectId))
                    {
                        m.Result = UnsafeNativeMethods.UiaReturnRawElementProvider(
                            new Runtime.InteropServices.HandleRef(this, Handle),
                            m.WParam,
                            m.LParam,
                            UiaProvider);

                        return;
                    }

                    if (m.Msg == Interop.WindowMessages.WM_DESTROY)
                    {
                        UnsafeNativeMethods.UiaReturnRawElementProvider(new Runtime.InteropServices.HandleRef(this, Handle), new IntPtr(0), new IntPtr(0), null);
                    }

                    base.WndProc(ref m);
                }
            }
        }
    }
}

