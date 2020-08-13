// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public enum ICM : uint
        {
            OFF = 1,
            ON = 2,
            QUERY = 3,
            DONE_OUTSIDEDC = 4
        }
    }
}
