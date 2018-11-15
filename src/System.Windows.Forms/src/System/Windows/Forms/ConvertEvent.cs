// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;

    /// <include file='doc\ConvertEvent.uex' path='docs/doc[@for="ConvertEventArgs"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class ConvertEventArgs : EventArgs {

        private object value;
        private Type desiredType;

        /// <include file='doc\ConvertEvent.uex' path='docs/doc[@for="ConvertEventArgs.ConvertEventArgs"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public ConvertEventArgs(object value, Type desiredType) {
            this.value = value;
            this.desiredType = desiredType;
        }

        /// <include file='doc\ConvertEvent.uex' path='docs/doc[@for="ConvertEventArgs.Value"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public object Value {
            get {
                return value;
            }
            set {
                this.value = value;
            }
        }

        /// <include file='doc\ConvertEvent.uex' path='docs/doc[@for="ConvertEventArgs.DesiredType"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public Type DesiredType {
            get {
                return desiredType;
            }
        }
    }
}
