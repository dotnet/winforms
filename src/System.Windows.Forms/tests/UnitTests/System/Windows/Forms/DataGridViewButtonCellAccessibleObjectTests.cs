// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewButtonCellAccessibleObjectTests : DataGridViewButtonCell
    {
        [WinFormsFact]
        public void DataGridViewButtonCellAccessibleObject_Ctor_Default()
        {
            var accessibleObject = new DataGridViewButtonCellAccessibleObject(null);
            Assert.Null(accessibleObject.Owner);
            Assert.Equal(AccessibleRole.Cell, accessibleObject.Role);
        }

        [WinFormsFact]
        public void DataGridViewButtonCellAccessibleObject_DefaultAction_ReturnsExpected()
        {
            var accessibleObject = new DataGridViewButtonCellAccessibleObject(null);
            Assert.Equal(SR.DataGridView_AccButtonCellDefaultAction, accessibleObject.DefaultAction);
        }

        [WinFormsFact]
        public void DataGridViewButtonCellAccessibleObject_GetChildCount_ReturnsExpected()
        {
            var accessibleObject = new DataGridViewButtonCellAccessibleObject(null);
            Assert.Equal(0, accessibleObject.GetChildCount());
        }

        [WinFormsFact]
        public void DataGridViewButtonCellAccessibleObject_IsIAccessibleExSupported_ReturnsExpected()
        {
            var accessibleObject = new DataGridViewButtonCellAccessibleObject(null);
            Assert.True(accessibleObject.IsIAccessibleExSupported());
        }

        [WinFormsFact]
        public void DataGridViewButtonCellAccessibleObject_ControlType_ReturnsExpected()
        {
            var accessibleObject = new DataGridViewButtonCellAccessibleObject(null);
            Assert.Equal(UiaCore.UIA.ButtonControlTypeId, accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
        }

        [WinFormsFact]
        public void DataGridViewButtonCellAccessibleObject_DoDefaultAction_ThrowsException_IfOwnerIsNull()
        {
            Assert.Throws <InvalidOperationException>(()
                => new DataGridViewButtonCellAccessibleObject(null).DoDefaultAction());
        }

        [WinFormsFact]
        public void DataGridViewButtonCellAccessibleObject_DoDefaultAction_ThrowsException_IfOwnerRowIndexIncorrect()
        {
            using DataGridViewCell cell = new DataGridViewButtonCell();
            AccessibleObject accessibleObject = cell.AccessibilityObject;

            Assert.Equal(-1, cell.RowIndex);
            Assert.Throws<InvalidOperationException>(() => accessibleObject.DoDefaultAction());
        }
    }
}
