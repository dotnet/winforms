// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewLinkCellAccessibleObjectTests : DataGridViewLinkCell
    {
        [WinFormsFact]
        public void DataGridViewLinkCellAccessibleObject_Ctor_Default()
        {
            var accessibleObject = new DataGridViewLinkCellAccessibleObject(null);

            Assert.Null(accessibleObject.Owner);
            Assert.Equal(AccessibleRole.Cell, accessibleObject.Role);
        }

        [WinFormsFact]
        public void DataGridViewLinkCellAccessibleObject_DefaultAction_ReturnsExpected()
        {
            var accessibleObject = new DataGridViewLinkCellAccessibleObject(null);

            Assert.Equal(SR.DataGridView_AccLinkCellDefaultAction, accessibleObject.DefaultAction);
        }

        [WinFormsFact]
        public void DataGridViewLinkCellAccessibleObject_GetChildCount_ReturnsExpected()
        {
            var accessibleObject = new DataGridViewLinkCellAccessibleObject(null);

            Assert.Equal(0, accessibleObject.GetChildCount());
        }

        [WinFormsFact]
        public void DataGridViewLinkCellAccessibleObject_IsIAccessibleExSupported_ReturnsExpected()
        {
            var accessibleObject = new DataGridViewLinkCellAccessibleObject(null);

            Assert.True(accessibleObject.IsIAccessibleExSupported());
        }

        [WinFormsFact]
        public void DataGridViewLinkCellAccessibleObject_ControlType_ReturnsExpected()
        {
            var accessibleObject = new DataGridViewLinkCellAccessibleObject(null);

            UiaCore.UIA expected = UiaCore.UIA.HyperlinkControlTypeId;

            Assert.Equal(expected, accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
        }

        [WinFormsFact]
        public void DataGridViewLinkCellAccessibleObject_DoDefaultAction_ThrowsException_IfOwnerIsNull()
        {
            Assert.Throws<InvalidOperationException>(() =>
            new DataGridViewLinkCellAccessibleObject(null).DoDefaultAction());
        }

        [WinFormsFact]
        public void DataGridViewLinkCellAccessibleObject_DoDefaultAction_ThrowsException_IfRowIndexIsIncorrect()
        {
            using DataGridViewLinkCell cell = new();

            Assert.Equal(-1, cell.RowIndex);
            Assert.Throws<InvalidOperationException>(() => cell.AccessibilityObject.DoDefaultAction());
        }
    }
}
