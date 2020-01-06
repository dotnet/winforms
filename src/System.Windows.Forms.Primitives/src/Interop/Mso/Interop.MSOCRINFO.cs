// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Mso
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MSOCRINFO
        {
            /// <summary>
            ///  The size, in bytes, of the <see cref="MSOCRINFO" /> structure.
            /// </summary>
            public uint cbSize;

            /// <summary>
            ///  The interval time, in milliseconds, of when a periodic idle phase
            ///  should occur. During the idle phase, the component needs to perform
            ///  idle-time tasks. This member applies only if the msocrfNeedPeriodicIdleTime
            ///  bit flag is registered in the <see cref="grfcrf" /> member.
            /// </summary>
            public uint uIdleTimeInterval;

            /// <summary>
            ///  Registration flags.
            /// </summary>
            public msocrf grfcrf;

            /// <summary>
            ///  Registration advise flags.
            /// </summary>
            public msocadvf grfcadvf;
        }
    }
}
