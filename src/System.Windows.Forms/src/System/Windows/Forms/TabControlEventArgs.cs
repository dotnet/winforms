// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;

    /// <include file='doc\TabControlEvent.uex' path='docs/doc[@for="TabControlEventArgs"]/*' />
    /// <devdoc>
    /// <para>Provides data for the <see cref='System.Windows.Forms.TabControl.Selecting'/>,<see cref='System.Windows.Forms.TabControl.Selected'/>
    /// event.</para>
    /// </devdoc>
    public class TabControlEventArgs : EventArgs {

        private TabPage tabPage;
        private int tabPageIndex;
        private TabControlAction action;

        /// <include file='doc\TabControlEvent.uex' path='docs/doc[@for="TabControlEventArgs.TabControlEventArgs"]/*' />
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

        /// <include file='doc\TabControlEvent.uex' path='docs/doc[@for="TabControlEventArgs.TabPage"]/*' />
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

        /// <include file='doc\TabControlEvent.uex' path='docs/doc[@for="TabControlEventArgs.TabPageIndex"]/*' />
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

        /// <include file='doc\TabControlEvent.uex' path='docs/doc[@for="TabControlEvent.Action"]/*' />
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

