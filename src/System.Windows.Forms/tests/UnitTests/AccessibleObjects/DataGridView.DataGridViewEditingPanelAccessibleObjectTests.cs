// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class DataGridView_DataGridViewEditingPanelAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DataGridViewEditingPanelAccessibleObject_FirstAndLastChildren_AreNull()
        {
            using DataGridView dataGridView = new DataGridView();
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
            AccessibleObject accessibleObject = dataGridView.EditingPanelAccessibleObject;

            // Exception does not appear when trying to get first Child
            UiaCore.IRawElementProviderFragment firstChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            Assert.Null(firstChild);

            // Exception does not appear when trying to get last Child
            UiaCore.IRawElementProviderFragment lastChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild);
            Assert.Null(lastChild);
        }

        [WinFormsFact]
        public void DataGridViewEditingPanelAccessibleObject_EditedState_FirstAndLastChildren_AreNotNull()
        {
            using DataGridView dataGridView = new DataGridView();
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
            DataGridViewCell cell = dataGridView.Rows[0].Cells[0];
            dataGridView.CurrentCell = cell;
            dataGridView.BeginEdit(false);

            AccessibleObject accessibleObject = dataGridView.EditingPanelAccessibleObject;

            UiaCore.IRawElementProviderFragment firstChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            Assert.NotNull(firstChild);

            UiaCore.IRawElementProviderFragment lastChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild);
            Assert.NotNull(lastChild);
        }
    }
}
