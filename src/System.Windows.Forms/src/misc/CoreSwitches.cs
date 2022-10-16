// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.ComponentModel
{
    // Shared between dlls
    internal static class CoreSwitches
    {
        private static BooleanSwitch? s_perfTrack;

        public static BooleanSwitch PerfTrack
        {
            get
            {
                s_perfTrack ??= new BooleanSwitch("PERFTRACK", "Debug performance critical sections.");

                return s_perfTrack;
            }
        }
    }
}

