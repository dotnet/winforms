// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\TabControlCancelEvent.uex' path='docs/doc[@for="TabControlCancelEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///    Provides data for the <see cref='System.Windows.Forms.TabControl.OnDeselecting'/>,
    ///    <see cref='System.Windows.Forms.TabControl.OnDeselected'/>
    ///    event.
    ///
    ///    </para>
    /// </devdoc>
    public class TabControlCancelEventArgs : CancelEventArgs {
        private TabPage tabPage;
        private int tabPageIndex;
        private TabControlAction action;


        /// <include file='doc\TabControlCancelEvent.uex' path='docs/doc[@for="TabControlCancelEventArgs.TabControlCancelEventArgs"]/*' />
        public TabControlCancelEventArgs(TabPage tabPage, int tabPageIndex, bool cancel, TabControlAction action)
        : base(cancel) {
            this.tabPage = tabPage;                                           
            this.tabPageIndex = tabPageIndex;
            this.action = action;
        }

        /// <include file='doc\TabControlCancelEvent.uex' path='docs/doc[@for="TabControlCancelEventArgs.TabPage"]/*' />
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

        /// <include file='doc\TabControlCancelEvent.uex' path='docs/doc[@for="TabControlCancelEventArgs.TabPageIndex"]/*' />
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

        /// <include file='doc\TabControlCancelEvent.uex' path='docs/doc[@for="TabControlCancelEventArgs.Action"]/*' />
        /// <devdoc>
        ///    <para>
        ///        Stores the TabControl action which instigated the TabControl event.
        ///       
        ///    </para>
        /// </devdoc>
        public TabControlAction Action {
            get {
                return action;
            }
        }
    }
}
