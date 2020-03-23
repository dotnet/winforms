// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal partial class Kernel32
    {
        [Flags]
        public enum STARTF : uint
        {
            USESHOWWINDOW = 0x00000001,
            USESIZE = 0x00000002,
            USEPOSITION = 0x00000004,
            USECOUNTCHARS = 0x00000008,
            USEFILLATTRIBUTE = 0x00000010,
            RUNFULLSCREEN = 0x00000020,
            FORCEONFEEDBACK = 0x00000040,
            FORCEOFFFEEDBACK = 0x00000080,
            USESTDHANDLES = 0x00000100,
            USEHOTKEY = 0x00000200,
            TITLEISLINKNAME = 0x00000800,
            TITLEISAPPID = 0x00001000,
            PREVENTPINNING = 0x00002000,
            UNTRUSTEDSOURCE = 0x00008000,
        }
    }
}
