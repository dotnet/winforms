// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal partial class Kernel32
    {
        [Flags]
        public enum ACTCTX_FLAG : uint
        {
            PROCESSOR_ARCHITECTURE_VALID = 0x00000001,
            LANGID_VALID = 0x00000002,
            ASSEMBLY_DIRECTORY_VALID = 0x00000004,
            RESOURCE_NAME_VALID = 0x00000008,
            SET_PROCESS_DEFAULT = 0x00000010,
            APPLICATION_NAME_VALID = 0x00000020,
            SOURCE_IS_ASSEMBLYREF = 0x00000040,
            HMODULE_VALID = 0x00000080,
        }
    }
}
