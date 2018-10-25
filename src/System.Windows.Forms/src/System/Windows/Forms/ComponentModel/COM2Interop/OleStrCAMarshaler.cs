// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop {
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Diagnostics;
    using System;


    /// <include file='doc\OleStrCAMarshaler.uex' path='docs/doc[@for="OleStrCAMarshaler"]/*' />
    /// <devdoc>
    ///   This class performs marshaling on a CALPOLESTR struct given
    ///   from native code.
    /// </devdoc>
    internal class OleStrCAMarshaler: BaseCAMarshaler {
        public OleStrCAMarshaler(NativeMethods.CA_STRUCT caAddr) : base(caAddr) {
        }

        /// <include file='doc\OleStrCAMarshaler.uex' path='docs/doc[@for="OleStrCAMarshaler.ItemType"]/*' />
        /// <devdoc>
        ///     Returns the type of item this marshaler will
        ///     return in the items array.  In this case, the type is string.
        /// </devdoc>
        public override Type ItemType {
            get {
                return typeof(string);
            }
        }

        protected override Array CreateArray() {
            return new string[Count];
        }

        /// <include file='doc\OleStrCAMarshaler.uex' path='docs/doc[@for="OleStrCAMarshaler.GetItemFromAddress"]/*' />
        /// <devdoc>
        ///     Override this member to perform marshalling of a single item
        ///     given it's native address.
        /// </devdoc>
        protected override object GetItemFromAddress(IntPtr addr) {
            string item =  Marshal.PtrToStringUni(addr);
            // free the memory
            Marshal.FreeCoTaskMem(addr);
            return item;
        }
    }
}
