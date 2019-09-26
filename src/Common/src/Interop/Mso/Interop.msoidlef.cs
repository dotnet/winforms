// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Mso
    {
        [Flags]
        public enum msoidlef : uint
        {
            Periodic = 1,
            NonPeriodic = 2,
            Priority = 4,
            All = 0xFFFFFFFF
        }
    }
}
