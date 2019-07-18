// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides access to the menu editing service.
    /// </summary>
    public interface IMenuEditorService
    {
        /// <summary>
        ///  Gets the current menu.
        /// </summary>
        Menu GetMenu();

        /// <summary>
        ///  Gets a value indicating whether the current menu is active.
        /// </summary>
        bool IsActive();

        /// <summary>
            ///  Sets the current menu visible
        ///  on the form.
            /// </summary>
        void SetMenu(Menu menu);

        /// <summary>
        ///  Sets the selected menu item of the current menu.
        /// </summary>
        void SetSelection(MenuItem item);

        /// <summary>
        ///  Allows the editor service to intercept Win32 messages.
        /// </summary>
        bool MessageFilter(ref Message m);
    }
}
