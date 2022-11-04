// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Windows.Forms
{
    internal static class TraceSwitchExtensions
    {
        public static void TraceVerbose(this TraceSwitch? traceSwitch, string message)
        {
            if (traceSwitch is not null && traceSwitch.TraceVerbose)
            {
                Debug.WriteLine(message);
            }
        }
    }
}
