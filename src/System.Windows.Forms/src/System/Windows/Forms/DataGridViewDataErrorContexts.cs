// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;

    /// <include file='doc\DataGridViewDataErrorContexts.uex' path='docs/doc[@for="DataGridViewDataErrorContexts.DataGridViewDataErrorContexts"]/*' />
    /// <devdoc>
    /// <para></para>
    /// </devdoc>
    [Flags]
    public enum DataGridViewDataErrorContexts
    {
        /// <include file='doc\DataGridViewDataErrorContexts.uex' path='docs/doc[@for="DataGridViewDataErrorContexts.Formatting"]/*' />
        Formatting = 0x0001,

        /// <include file='doc\DataGridViewDataErrorContexts.uex' path='docs/doc[@for="DataGridViewDataErrorContexts.Display"]/*' />
        Display = 0x0002,

        /// <include file='doc\DataGridViewDataErrorContexts.uex' path='docs/doc[@for="DataGridViewDataErrorContexts.PreferredSize"]/*' />
        PreferredSize = 0x0004,

        /// <include file='doc\DataGridViewDataErrorContexts.uex' path='docs/doc[@for="DataGridViewDataErrorContexts.RowDeletion"]/*' />
        RowDeletion = 0x0008,

        /// <include file='doc\DataGridViewDataErrorContexts.uex' path='docs/doc[@for="DataGridViewDataErrorContexts.Parsing"]/*' />
        Parsing = 0x0100,

        /// <include file='doc\DataGridViewDataErrorContexts.uex' path='docs/doc[@for="DataGridViewDataErrorContexts.Commit"]/*' />
        Commit = 0x0200,

        /// <include file='doc\DataGridViewDataErrorContexts.uex' path='docs/doc[@for="DataGridViewDataErrorContexts.InitialValueRestoration"]/*' />
        InitialValueRestoration = 0x0400,

        /// <include file='doc\DataGridViewDataErrorContexts.uex' path='docs/doc[@for="DataGridViewDataErrorContexts.LeaveControl"]/*' />
        LeaveControl = 0x0800,

        /// <include file='doc\DataGridViewDataErrorContexts.uex' path='docs/doc[@for="DataGridViewDataErrorContexts.CurrentCellChange"]/*' />
        CurrentCellChange = 0x1000,

        /// <include file='doc\DataGridViewDataErrorContexts.uex' path='docs/doc[@for="DataGridViewDataErrorContexts.Scroll"]/*' />
        Scroll = 0x2000,

        /// <include file='doc\DataGridViewDataErrorContexts.uex' path='docs/doc[@for="DataGridViewDataErrorContexts.ClipboardContent"]/*' />
        ClipboardContent = 0x4000
    }
}
