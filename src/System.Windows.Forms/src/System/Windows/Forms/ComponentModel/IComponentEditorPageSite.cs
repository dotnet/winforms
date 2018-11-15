// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



namespace System.Windows.Forms {
    using System.ComponentModel;

    using System.Diagnostics;

    using System;
    using System.Windows.Forms;
    using Microsoft.Win32;

    /// <include file='doc\IComponentEditorPageSite.uex' path='docs/doc[@for="IComponentEditorPageSite"]/*' />
    /// <internalonly/>
    /// <devdoc>
    ///    <para>The site for a ComponentEditorPage.</para>
    /// </devdoc>
    public interface IComponentEditorPageSite {

        /// <include file='doc\IComponentEditorPageSite.uex' path='docs/doc[@for="IComponentEditorPageSite.GetControl"]/*' />
        /// <devdoc>
        ///     Returns the parent control for the page window.
        /// </devdoc>
        Control GetControl();

        /// <include file='doc\IComponentEditorPageSite.uex' path='docs/doc[@for="IComponentEditorPageSite.SetDirty"]/*' />
        /// <devdoc>
        ///     Notifies the site that the editor is in dirty state.
        /// </devdoc>
        void SetDirty();
    }
}
