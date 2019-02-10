// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms.Design {
    
    using System;    
    using Microsoft.Win32;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    /// <devdoc>
    ///    <para> 
    ///       Provides support
    ///       for interaction with the user
    ///       interface of the development environment object that is hosting the designer.</para>
    /// </devdoc>
    [Guid("06A9C74B-5E32-4561-BE73-381B37869F4F")]
    public interface IUIService {
        /// <devdoc>
        ///    <para> Gets or sets the collections of
        ///       styles that are specific to the host's environment.</para>
        /// </devdoc>
        IDictionary Styles {get;}
        
        /// <devdoc>
        ///    <para> Indicates whether the component can 
        ///       display a ComponentDesigner.</para>
        /// </devdoc>
        bool CanShowComponentEditor(object component);

        /// <devdoc>
        ///    <para>Gets the window that should be used for dialog parenting.</para>
        /// </devdoc>
        IWin32Window GetDialogOwnerWindow();
        

        /// <devdoc>
        ///    <para>Sets a flag indicating the UI is dirty.</para>
        /// </devdoc>
        void SetUIDirty();
                
        /// <devdoc>
        /// <para>Attempts to display a ComponentEditor for a component.</para>
        /// </devdoc>
        bool ShowComponentEditor(object component, IWin32Window parent);
        /// <devdoc>
        ///    <para>Attempts to display the specified form in a dialog box.</para>
        /// </devdoc>
        
        DialogResult ShowDialog(Form form);

        /// <devdoc>
        ///    <para>Displays the specified error message in a message box.</para>
        /// </devdoc>
        void ShowError(string message);

        /// <devdoc>
        ///    <para> Displays the specified exception
        ///       and its information in a message box.</para>
        /// </devdoc>
        void ShowError(Exception ex);

        /// <devdoc>
        ///    <para> Displays the specified exception
        ///       and its information in a message box.</para>
        /// </devdoc>
        void ShowError(Exception ex, string message);
        
        /// <devdoc>
        ///    <para>Displays the specified message in a message box.</para>
        /// </devdoc>
        void ShowMessage(string message);

        /// <devdoc>
        ///    <para> Displays the specified message in
        ///       a message box with the specified caption.</para>
        /// </devdoc>
        void ShowMessage(string message, string caption);

        /// <devdoc>
        ///    <para> Displays the specified message in a message box with the specified caption and
        ///       buttons to place on the dialog box.</para>
        /// </devdoc>
        DialogResult ShowMessage(string message, string caption, MessageBoxButtons buttons);

        /// <devdoc>
        ///    <para>Displays the specified tool window.</para>
        /// </devdoc>
        bool ShowToolWindow(Guid toolWindow);
    }
}
