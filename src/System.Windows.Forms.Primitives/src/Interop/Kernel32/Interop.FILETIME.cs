// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Kernel32
    {
        public struct FILETIME
        {
            public FILETIME(DateTime date)
            {
                long ft = date.ToFileTime();
                dwLowDateTime = (uint)(ft & 0xFFFFFFFF);
                dwHighDateTime = (uint)(ft >> 32);
            }

            public uint dwLowDateTime;
            public uint dwHighDateTime;

            public DateTime ToDateTime()
            {
                return DateTime.FromFileTime(((long)dwHighDateTime << 32) + dwLowDateTime);
            }
        }
    }
}
