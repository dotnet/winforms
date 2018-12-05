// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;

    /// <devdoc>
    /// <para></para>
    /// </devdoc>
    public enum DataGridViewContentAlignment
    {
        NotSet = 0x000,

        /// <devdoc>
        ///    Content is vertically aligned at the top, and horizontally
        ///    aligned on the left.
        /// </devdoc>
        TopLeft = 0x001,
        
        /// <devdoc>
        ///    <para>
        ///       Content is vertically aligned at the top, and
        ///       horizontally aligned at the center.
        ///    </para>
        /// </devdoc>
        TopCenter = 0x002,
        
        /// <devdoc>
        ///    <para>
        ///       Content is vertically aligned at the top, and
        ///       horizontally aligned on the right.
        ///    </para>
        /// </devdoc>
        TopRight = 0x004,
        
        /// <devdoc>
        ///    <para>
        ///       Content is vertically aligned in the middle, and
        ///       horizontally aligned on the left.
        ///    </para>
        /// </devdoc>
        MiddleLeft = 0x010,
        
        /// <devdoc>
        ///    <para>
        ///       Content is vertically aligned in the middle, and
        ///       horizontally aligned at the center.
        ///    </para>
        /// </devdoc>
        MiddleCenter = 0x020,
        
        /// <devdoc>
        ///    <para>
        ///       Content is vertically aligned in the middle, and horizontally aligned on the
        ///       right.
        ///    </para>
        /// </devdoc>
        MiddleRight = 0x040,
        
        /// <devdoc>
        ///    <para>
        ///       Content is vertically aligned at the bottom, and horizontally aligned on the
        ///       left.
        ///    </para>
        /// </devdoc>
        BottomLeft = 0x100,
        
        /// <devdoc>
        ///    <para>
        ///       Content is vertically aligned at the bottom, and horizontally aligned at the
        ///       center.
        ///    </para>
        /// </devdoc>
        BottomCenter = 0x200,

        /// <devdoc>
        ///    <para>
        ///       Content is vertically aligned at the bottom, and horizontally aligned on the
        ///       right.
        ///    </para>
        /// </devdoc>
        BottomRight = 0x400,
    }
}
