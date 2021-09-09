// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.DomainUpDown;

namespace System.Windows.Forms.Tests
{
    public class DomainUpDown_DomainItemListAccessibleObjectTests
    {
        [WinFormsFact]
        public void DomainItemListAccessibleObject_Ctor_Default()
        {
            var accessibleObject = new DomainItemListAccessibleObject(null);
            Assert.Null(accessibleObject.Parent);
            Assert.Equal(AccessibleRole.List, accessibleObject.Role);
        }
    }
}
