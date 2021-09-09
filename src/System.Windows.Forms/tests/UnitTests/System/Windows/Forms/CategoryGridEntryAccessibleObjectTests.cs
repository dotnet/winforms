// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.PropertyGridInternal;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class CategoryGridEntryAccessibleObjectTests
    {
        [WinFormsFact]
        public void CategoryGridEntryAccessibleObject_Ctor_OwnerGridEntryCannotBeNull()
        {
            using PropertyGrid control = new();
            SubGridEntry gridEntry = new(control, null);

            Assert.Throws<NullReferenceException>(() => new CategoryGridEntry(control, null, "Name", new []{ gridEntry }));
        }

        private class SubGridEntry: GridEntry
        {
            public SubGridEntry(PropertyGrid ownerGrid, GridEntry parent) : base(ownerGrid, parent)
            { }
        }
    }
}
