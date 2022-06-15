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

        [WinFormsFact]
        public void DomainItemListAccessibleObject_Name_Default_ReturnsExpected()
        {
            var accessibleObject = new DomainItemListAccessibleObject(null);
            Assert.Equal(accessibleObject.TestAccessor().Dynamic.DefaultName, accessibleObject.Name);
        }

        [WinFormsFact]
        public void DomainItemListAccessibleObject_State_Default_ReturnsExpected()
        {
            var accessibleObject = new DomainItemListAccessibleObject(null);
            Assert.Equal(AccessibleStates.Invisible | AccessibleStates.Offscreen, accessibleObject.State);
        }

        [WinFormsFact]
        public void DomainItemListAccessibleObject_GetChild_ReturnsExpected()
        {
            using DomainUpDown control = new();
            control.Items.AddRange(new[] { "Item1", "Item2", "Item3" });
            AccessibleObject accessibleObject = control.AccessibilityObject.TestAccessor().Dynamic.ItemList;

            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Equal("Item1", accessibleObject.GetChild(0).Name);
            Assert.Equal("Item2", accessibleObject.GetChild(1).Name);
            Assert.Equal("Item3", accessibleObject.GetChild(2).Name);
            Assert.Null(accessibleObject.GetChild(100));

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DomainItemListAccessibleObject_GetChild_ThrowsNullReferenceException_IfOwnerIsNull()
        {
            Assert.Throws<NullReferenceException>(() => new DomainItemListAccessibleObject(null).GetChild(0));
        }

        [WinFormsFact]
        public void DomainItemListAccessibleObject_GetChildCount_ReturnsExpected()
        {
            using DomainUpDown control = new();
            control.Items.AddRange(new[] { "Item1", "Item2", "Item3" });
            AccessibleObject accessibleObject = control.AccessibilityObject.TestAccessor().Dynamic.ItemList;

            Assert.Equal(control.Items.Count, accessibleObject.GetChildCount());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DomainItemListAccessibleObject_GetChildCount_ThrowsNullReferenceException_IfOwnerIsNull()
        {
            Assert.Throws<NullReferenceException>(() => new DomainItemListAccessibleObject(null).GetChildCount());
        }
    }
}
