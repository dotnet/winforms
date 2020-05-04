// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Mshtml
    {
        [ComImport]
        [Guid("332C4426-26CB-11D0-B483-00C04FD90119")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLFramesCollection2
        {
            object Item(ref object idOrName);
            int GetLength();
        }
    }
}
