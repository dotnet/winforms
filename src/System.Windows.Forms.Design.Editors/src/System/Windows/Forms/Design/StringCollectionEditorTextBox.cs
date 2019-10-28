// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static System.Windows.Forms.Design.UnsafeNativeMethods;

namespace System.Windows.Forms.Design
{
    internal class StringCollectionEditorTextBox : TextBox
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
