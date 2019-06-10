// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms.IntegrationTests.Common
{
    public class ExternalTestHelpers
    {
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        [DllImport("User32.dll")]
        static extern IntPtr GetForegroundWindow();

        public static int TrySetForegroundWindow(IntPtr point)
        {
            return SetForegroundWindow(point);
        }

        public static IntPtr TryGetForegroundWindow()
        {
            return GetForegroundWindow();
        }
    }
}
