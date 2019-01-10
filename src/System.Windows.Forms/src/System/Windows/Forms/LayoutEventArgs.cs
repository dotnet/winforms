// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.ComponentModel;

    /// <include file='doc\LayoutEvent.uex' path='docs/doc[@for="LayoutEventArgs"]/*' />
    /// <devdoc>
    /// </devdoc>
    public sealed class LayoutEventArgs : EventArgs {
        private readonly IComponent affectedComponent;
        private readonly string affectedProperty;

        /// <include file='doc\LayoutEvent.uex' path='docs/doc[@for="LayoutEventArgs.LayoutEventArgs"]/*' />
        public LayoutEventArgs(IComponent affectedComponent, string affectedProperty) {
            this.affectedComponent = affectedComponent;
            this.affectedProperty = affectedProperty;
        }

        // This ctor required for binary compatibility with RTM.
        /// <include file='doc\LayoutEvent.uex' path='docs/doc[@for="LayoutEventArgs.LayoutEventArgs1"]/*' />
        public LayoutEventArgs(Control affectedControl, string affectedProperty)
            : this((IComponent)affectedControl, affectedProperty) {
        }

        /// <include file='doc\LayoutEvent.uex' path='docs/doc[@for="LayoutEventArgs.AffectedComponent"]/*' />
        public IComponent AffectedComponent {
            get {
                return affectedComponent;
            }
        }

        /// <include file='doc\LayoutEvent.uex' path='docs/doc[@for="LayoutEventArgs.AffectedControl"]/*' />
        public Control AffectedControl {
            get {
                return affectedComponent as Control;
            }
        }

        /// <include file='doc\LayoutEvent.uex' path='docs/doc[@for="LayoutEventArgs.AffectedProperty"]/*' />
        public string AffectedProperty {
            get {
                return affectedProperty;
            }
        }
    }
}
