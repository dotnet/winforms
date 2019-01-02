// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    //=--------------------------------------------------------------------------=
    // GiveFeedbackEventArgs.cs
    //=--------------------------------------------------------------------------=

    using System;
    using System.Drawing;
    using System.ComponentModel;
    using Microsoft.Win32;


    /// <include file='doc\GiveFeedbackEvent.uex' path='docs/doc[@for="GiveFeedbackEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.Control.GiveFeedback'/>
    ///       event.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class GiveFeedbackEventArgs : EventArgs {
        private readonly DragDropEffects effect;
        private bool useDefaultCursors;

        /// <include file='doc\GiveFeedbackEvent.uex' path='docs/doc[@for="GiveFeedbackEventArgs.GiveFeedbackEventArgs"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.GiveFeedbackEventArgs'/> class.
        ///    </para>
        /// </devdoc>
        public GiveFeedbackEventArgs(DragDropEffects effect, bool useDefaultCursors) {
            this.effect = effect;
            this.useDefaultCursors = useDefaultCursors;
        }

        /// <include file='doc\GiveFeedbackEvent.uex' path='docs/doc[@for="GiveFeedbackEventArgs.Effect"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the type of drag-and-drop operation.
        ///    </para>
        /// </devdoc>
        public DragDropEffects Effect {
            get {
                return effect;
            }
        }

        /// <include file='doc\GiveFeedbackEvent.uex' path='docs/doc[@for="GiveFeedbackEventArgs.UseDefaultCursors"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a
        ///       value
        ///       indicating whether a default pointer is used.
        ///    </para>
        /// </devdoc>
        public bool UseDefaultCursors {
            get {
                return useDefaultCursors;
            }
            set {
                useDefaultCursors = value;
            }
        }
    }
}
