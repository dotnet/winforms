// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal partial class Kernel32
    {
        /// <summary>
        /// This access right is checked against the security descriptor for the process
        /// </summary>
        [Flags]
        public enum ProcessAccessOptions : uint
        {
            TERMINATE = 0x0001,
            CREATE_THREAD  = 0x0002,
            VM_OPERATION = 0x0008,
            VM_READ = 0x0010,
            VM_WRITE = 0x0020,
            DUP_HANDLE = 0x0040,
            CREATE_PROCESS  = 0x0080,
            SET_QUOTA = 0x0100,
            SET_INFORMATION = 0x0200,
            QUERY_INFORMATION = 0x0400
        }
    }
}
