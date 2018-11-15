// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel {
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <internalonly/>
    // Shared between dlls
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    internal static class CoreSwitches {   
    
        private static BooleanSwitch perfTrack;                        
        
        public static BooleanSwitch PerfTrack {            
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            get {
                if (perfTrack == null) {
                    perfTrack  = new BooleanSwitch("PERFTRACK", "Debug performance critical sections.");       
                }
                return perfTrack;
            }
        }
    }
}    

