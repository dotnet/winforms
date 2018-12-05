// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows.Forms.Design;
    using System.ComponentModel.Design;
    using System.Drawing;
    using Microsoft.Win32;

    /// <devdoc>
    ///    <para>
    ///       Provides ambient property values to top-level controls.
    ///    </para>
    /// </devdoc>
    //
    // NOTE: internally, this class does double duty as storage for Control's inherited properties.
    public sealed class AmbientProperties {
    
        // Public ambient properties
        private Color    backColor; 
        private Color    foreColor;
        private Cursor   cursor;
        private Font     font;
        
        /// <devdoc>
        ///    <para>
        ///       Gets the ambient BackColor, or Color.Empty if there is none.
        ///    </para>
        /// </devdoc>
        public Color BackColor {
            get {
                return backColor;
            }
            set {
                backColor = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets the ambient BackColor, or null if there is none.
        ///    </para>
        /// </devdoc>
        public Cursor Cursor {
            get {
                return cursor;
            }
            set {
                cursor = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets the ambient Font, or null if there is none.
        ///    </para>
        /// </devdoc>
        public Font Font {
            get {
                return font;
            }
            set {
                font = value;
            }
        }
        
        /// <devdoc>
        ///    <para>
        ///       Gets the ambient ForeColor, or Color.Empty if there is none.
        ///    </para>
        /// </devdoc>
        public Color ForeColor {
            get {
                return foreColor;
            }
            set {
                foreColor = value;
            }
        }
    }
}

