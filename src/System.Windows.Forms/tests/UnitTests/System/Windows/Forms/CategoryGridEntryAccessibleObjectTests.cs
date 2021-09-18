// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.PropertyGridInternal;
using Xunit;
using static System.Windows.Forms.PropertyGridInternal.CategoryGridEntry;

namespace System.Windows.Forms.Tests
{
    public class CategoryGridEntryAccessibleObjectTests
    {
        [WinFormsFact]
        public void CategoryGridEntryAccessibleObject_Ctor_OwnerCategoryGridEntryCannotBeNull()
        {
            using NoAssertContext context = new();
            var accessibilityObject = new CategoryGridEntryAccessibleObject(null);
            Assert.Null(accessibilityObject.TestAccessor().Dynamic._owningCategoryGridEntry);
        }

        private class SubGridEntry: GridEntry
        {
            public SubGridEntry(PropertyGrid ownerGrid, GridEntry parent) : base(ownerGrid, parent)
            { }
        }
    }
}
