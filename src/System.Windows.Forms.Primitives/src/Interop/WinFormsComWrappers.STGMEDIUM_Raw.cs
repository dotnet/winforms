// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices.ComTypes;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal struct STGMEDIUM_Raw
        {
            public TYMED tymed;
            public IntPtr unionmember;
            public IntPtr pUnkForRelease;
        }
    }
}
