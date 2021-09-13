// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Windows.Forms.PropertyGridInternal;
using Xunit;
using static System.Windows.Forms.PropertyGridInternal.PropertyGridView;

namespace System.Windows.Forms.Tests
{
    public class PropertyGridView_GridViewListBoxItemAccessibleObjectTests
    {
        [WinFormsFact]
        public void GridViewListBoxItemAccessibleObject_Ctor_OwnerGridViewListBoxCannotBeNull()
        {
            Type type = typeof(PropertyGridView)
                .GetNestedType("GridViewListBoxItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.Throws<TargetInvocationException>(() => (AccessibleObject)Activator.CreateInstance(type, new object[] { null, new ItemArray.Entry("A") }));
        }

        [WinFormsFact]
        public void GridViewListBoxItemAccessibleObject_Ctor_OwnerItemCannotBeNull()
        {
            using GridViewListBox control = new(new PropertyGridView(null, null));
            Type type = typeof(PropertyGridView)
                .GetNestedType("GridViewListBoxItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.Throws<TargetInvocationException>(() => (AccessibleObject)Activator.CreateInstance(type, new object[] { control, null }));
        }
    }
}
