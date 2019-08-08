// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides ambient property values to top-level controls.
    ///  NOTE: internally, this class does double duty as storage for
    ///  Control's inherited properties.
    /// </summary>
    public sealed class AmbientProperties
    {
        /// <summary>
        ///  Gets the ambient BackColor, or Color.Empty if there is none.
        /// </summary>
        public Color BackColor { get; set; }

        /// <summary>
        ///  Gets the ambient BackColor, or null if there is none.
        /// </summary>
        public Cursor Cursor { get; set; }

        /// <summary>
        ///  Gets the ambient Font, or null if there is none.
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        ///  Gets the ambient ForeColor, or Color.Empty if there is none.
        /// </summary>
        public Color ForeColor { get; set; }
    }
}
