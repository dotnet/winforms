// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum EDGE : uint
        {
            RAISEDOUTER = BDR.RAISEDOUTER,
            SUNKENOUTER = BDR.SUNKENOUTER,
            RAISEDINNER = BDR.RAISEDINNER,
            SUNKENINNER = BDR.SUNKENINNER,
            RAISED = RAISEDOUTER | RAISEDINNER,
            SUNKEN = SUNKENOUTER | SUNKENINNER,
            ETCHED = SUNKENOUTER | RAISEDINNER,
            BUMP = RAISEDOUTER | SUNKENINNER,
        }
    }
}
