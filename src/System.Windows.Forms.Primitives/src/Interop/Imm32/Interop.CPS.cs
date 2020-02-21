// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal partial class Imm32
    {
        public enum CPS : uint
        {
            COMPLETE = 0x0001,
            CONVERT = 0x0002,
            REVERT = 0x0003,
            CANCEL = 0x0004,
        }
    }
}
