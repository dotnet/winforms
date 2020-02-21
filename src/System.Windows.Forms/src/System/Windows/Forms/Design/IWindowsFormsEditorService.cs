// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides an interface to display Win Forms dialog boxes and controls.
    /// </summary>
    public interface IWindowsFormsEditorService
    {
        /// <summary>
        ///  Closes a previously opened drop down list
        /// </summary>
        void CloseDropDown();

        /// <summary>
        ///  Displays the specified control in a drop down list
        /// </summary>
        void DropDownControl(Control control);

        /// <summary>
        ///  Shows the specified dialog box.
        /// </summary>
        DialogResult ShowDialog(Form dialog);
    }
}
