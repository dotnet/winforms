// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;

    /// <devdoc>
    /// <para>Provides data for the <see cref='System.Windows.Forms.TabControl.Selecting'/>,<see cref='System.Windows.Forms.TabControl.Selected'/>
    /// event.</para>
    /// </devdoc>
    public class TabControlEventArgs : EventArgs {

        private TabPage tabPage;
        private int tabPageIndex;
        private TabControlAction action;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.TabControlEventArgs'/>class.
        ///       
        ///    </para>
        /// </devdoc>
        public TabControlEventArgs(TabPage tabPage, int tabPageIndex, TabControlAction action) {
            this.tabPage = tabPage;
            this.tabPageIndex = tabPageIndex;
            this.action = action;
        }

        /// <devdoc>
        ///    <para>
        ///       Stores the referemce to the tabpage that is undergoing the TabControl event.
        ///       
        ///    </para>
        /// </devdoc>
        public TabPage TabPage {
            get {
                return tabPage;
            }
        }

        /// <devdoc>
        ///    <para>
        ///        Stores the index to the tabpage that is undergoing the TabControl event.
        ///       
        ///    </para>
        /// </devdoc>
        public int TabPageIndex {
            get {
                return tabPageIndex;
            }
        }

        /// <devdoc>
        ///    <para>
        ///        Stores the TabControl action which instigated the TabControl event.
        ///       
        ///    </para>
        /// </devdoc>
        public TabControlAction Action{
            get {
                return action;
            }
        }
    }
}

