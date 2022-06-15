// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.DomainUpDown;

namespace System.Windows.Forms.Tests
{
    public class DomainUpDown_DomainItemAccessibleObjectTests
    {
        [WinFormsFact]
        public void DomainItemAccessibleObject_Ctor_NullParent_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DomainItemAccessibleObject(string.Empty, null));
        }

        [WinFormsFact]
        public void DomainItemAccessibleObject_Ctor_Default()
        {
            string testName = "Some test name";
            using var domainUpDown = new DomainUpDown();
            var parent = new DomainItemListAccessibleObject(new DomainUpDownAccessibleObject(domainUpDown));
            var accessibleObject = new DomainItemAccessibleObject(testName, parent);
            Assert.Equal(parent, accessibleObject.Parent);
            Assert.Equal(testName, accessibleObject.Name);
            Assert.Equal(AccessibleRole.ListItem, accessibleObject.Role);
        }

        [WinFormsTheory]
        [InlineData("Some string")]
        [InlineData("")]
        [InlineData(null)]
        public void DomainItemAccessibleObject_Name_Set_ReturnsExpected(string testValue)
        {
            using var domainUpDown = new DomainUpDown();
            var accessibleObject = new DomainItemAccessibleObject(null, new DomainItemListAccessibleObject(new DomainUpDownAccessibleObject(domainUpDown)));
            accessibleObject.Name = testValue;
            Assert.Equal(testValue, accessibleObject.Name);
        }

        [WinFormsFact]
        public void DomainItemAccessibleObject_State_Default_ReturnsExpected()
        {
            using var domainUpDown = new DomainUpDown();
            var accessibleObject = new DomainItemAccessibleObject(null, new DomainItemListAccessibleObject(new DomainUpDownAccessibleObject(domainUpDown)));
            Assert.Equal(AccessibleStates.Selectable, accessibleObject.State);
        }

        [WinFormsFact]
        public void DomainItemAccessibleObject_Value_Default_ReturnsExpected()
        {
            string testName = "Some test name";
            using var domainUpDown = new DomainUpDown();
            var accessibleObject = new DomainItemAccessibleObject(testName, new DomainItemListAccessibleObject(new DomainUpDownAccessibleObject(domainUpDown)));
            Assert.Equal(testName, accessibleObject.Value);
        }
    }
}
