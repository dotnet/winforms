// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Richedit
    {
        public enum RECO : uint
        {
            PASTE = 0x00000000,
            DROP = 0x00000001,
            COPY = 0x00000002,
            CUT = 0x00000003,
            DRAG = 0x00000004
        }
    }
}
