// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the <see cref='System.Windows.Forms.TabControl.Selecting'/>,<see cref='System.Windows.Forms.TabControl.Selected'/>
    /// event.
    /// </devdoc>
    public class TabControlEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.TabControlEventArgs'/>class.
        /// </devdoc>
        public TabControlEventArgs(TabPage tabPage, int tabPageIndex, TabControlAction action)
        {
            TabPage = tabPage;
            TabPageIndex = tabPageIndex;
            Action = action;
        }

        /// <summary>
        /// Stores the referemce to the tabpage that is undergoing the TabControl event.
        /// </devdoc>
        public TabPage TabPage { get; }

        /// <summary>
        /// Stores the index to the tabpage that is undergoing the TabControl event.
        /// </devdoc>
        public int TabPageIndex { get; }

        /// <summary>
        /// Stores the TabControl action which instigated the TabControl event.
        ///    </para>
        /// </devdoc>
        public TabControlAction Action { get; }
    }
}
