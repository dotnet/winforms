// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal partial class Imm32
    {
        public enum NI : uint
        {
            OPENCANDIDATE = 0x0010,
            CLOSECANDIDATE = 0x0011,
            SELECTCANDIDATESTR = 0x0012,
            CHANGECANDIDATELIST = 0x0013,
            FINALIZECONVERSIONRESULT = 0x0014,
            COMPOSITIONSTR = 0x0015,
            SETCANDIDATE_PAGESTART = 0x0016,
            SETCANDIDATE_PAGESIZE = 0x0017,
            IMEMENUSELECTED = 0x0018,
        }
    }
}
