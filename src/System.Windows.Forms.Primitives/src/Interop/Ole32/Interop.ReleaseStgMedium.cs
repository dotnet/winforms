// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        // Even though the windows headers indicate that STGMEDIUM is input only this is not true,
        // ReleaseStgMedium will clear the fields it released. It is important for classic marshaling
        // to *NOT* mark the argument as input parameter because otherwise .NET will not transfer
        // ownership of the COM pointers correctly. This defeats the purpose of calling this method.
        [DllImport(Libraries.Ole32, ExactSpelling = true)]
        public static extern void ReleaseStgMedium(ref STGMEDIUM pmedium);
    }
}
