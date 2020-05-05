// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripItemEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_ToolStripItem_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ToolStripButton() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_ToolStripItem_TestData))]
        public void Ctor_ToolStripItem(ToolStripItem item)
        {
            var e = new ToolStripItemEventArgs(item);
            Assert.Equal(item, e.Item);
        }
    }
}
