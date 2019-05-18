﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// The site for a ComponentEditorPage.
    /// </devdoc>
    public interface IComponentEditorPageSite
    {
        /// <summary>
        /// Returns the parent control for the page window.
        /// </devdoc>
        Control GetControl();

        /// <summary>
        /// Notifies the site that the editor is in dirty state.
        /// </devdoc>
        void SetDirty();
    }
}
