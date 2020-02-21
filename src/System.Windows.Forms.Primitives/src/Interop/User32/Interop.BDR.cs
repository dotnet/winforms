// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum BDR : uint
        {
            RAISEDOUTER = 0x0001,
            SUNKENOUTER = 0x0002,
            RAISEDINNER = 0x0004,
            SUNKENINNER = 0x0008,
            OUTER = RAISEDOUTER | SUNKENOUTER,
            INNER = RAISEDINNER | SUNKENINNER,
            RAISED = RAISEDOUTER | RAISEDINNER,
            SUNKEN = SUNKENOUTER | SUNKENINNER,
        }
    }
}
