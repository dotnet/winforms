// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms{
    using System.Runtime.Remoting;
    using System;
    using System.Security.Permissions;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;
    using System.Runtime.InteropServices;

    /// <include file='doc\DataGridTextBox.uex' path='docs/doc[@for="DataGridTextBox"]/*' />
    /// <devdoc>
    /// <para>Represents a <see cref='System.Windows.Forms.TextBox'/> control that is hosted in a 
    /// <see cref='System.Windows.Forms.DataGridTextBoxColumn'/> .</para>
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    ToolboxItem(false),
    DesignTimeVisible(false)
    ]
    public class DataGridTextBox : TextBox {

        private bool isInEditOrNavigateMode = true;

        // only needed to signal the dataGrid that an edit
        // takes place
        private DataGrid dataGrid;

        /// <include file='doc\DataGridTextBox.uex' path='docs/doc[@for="DataGridTextBox.DataGridTextBox"]/*' />
        public DataGridTextBox() : base () { 
            TabStop = false;
        }
        /// <include file='doc\DataGridTextBox.uex' path='docs/doc[@for="DataGridTextBox.SetDataGrid"]/*' />
        /// <devdoc>
        /// <para>Sets the <see cref='System.Windows.Forms.DataGrid'/> to which this <see cref='System.Windows.Forms.TextBox'/> control belongs.</para>
        /// </devdoc>
        public void SetDataGrid(DataGrid parentGrid)
        {
            dataGrid = parentGrid;
        }

        /// <include file='doc\DataGridTextBox.uex' path='docs/doc[@for="DataGridTextBox.WndProc"]/*' />
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m) {
            // but what if we get a CtrlV?
            // what about deleting from the menu?
            if (m.Msg == NativeMethods.WM_PASTE || m.Msg == NativeMethods.WM_CUT || m.Msg == NativeMethods.WM_CLEAR) {
                IsInEditOrNavigateMode = false;
                dataGrid.ColumnStartedEditing(Bounds);
            }

            base.WndProc(ref m);

        }

        /// <include file='doc\DataGridTextBox.uex' path='docs/doc[@for="DataGridTextBox.OnMouseWheel"]/*' />
        protected override void OnMouseWheel(MouseEventArgs e) {
            dataGrid.TextBoxOnMouseWheel(e);
        }

        /// <include file='doc\DataGridTextBox.uex' path='docs/doc[@for="DataGridTextBox.OnKeyPress"]/*' />
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            // Shift-Space should not cause the grid to 
            // be put in edit mode
            if (e.KeyChar == ' ' && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                return;

            // if the edit box is in ReadOnly mode, then do not tell the DataGrid about the 
            // edit
            if (this.ReadOnly)
                return;

            // Ctrl-* should not put the grid in edit mode
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control && ((Control.ModifierKeys & Keys.Alt) == 0))
                return;
            IsInEditOrNavigateMode = false;

            // let the DataGrid know about the edit
            dataGrid.ColumnStartedEditing(Bounds);
        }

        /// <include file='doc\DataGridTextBox.uex' path='docs/doc[@for="DataGridTextBox.ProcessKeyMessage"]/*' />
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected internal override bool ProcessKeyMessage(ref Message m)
        {
            Keys key = (Keys)unchecked((int)(long)m.WParam);
            Keys modifierKeys = ModifierKeys;

            if ((key | modifierKeys) == Keys.Enter || (key | modifierKeys) == Keys.Escape || ((key | modifierKeys) == (Keys.Enter | Keys.Control))
            )
            {
                // enter and escape keys are sent directly to the DataGrid
                // for those keys, eat the WM_CHAR part of the KeyMessage
                //
                if (m.Msg == NativeMethods.WM_CHAR)
                    return true;
                return ProcessKeyPreview(ref m);
            }

            if (m.Msg == NativeMethods.WM_CHAR) {
                if (key == Keys.LineFeed)           // eat the LineFeed we get when the user presses Ctrl-Enter in a gridTextBox
                    return true;
                return ProcessKeyEventArgs(ref m);
            }

            // now the edit control will be always on top of the grid
            // we only want to process the WM_KEYUP message ( the same way the grid was doing when the grid was getting all
            // the keys )
            if (m.Msg == NativeMethods.WM_KEYUP)
                return true;

            Keys keyData = key & Keys.KeyCode;

            switch (keyData)
            {
                case Keys.Right:
                    // here is the deal with Keys.Right:
                    // if the end of the selection is at the end of the string
                    // send this character to the dataGrid
                    // else, process the KeyEvent
                    //
                    if (SelectionStart + SelectionLength == Text.Length)
                        return ProcessKeyPreview(ref m);
                    return ProcessKeyEventArgs(ref m);
                case Keys.Left:
                    // if the end of the selection is at the begining of the string
                    // or if the entire text is selected and we did not start editing
                    // send this character to the dataGrid
                    // else, process the KeyEvent
                    // 
                    if (SelectionStart + SelectionLength == 0 ||
                        (this.IsInEditOrNavigateMode && this.SelectionLength == Text.Length))
                        return ProcessKeyPreview(ref m);
                    return ProcessKeyEventArgs(ref m);
                case Keys.Down:
                    // if the end of the selection is on the last line of the text then 
                    // send this character to the dataGrid
                    // else, process the KeyEvent
                    //
                    int end = SelectionStart + SelectionLength;
                    if (Text.IndexOf("\r\n", end) == -1)
                        return ProcessKeyPreview(ref m);
                    return ProcessKeyEventArgs(ref m);
                case Keys.Up:
                    // if the end of the selection is on the first line of the text then 
                    // send this character to the dataGrid
                    // else, process the KeyEvent
                    //
                    if ( Text.IndexOf("\r\n") < 0 || SelectionStart + SelectionLength  < Text.IndexOf("\r\n"))
                        return ProcessKeyPreview(ref m);
                    return ProcessKeyEventArgs(ref m);
                case Keys.Home:
                case Keys.End:
                    if (SelectionLength == Text.Length)
                        return ProcessKeyPreview(ref m);
                    else
                        return ProcessKeyEventArgs(ref m);
                case Keys.Prior:
                case Keys.Next:
                case Keys.Oemplus:
                case Keys.Add:
                case Keys.OemMinus:
                case Keys.Subtract:
                    if (IsInEditOrNavigateMode)
                    {
                        // this will ultimately call parent's ProcessKeyPreview
                        // in our case, DataGrid's ProcessKeyPreview
                        return ProcessKeyPreview(ref m);
                    }
                    else
                    {
                        return ProcessKeyEventArgs(ref m);
                    }
                case Keys.Space:
                    if (IsInEditOrNavigateMode && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                    {
                            // when we get a SHIFT-SPACEBAR message, disregard the WM_CHAR part of the message
                            if (m.Msg == NativeMethods.WM_CHAR) return true;

                            // if the user pressed the SHIFT key at the same time with 
                            // the space key, send the key message to the DataGrid
                            return ProcessKeyPreview(ref m);
                    }
                    return ProcessKeyEventArgs(ref m);
                case Keys.A:
                    if (IsInEditOrNavigateMode && (Control.ModifierKeys & Keys.Control) == Keys.Control)
                    {
                            // when we get a Control-A message, disregard the WM_CHAR part of the message
                            if (m.Msg == NativeMethods.WM_CHAR) return true;

                            // if the user pressed the Control key at the same time with 
                            // the space key, send the key message to the DataGrid
                            return ProcessKeyPreview(ref m);
                    }
                    return ProcessKeyEventArgs(ref m);
                case Keys.F2:
                    IsInEditOrNavigateMode = false;
                    // do not select all the text, but
                    // position the caret at the end of the text
                    SelectionStart = Text.Length;
                    return true;
                case Keys.Delete:
                    if (IsInEditOrNavigateMode) {
                        // pass the delete to the parent, in our case, the DataGrid
                        // if the dataGrid used the key, then we aren't gonne
                        // use it anymore, else we are
                        if (ProcessKeyPreview(ref m))
                            return true;
                        else {
                            // the edit control will use the 
                            // delete key: we are in Edit mode now:
                            IsInEditOrNavigateMode = false;
                            dataGrid.ColumnStartedEditing(Bounds);

                            return ProcessKeyEventArgs(ref m);
                        }
                    }
                    else
                        return ProcessKeyEventArgs(ref m);
                case Keys.Tab:
                    // the TextBox gets the Control-Tab messages,
                    // not the parent
                    if ((ModifierKeys & Keys.Control) == Keys.Control)
                        return ProcessKeyPreview(ref m);
                    else
                        return ProcessKeyEventArgs(ref m);
                default:
                    return ProcessKeyEventArgs(ref m);
            }
        }

        /// <include file='doc\DataGridTextBox.uex' path='docs/doc[@for="DataGridTextBox.IsInEditOrNavigateMode"]/*' />
        public bool IsInEditOrNavigateMode {
            get {
                return isInEditOrNavigateMode;
            }
            set {
                isInEditOrNavigateMode = value;
                if (value)
                    SelectAll();
            }
        }
    }
}
