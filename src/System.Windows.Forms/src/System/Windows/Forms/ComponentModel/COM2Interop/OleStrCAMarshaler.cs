// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    using System.Runtime.InteropServices;
    using System;

    /// <summary>
    ///  This class performs marshaling on a CALPOLESTR struct given
    ///  from native code.
    /// </summary>
    internal class OleStrCAMarshaler : BaseCAMarshaler
    {
        public OleStrCAMarshaler(NativeMethods.CA_STRUCT caAddr) : base(caAddr)
        {
        }

        /// <summary>
        ///  Returns the type of item this marshaler will
        ///  return in the items array.  In this case, the type is string.
        /// </summary>
        public override Type ItemType
        {
            get
            {
                return typeof(string);
            }
        }

        protected override Array CreateArray()
        {
            return new string[Count];
        }

        /// <summary>
        ///  Override this member to perform marshalling of a single item
        ///  given it's native address.
        /// </summary>
        protected override object GetItemFromAddress(IntPtr addr)
        {
            string item = Marshal.PtrToStringUni(addr);
            // free the memory
            Marshal.FreeCoTaskMem(addr);
            return item;
        }
    }
}
