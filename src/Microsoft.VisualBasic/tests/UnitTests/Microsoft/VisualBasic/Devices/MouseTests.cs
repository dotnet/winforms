// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace Microsoft.VisualBasic.Devices.Tests
{
    public class MouseTests
    {
        [Fact]
        public void Properties()
        {
            var mouse = new Mouse();
            Invoke(() => _ = mouse.ButtonsSwapped);
            Invoke(() => _ = mouse.WheelExists);
            Invoke(() => _ = mouse.WheelScrollLines);
        }

        private static void Invoke(Action action)
        {
            if (System.Windows.Forms.SystemInformation.MousePresent)
            {
                action();
            }
            else
            {
                // Exception.ToString() called to verify message is constructed successfully.
                _ = Assert.Throws<InvalidOperationException>(action).ToString();
            }
        }
    }
}
