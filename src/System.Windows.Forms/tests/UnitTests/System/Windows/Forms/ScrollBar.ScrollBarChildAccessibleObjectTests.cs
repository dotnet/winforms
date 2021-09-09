// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ScrollBar;

namespace System.Windows.Forms.Tests
{
    public class ScrollBar_ScrollBarChildAccessibleObjectTests
    {
        [WinFormsFact]
        public void ScrollBarChildAccessibleObject_Ctor_Default()
        {
            var accessibleObject = new ScrollBarChildAccessibleObject(null);
            Assert.Null(accessibleObject.OwningScrollBar);
        }
    }
}
