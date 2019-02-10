// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms.Design {

    using System.Diagnostics;

    using System.Windows.Forms;


    /// <devdoc>
    ///    <para>
    ///       Provides an interface to display Win Forms dialog
    ///       boxes and controls.
    ///    </para>
    /// </devdoc>
    public interface IWindowsFormsEditorService {


        /// <devdoc>
        /// <para>Closes a previously opened drop down
        /// list.</para>
        /// </devdoc>
        void CloseDropDown();
    

        /// <devdoc>
        ///    <para>Displays the specified control in a drop down list.</para>
        /// </devdoc>
        void DropDownControl(Control control);
    

        /// <devdoc>
        ///    <para>
        ///       Shows the specified dialog box.
        ///    </para>
        /// </devdoc>
        DialogResult ShowDialog(Form dialog);
    }
}

