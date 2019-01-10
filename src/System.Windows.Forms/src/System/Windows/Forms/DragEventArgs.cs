// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.ComponentModel;
    using Microsoft.Win32;

    /// <include file='doc\DragEvent.uex' path='docs/doc[@for="DragEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.Control.DragDrop'/>, <see cref='System.Windows.Forms.Control.DragEnter'/>, or <see cref='System.Windows.Forms.Control.DragOver'/> event.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class DragEventArgs : EventArgs {
        /// <include file='doc\DragEvent.uex' path='docs/doc[@for="DragEventArgs.data"]/*' />
        /// <devdoc>
        ///     The data associated with this event.
        /// </devdoc>
        private readonly IDataObject data;
        /// <include file='doc\DragEvent.uex' path='docs/doc[@for="DragEventArgs.keyState"]/*' />
        /// <devdoc>
        ///     The current state of the shift, ctrl, and alt keys.
        /// </devdoc>
        private readonly int keyState;
        /// <include file='doc\DragEvent.uex' path='docs/doc[@for="DragEventArgs.x"]/*' />
        /// <devdoc>
        ///     The mouse x location.
        /// </devdoc>
        private readonly int x;
        /// <include file='doc\DragEvent.uex' path='docs/doc[@for="DragEventArgs.y"]/*' />
        /// <devdoc>
        ///     The mouse y location.
        /// </devdoc>
        private readonly int y;
        /// <include file='doc\DragEvent.uex' path='docs/doc[@for="DragEventArgs.allowedEffect"]/*' />
        /// <devdoc>
        ///     The effect that should be applied to the mouse cursor.
        /// </devdoc>
        private readonly DragDropEffects allowedEffect;
        /// <include file='doc\DragEvent.uex' path='docs/doc[@for="DragEventArgs.effect"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.DragEventArgs'/>
        ///       class.
        ///
        ///    </para>
        /// </devdoc>
        private DragDropEffects effect;

        /// <include file='doc\DragEvent.uex' path='docs/doc[@for="DragEventArgs.DragEventArgs"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.DragEventArgs'/> class.
        ///    </para>
        /// </devdoc>
        public DragEventArgs(IDataObject data, int keyState, int x, int y, DragDropEffects allowedEffect, DragDropEffects effect) {
            this.data = data;
            this.keyState = keyState;
            this.x = x;
            this.y = y;
            this.allowedEffect = allowedEffect;
            this.effect = effect;
        }

        /// <include file='doc\DragEvent.uex' path='docs/doc[@for="DragEventArgs.Data"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The <see cref='System.Windows.Forms.IDataObject'/>
        ///       that contains the data associated with this event.
        ///    </para>
        /// </devdoc>
        public IDataObject Data {
            get {
                return data;
            }
        }
        /// <include file='doc\DragEvent.uex' path='docs/doc[@for="DragEventArgs.KeyState"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       the current state of the SHIFT, CTRL, and ALT keys.
        ///
        ///    </para>
        /// </devdoc>
        public int KeyState {
            get {
                return keyState;
            }
        }
        /// <include file='doc\DragEvent.uex' path='docs/doc[@for="DragEventArgs.X"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the
        ///       x-coordinate
        ///       of the mouse pointer.
        ///    </para>
        /// </devdoc>
        public int X {
            get {
                return x;
            }
        }
        /// <include file='doc\DragEvent.uex' path='docs/doc[@for="DragEventArgs.Y"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       the y-coordinate
        ///       of the mouse pointer.
        ///    </para>
        /// </devdoc>
        public int Y {
            get {
                return y;
            }
        }
        /// <include file='doc\DragEvent.uex' path='docs/doc[@for="DragEventArgs.AllowedEffect"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets which drag-and-drop operations are allowed by the
        ///       originator (or source) of the drag event.
        ///    </para>
        /// </devdoc>
        public DragDropEffects AllowedEffect {
            get {
                return allowedEffect;
            }
        }
        /// <include file='doc\DragEvent.uex' path='docs/doc[@for="DragEventArgs.Effect"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets which drag-and-drop operations are allowed by the target of the drag event.
        ///    </para>
        /// </devdoc>
        public DragDropEffects Effect {
            get {
                return effect;
            }
            set {
                effect = value;
            }
        }
    }
}
