// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop.User32;

namespace System.Windows.Forms.Primitives.Tests.Interop.User32
{
    public class SysInfoTests
    {
        [Fact]
        public unsafe void NonClientMetrics_Size()
        {
            Assert.Equal(504, sizeof(NONCLIENTMETRICSW));
        }

        [Fact]
        public unsafe void HighContrast_Size()
        {
#pragma warning disable xUnit2000 // Constant on the right isn't typical
            Assert.Equal(Environment.Is64BitProcess ? 16 : 12, sizeof(HIGHCONTRASTW));
#pragma warning restore xUnit2000
        }
    }
}
