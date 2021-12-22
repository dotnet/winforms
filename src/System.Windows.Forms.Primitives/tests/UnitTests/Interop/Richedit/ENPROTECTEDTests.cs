// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop.Richedit;

namespace System.Windows.Forms.Primitives.Tests.Interop.Richedit
{
    public class ENPROTECTEDTests
    {
        [Fact]
        public unsafe void Enprotected_Size_Get_ReturnsExpected()
        {
            if (IntPtr.Size == 4)
            {
                Assert.Equal(32, sizeof(ENPROTECTED));
            }
            else
            {
                Assert.Equal(56, sizeof(ENPROTECTED));
            }
        }
    }
}
