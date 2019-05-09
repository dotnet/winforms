// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides ambient property values to top-level controls.
    /// NOTE: internally, this class does double duty as storage for
    /// Control's inherited properties.
    /// </devdoc>
    public sealed class AmbientProperties
    {
        /// <devdoc>
        /// Gets the ambient BackColor, or Color.Empty if there is none.
        /// </devdoc>
        public Color BackColor { get; set; }

        /// <devdoc>
        /// Gets the ambient BackColor, or null if there is none.
        /// </devdoc>
        public Cursor Cursor { get; set; }

        /// <devdoc>
        /// Gets the ambient Font, or null if there is none.
        /// </devdoc>
        public Font Font { get; set; }

        /// <devdoc>
        /// Gets the ambient ForeColor, or Color.Empty if there is none.
        /// </devdoc>
        public Color ForeColor { get; set; }
    }
}
