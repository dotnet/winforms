// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        /// <summary>
        ///  <see cref="RedrawWindow(IntPtr, RECT*, Gdi32.HRGN, RDW)"/> flags.
        /// </summary>
        [Flags]
        public enum RDW : uint
        {
            INVALIDATE = 0x0001,
            INTERNALPAINT = 0x0002,
            ERASE = 0x0004,
            VALIDATE = 0x0008,
            NOINTERNALPAINT = 0x0010,
            NOERASE = 0x0020,
            NOCHILDREN = 0x0040,
            ALLCHILDREN = 0x0080,
            UPDATENOW = 0x0100,
            ERASENOW = 0x0200,
            FRAME = 0x0400,
            NOFRAME = 0x0800,
        }
    }
}
