// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.TabControl.Selecting'/>,<see cref='System.Windows.Forms.TabControl.Selected'/>
    /// event.
    /// </devdoc>
    public class TabControlEventArgs : EventArgs
    {
        /// <devdoc>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.TabControlEventArgs'/>class.
        /// </devdoc>
        public TabControlEventArgs(TabPage tabPage, int tabPageIndex, TabControlAction action)
        {
            TabPage = tabPage;
            TabPageIndex = tabPageIndex;
            Action = action;
        }

        /// <devdoc>
        /// Stores the referemce to the tabpage that is undergoing the TabControl event.
        /// </devdoc>
        public TabPage TabPage { get; }

        /// <devdoc>
        /// Stores the index to the tabpage that is undergoing the TabControl event.
        /// </devdoc>
        public int TabPageIndex { get; }

        /// <devdoc>
        /// Stores the TabControl action which instigated the TabControl event.
        ///    </para>
        /// </devdoc>
        public TabControlAction Action { get; }
    }
}
