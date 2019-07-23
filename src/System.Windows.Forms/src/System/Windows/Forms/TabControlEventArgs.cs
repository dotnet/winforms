// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the <see cref='TabControl.Selecting'/>,<see cref='TabControl.Selected'/>
    /// event.
    /// </summary>
    public class TabControlEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref='TabControlEventArgs'/>class.
        /// </summary>
        public TabControlEventArgs(TabPage tabPage, int tabPageIndex, TabControlAction action)
        {
            TabPage = tabPage;
            TabPageIndex = tabPageIndex;
            Action = action;
        }

        /// <summary>
        /// Stores the referemce to the tabpage that is undergoing the TabControl event.
        /// </summary>
        public TabPage TabPage { get; }

        /// <summary>
        /// Stores the index to the tabpage that is undergoing the TabControl event.
        /// </summary>
        public int TabPageIndex { get; }

        /// <summary>
        /// Stores the TabControl action which instigated the TabControl event.
        /// </summary>
        public TabControlAction Action { get; }
    }
}
