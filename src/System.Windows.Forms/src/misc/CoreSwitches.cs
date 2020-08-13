// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;

namespace System.ComponentModel
{
    // Shared between dlls
    internal static class CoreSwitches
    {
        private static BooleanSwitch perfTrack;

        public static BooleanSwitch PerfTrack
        {
            get
            {
                if (perfTrack is null)
                {
                    perfTrack = new BooleanSwitch("PERFTRACK", "Debug performance critical sections.");
                }
                return perfTrack;
            }
        }
    }
}

