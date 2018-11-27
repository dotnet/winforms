// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///     <para>Provides access to the menu editing service.</para>
    /// </summary>
    public interface IMenuEditorService
    {
        /// <summary>
        ///     Gets the current menu.
        /// </summary>
        Menu GetMenu();

        /// <summary>
        ///     <para>Gets a value indicating whether the current menu is active.</para>
        /// </summary>
        bool IsActive();

        /// <summary>
        ///     <para>
        ///         Sets the current menu visible
        ///         on the form.
        ///     </para>
        /// </summary>
        void SetMenu(Menu menu);

        /// <summary>
        ///     <para>Sets the selected menu item of the current menu.</para>
        /// </summary>
        void SetSelection(MenuItem item);

        /// <summary>
        ///     Allows the editor service to intercept Win32 messages.
        /// </summary>
        bool MessageFilter(ref Message m);
    }
}
