// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewElementTests
    {
        [Fact]
        public void Ctor_Default()
        {
            var element = new DataGridViewElement();
            Assert.Equal(DataGridViewElementStates.Visible, element.State);
            Assert.Null(element.DataGridView);
        }

        private class SubDataGridViewElement : DataGridViewElement
        {
            public void OnDataGridViewChangedEntry() => OnDataGridViewChanged();

            public void RaiseCellClickEntry(DataGridViewCellEventArgs e) => RaiseCellClick(e);

            public void RaiseCellContentClickEntry(DataGridViewCellEventArgs e) => RaiseCellContentClick(e);

            public void RaiseCellContentDoubleClickEntry(DataGridViewCellEventArgs e) => RaiseCellContentDoubleClick(e);
        
            public void RaiseCellValueChangedEntry(DataGridViewCellEventArgs e) => RaiseCellValueChanged(e);

            public void RaiseDataErrorEntry(DataGridViewDataErrorEventArgs e) => RaiseDataErrorEntry(e);

            public void RaiseMouseWheelEntry(MouseEventArgs e) => RaiseMouseWheel(e);
        }
    }
}
