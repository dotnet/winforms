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
        public void DomainItemAccessibleObject_Ctor_Default()
        {
            string testName = "Some test name";
            var accessibleObject = new DomainItemAccessibleObject(testName, null);
            Assert.Null(accessibleObject.Parent);
            Assert.Equal(testName, accessibleObject.Name);
            Assert.Equal(AccessibleRole.ListItem, accessibleObject.Role);
        }

        [WinFormsTheory]
        [InlineData("Some string")]
        [InlineData("")]
        [InlineData(null)]
        public void DomainItemAccessibleObject_Name_Set_ReturnsExpected(string testValue)
        {
            var accessibleObject = new DomainItemAccessibleObject(null, null);
            accessibleObject.Name = testValue;
            Assert.Equal(testValue, accessibleObject.Name);
        }

        [WinFormsFact]
        public void DomainItemAccessibleObject_State_Default_ReturnsExpected()
        {
            var accessibleObject = new DomainItemAccessibleObject(null, null);
            Assert.Equal(AccessibleStates.Selectable, accessibleObject.State);
        }

        [WinFormsFact]
        public void DomainItemAccessibleObject_Value_Default_ReturnsExpected()
        {
            string testName = "Some test name";
            var accessibleObject = new DomainItemAccessibleObject(testName, null);
            Assert.Equal(testName, accessibleObject.Value);
        }
    }
}
