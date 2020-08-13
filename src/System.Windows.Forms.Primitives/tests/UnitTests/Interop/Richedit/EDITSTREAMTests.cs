// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop.Richedit;

namespace System.Windows.Forms.Tests.Interop.Richedit
{
    public class EDITSTREAMTests
    {
        [Fact]
        public unsafe void EditStream_Size_Get_ReturnsExpected()
        {
            if (IntPtr.Size == 4)
            {
                Assert.Equal(12, sizeof(EDITSTREAM));
            }
            else
            {
                Assert.Equal(20, sizeof(EDITSTREAM));
            }
        }
    }
}
