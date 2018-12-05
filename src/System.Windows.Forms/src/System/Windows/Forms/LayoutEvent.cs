// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.ComponentModel;

    /// <devdoc>
    /// </devdoc>
    public sealed class LayoutEventArgs : EventArgs {
        private readonly IComponent affectedComponent;
        private readonly string affectedProperty;

        public LayoutEventArgs(IComponent affectedComponent, string affectedProperty) {
            this.affectedComponent = affectedComponent;
            this.affectedProperty = affectedProperty;
        }

        // This ctor required for binary compatibility with RTM.
        public LayoutEventArgs(Control affectedControl, string affectedProperty)
            : this((IComponent)affectedControl, affectedProperty) {
        }

        public IComponent AffectedComponent {
            get {
                return affectedComponent;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public Control AffectedControl {
            get {
                return affectedComponent as Control;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string AffectedProperty {
            get {
                return affectedProperty;
            }
        }
    }
}
