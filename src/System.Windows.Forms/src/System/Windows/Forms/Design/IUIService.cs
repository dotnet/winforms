// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides support for interaction with the user interface of the development
    ///  environment object that is hosting the designer
    /// </summary>
    [Guid("06A9C74B-5E32-4561-BE73-381B37869F4F")]
    public interface IUIService
    {
        /// <summary>
        ///  Gets or sets the collections of styles that are specific to the host's environment
        /// </summary>
        IDictionary Styles { get; }

        /// <summary>
        ///  Indicates whether the component can display a ComponentDesigner
        /// </summary>
        bool CanShowComponentEditor(object component);

        /// <summary>
        ///  Gets the window that should be used for dialog parenting
        /// </summary>
        IWin32Window GetDialogOwnerWindow();

        /// <summary>
        ///  Sets a flag indicating the UI is dirty
        /// </summary>
        void SetUIDirty();

        /// <summary>
        ///  Attempts to display a ComponentEditor for a component
        /// </summary>
        bool ShowComponentEditor(object component, IWin32Window parent);

        /// <summary>
        ///  Attempts to display the specified form in a dialog box
        /// </summary>
        DialogResult ShowDialog(Form form);

        /// <summary>
        ///  Displays the specified error message in a message box
        /// </summary>
        void ShowError(string message);

        /// <summary>
        ///  Displays the specified exception and its information in a message box
        /// </summary>
        void ShowError(Exception ex);

        /// <summary>
        ///  Displays the specified exception and its information in a message box
        /// </summary>
        void ShowError(Exception ex, string message);

        /// <summary>
        ///  Displays the specified message in a message box
        /// </summary>
        void ShowMessage(string message);

        /// <summary>
        ///  Displays the specified message in a message box with the specified caption
        /// </summary>
        void ShowMessage(string message, string caption);

        /// <summary>
        ///  Displays the specified message in a message box with the specified caption and
        ///  buttons to place on the dialog box
        /// </summary>
        DialogResult ShowMessage(string message, string caption, MessageBoxButtons buttons);

        /// <summary>
        ///  Displays the specified tool window
        /// </summary>
        bool ShowToolWindow(Guid toolWindow);
    }
}
