// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;


    /// <devdoc>
    ///      A ControlEventArgs is an event that has a control
    ///      as a property.
    /// </devdoc>
    public class ControlEventArgs : EventArgs {
        private Control control;

        /// <devdoc>
        ///      Retrieves the control object stored in this event.
        /// </devdoc>
        public Control Control {
            get {
                return control;
            }
        }

        /// <devdoc>
        ///      Creates a new ControlEventArgs.
        /// </devdoc>
        public ControlEventArgs(Control control) {
            this.control = control;
        }
    }
}

