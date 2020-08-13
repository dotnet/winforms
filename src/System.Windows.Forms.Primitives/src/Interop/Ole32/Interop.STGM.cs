// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        /// <summary>
        ///  Stream / storage modes.
        /// <see href="https://docs.microsoft.com/en-us/windows/desktop/Stg/stgm-constants"/>
        /// </summary>
        [Flags]
        public enum STGM : uint
        {
            /// <summary>
            ///  Read only, and each change to a storage or stream element is written as it occurs.
            ///  Fails if the given storage object already exists.
            ///  [DIRECT] [READ] [FAILIFTHERE] [SHARE_DENY_WRITE]
            /// </summary>
            Default = 0x00000000,
            READ = 0x00000000,
            TRANSACTED = 0x00010000,
            SIMPLE = 0x08000000,
            WRITE = 0x00000001,
            READWRITE = 0x00000002,
            SHARE_DENY_NONE = 0x00000040,
            SHARE_DENY_READ = 0x00000030,
            SHARE_DENY_WRITE = 0x00000020,
            SHARE_EXCLUSIVE = 0x00000010,
            PRIORITY = 0x00040000,
            DELETEONRELEASE = 0x04000000,
            NOSCRATCH = 0x00100000,
            CREATE = 0x00001000,
            CONVERT = 0x00020000,
            NOSNAPSHOT = 0x00200000,
            DIRECT_SWMR = 0x00400000
        }
    }
}
