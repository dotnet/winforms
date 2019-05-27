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

        [Fact]
        public void OnDataGridViewChanged_Invoke_Nop()
        {
            var element = new SubDataGridViewElement();
            element.OnDataGridViewChanged();
        }

        [Fact]
        public void RaiseCellClick_Invoke_Nop()
        {
            var element = new SubDataGridViewElement();
            element.RaiseCellClick(new DataGridViewCellEventArgs(1, 2));
        }

        [Fact]
        public void RaiseCellContentClick_Invoke_Nop()
        {
            var element = new SubDataGridViewElement();
            element.RaiseCellContentClick(new DataGridViewCellEventArgs(1, 2));
        }

        [Fact]
        public void RaiseCellContentDoubleClick_Invoke_Nop()
        {
            var element = new SubDataGridViewElement();
            element.RaiseCellContentDoubleClick(new DataGridViewCellEventArgs(1, 2));
        }

        [Fact]
        public void RaiseCellValueChanged_Invoke_Nop()
        {
            var element = new SubDataGridViewElement();
            element.RaiseCellValueChanged(new DataGridViewCellEventArgs(1, 2));
        }

        [Fact]
        public void RaiseDataError_Invoke_Nop()
        {
            var element = new SubDataGridViewElement();
            element.RaiseDataError(new DataGridViewDataErrorEventArgs(new Exception(), 1, 2, DataGridViewDataErrorContexts.Formatting));
        }

        [Fact]
        public void RaiseMouseWheel_Invoke_Nop()
        {
            var element = new SubDataGridViewElement();
            element.RaiseMouseWheel(new MouseEventArgs(MouseButtons.Left, 1, 2, 3, 4));
        }

        private class SubDataGridViewElement : DataGridViewElement
        {
            public new void OnDataGridViewChanged() => base.OnDataGridViewChanged();

            public new void RaiseCellClick(DataGridViewCellEventArgs e) => base.RaiseCellClick(e);

            public new void RaiseCellContentClick(DataGridViewCellEventArgs e) => base.RaiseCellContentClick(e);

            public new void RaiseCellContentDoubleClick(DataGridViewCellEventArgs e) => base.RaiseCellContentDoubleClick(e);

            public new void RaiseCellValueChanged(DataGridViewCellEventArgs e) => base.RaiseCellValueChanged(e);

            public new void RaiseDataError(DataGridViewDataErrorEventArgs e) => base.RaiseDataError(e);

            public new void RaiseMouseWheel(MouseEventArgs e) => base.RaiseMouseWheel(e);
        }
    }
}
