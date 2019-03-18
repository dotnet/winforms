// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Design
{
    /// <devdoc>
    /// Provides support for interaction with the user interface of the development
    /// environment object that is hosting the designer
    /// </devdoc>
    [Guid("06A9C74B-5E32-4561-BE73-381B37869F4F")]
    public interface IUIService
    {
        /// <devdoc>
        /// Gets or sets the collections of styles that are specific to the host's environment
        /// </devdoc>
        IDictionary Styles { get; }

        /// <devdoc>
        /// Indicates whether the component can display a ComponentDesigner
        /// </devdoc>
        bool CanShowComponentEditor(object component);

        /// <devdoc>
        /// Gets the window that should be used for dialog parenting
        /// </devdoc>
        IWin32Window GetDialogOwnerWindow();

        /// <devdoc>
        /// Sets a flag indicating the UI is dirty
        /// </devdoc>
        void SetUIDirty();

        /// <devdoc>
        /// Attempts to display a ComponentEditor for a component
        /// </devdoc>
        bool ShowComponentEditor(object component, IWin32Window parent);

        /// <devdoc>
        /// Attempts to display the specified form in a dialog box
        /// </devdoc>
        DialogResult ShowDialog(Form form);

        /// <devdoc>
        /// Displays the specified error message in a message box
        /// </devdoc>
        void ShowError(string message);

        /// <devdoc>
        /// Displays the specified exception and its information in a message box
        /// </devdoc>
        void ShowError(Exception ex);

        /// <devdoc>
        /// Displays the specified exception and its information in a message box
        /// </devdoc>
        void ShowError(Exception ex, string message);

        /// <devdoc>
        /// Displays the specified message in a message box
        /// </devdoc>
        void ShowMessage(string message);

        /// <devdoc>
        /// Displays the specified message in a message box with the specified caption
        /// </devdoc>
        void ShowMessage(string message, string caption);

        /// <devdoc>
        /// Displays the specified message in a message box with the specified caption and
        /// buttons to place on the dialog box
        /// </devdoc>
        DialogResult ShowMessage(string message, string caption, MessageBoxButtons buttons);

        /// <devdoc>
        /// Displays the specified tool window
        /// </devdoc>
        bool ShowToolWindow(Guid toolWindow);
    }
}
