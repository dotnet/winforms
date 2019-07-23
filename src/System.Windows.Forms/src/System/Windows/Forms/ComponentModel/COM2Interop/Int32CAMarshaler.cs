// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  This class performs marshaling on a CADWORD struct given
    ///  from native code.
    /// </summary>
    internal class Int32CAMarshaler : BaseCAMarshaler
    {
        public Int32CAMarshaler(NativeMethods.CA_STRUCT caStruct) : base(caStruct)
        {
        }

        /// <summary>
        ///  Returns the type of item this marshaler will
        ///  return in the items array.  In this case, the type is int.
        /// </summary>
        public override Type ItemType
        {
            get
            {
                return typeof(int);
            }
        }

        protected override Array CreateArray()
        {
            return new int[Count];
        }

        /// <summary>
        ///  Override this member to perform marshalling of a single item
        ///  given it's native address.
        /// </summary>
        protected override object GetItemFromAddress(IntPtr addr)
        {
            return addr.ToInt32();
        }
    }

}
