// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  This class performs marshaling on a CALPOLESTR struct given from native code.
    /// </summary>
    internal class OleStrCAMarshaler : BaseCAMarshaler<IntPtr>
    {
        public OleStrCAMarshaler(in Ole32.CA caAddr) : base(caAddr)
        {
        }

        /// <summary>
        ///  Returns the type of item this marshaler will return in the items array.
        ///  In this case, the type is string.
        /// </summary>
        public override Type ItemType => typeof(string);

        protected override object GetItem(IntPtr item)
        {
            string value = Marshal.PtrToStringUni(item);
            Marshal.FreeCoTaskMem(item);
            return value;
        }
    }
}
