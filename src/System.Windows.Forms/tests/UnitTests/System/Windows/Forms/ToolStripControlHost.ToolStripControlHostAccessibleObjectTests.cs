// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripControlHost;

namespace System.Windows.Forms.Tests
{
    public class ToolStripControlHost_ToolStripControlHostAccessibleObjectTests
    {
        [WinFormsFact]
        public void ToolStripControlHostAccessibleObject_Ctor_OwnerToolStripControlHostCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ToolStripControlHostAccessibleObject(null));
        }
    }
}
