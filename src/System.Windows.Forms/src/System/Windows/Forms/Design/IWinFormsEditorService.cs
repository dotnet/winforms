// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    /// <devdoc>
    /// Provides an interface to display Win Forms dialog boxes and controls.
    /// </devdoc>
    public interface IWindowsFormsEditorService
    {
        /// <devdoc>
        /// Closes a previously opened drop down list
        /// </devdoc>
        void CloseDropDown();

        /// <devdoc>
        /// Displays the specified control in a drop down list
        /// </devdoc>
        void DropDownControl(Control control);

        /// <devdoc>
        /// Shows the specified dialog box.
        /// </devdoc>
        DialogResult ShowDialog(Form dialog);
    }
}
