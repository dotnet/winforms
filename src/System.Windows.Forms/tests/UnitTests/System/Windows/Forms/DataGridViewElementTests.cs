// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewElementTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DataGridViewElement_Ctor_Default()
        {
            var element = new DataGridViewElement();
            Assert.Null(element.DataGridView);
            Assert.Equal(DataGridViewElementStates.Visible, element.State);
        }

        [WinFormsFact]
        public void DataGridViewElement_OnDataGridViewChanged_Invoke_Nop()
        {
            var element = new SubDataGridViewElement();
            element.OnDataGridViewChanged();
        }

        public static IEnumerable<object[]> DataGridViewCellEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new DataGridViewCellEventArgs(1, 2) };
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellEventArgs_TestData))]
        public void DataGridViewElement_RaiseCellClick_Invoke_Nop(DataGridViewCellEventArgs eventArgs)
        {
            var element = new SubDataGridViewElement();
            element.RaiseCellClick(eventArgs);
        }

        [WinFormsFact]
        public void DataGridViewElement_RaiseCellClick_InvokeWithDataGridView_Success()
        {
            var eventArgs = new DataGridViewCellEventArgs(0, 0);
            using var control = new DataGridView
            {
                RowCount = 1,
                ColumnCount = 1
            };
            var element = new SubDataGridViewCell();
            control.Rows[0].Cells[0] = element;

            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(sender, control);
                Assert.Same(eventArgs, e);
                callCount++;
            }

            // Call with handler.
            control.CellClick += handler;
            element.RaiseCellClick(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.CellClick -= handler;
            element.RaiseCellClick(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void DataGridViewElement_RaiseCellClick_NullE_ThrowsNullReferenceException()
        {
            using var control = new DataGridView
            {
                RowCount = 1,
                ColumnCount = 1
            };
            var element = new SubDataGridViewCell();
            control.Rows[0].Cells[0] = element;
            Assert.Throws<NullReferenceException>(() => element.RaiseCellClick(null));
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellEventArgs_TestData))]
        public void DataGridViewElement_RaiseCellContentClick_Invoke_Nop(DataGridViewCellEventArgs eventArgs)
        {
            var element = new SubDataGridViewElement();
            element.RaiseCellContentClick(eventArgs);
        }

        [WinFormsFact]
        public void DataGridViewElement_RaiseCellContentClick_InvokeWithDataGridView_Success()
        {
            var eventArgs = new DataGridViewCellEventArgs(0, 0);
            using var control = new DataGridView
            {
                RowCount = 1,
                ColumnCount = 1
            };
            var element = new SubDataGridViewCell();
            control.Rows[0].Cells[0] = element;

            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(sender, control);
                Assert.Same(eventArgs, e);
                callCount++;
            }

            // Call with handler.
            control.CellContentClick += handler;
            element.RaiseCellContentClick(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.CellContentClick -= handler;
            element.RaiseCellContentClick(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void DataGridViewElement_RaiseCellContentClick_NullE_ThrowsNullReferenceException()
        {
            using var control = new DataGridView
            {
                RowCount = 1,
                ColumnCount = 1
            };
            var element = new SubDataGridViewCell();
            control.Rows[0].Cells[0] = element;
            Assert.Throws<NullReferenceException>(() => element.RaiseCellContentClick(null));
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellEventArgs_TestData))]
        public void DataGridViewElement_RaiseCellContentDoubleClick_Invoke_Nop(DataGridViewCellEventArgs eventArgs)
        {
            var element = new SubDataGridViewElement();
            element.RaiseCellContentDoubleClick(eventArgs);
        }

        [WinFormsFact]
        public void DataGridViewElement_RaiseCellContentDoubleClick_InvokeWithDataGridView_Success()
        {
            var eventArgs = new DataGridViewCellEventArgs(0, 0);
            using var control = new DataGridView
            {
                RowCount = 1,
                ColumnCount = 1
            };
            var element = new SubDataGridViewCell();
            control.Rows[0].Cells[0] = element;

            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(sender, control);
                Assert.Same(eventArgs, e);
                callCount++;
            }

            // Call with handler.
            control.CellContentDoubleClick += handler;
            element.RaiseCellContentDoubleClick(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.CellContentDoubleClick -= handler;
            element.RaiseCellContentDoubleClick(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void DataGridViewElement_RaiseCellContentDoubleClick_NullE_ThrowsNullReferenceException()
        {
            using var control = new DataGridView
            {
                RowCount = 1,
                ColumnCount = 1
            };
            var element = new SubDataGridViewCell();
            control.Rows[0].Cells[0] = element;
            Assert.Throws<NullReferenceException>(() => element.RaiseCellContentDoubleClick(null));
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellEventArgs_TestData))]
        public void DataGridViewElement_RaiseCellValueChanged_Invoke_Nop(DataGridViewCellEventArgs eventArgs)
        {
            var element = new SubDataGridViewElement();
            element.RaiseCellValueChanged(eventArgs);
        }

        [WinFormsFact]
        public void DataGridViewElement_RaiseCellValueChanged_InvokeWithDataGridView_Success()
        {
            var eventArgs = new DataGridViewCellEventArgs(0, 0);
            using var control = new DataGridView
            {
                RowCount = 1,
                ColumnCount = 1
            };
            var element = new SubDataGridViewCell();
            control.Rows[0].Cells[0] = element;

            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(sender, control);
                Assert.Same(eventArgs, e);
                callCount++;
            }

            // Call with handler.
            control.CellValueChanged += handler;
            element.RaiseCellValueChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.CellValueChanged -= handler;
            element.RaiseCellValueChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void DataGridViewElement_RaiseCellValueChanged_NullE_ThrowsNullReferenceException()
        {
            using var control = new DataGridView
            {
                RowCount = 1,
                ColumnCount = 1
            };
            var element = new SubDataGridViewCell();
            control.Rows[0].Cells[0] = element;
            Assert.Throws<NullReferenceException>(() => element.RaiseCellValueChanged(null));
        }

        public static IEnumerable<object[]> DataGridViewDataErrorEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new DataGridViewDataErrorEventArgs(new Exception(), 1, 2, DataGridViewDataErrorContexts.Formatting) };
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewDataErrorEventArgs_TestData))]
        public void DataGridViewElement_RaiseDataError_Invoke_Nop(DataGridViewDataErrorEventArgs eventArgs)
        {
            var element = new SubDataGridViewElement();
            element.RaiseDataError(eventArgs);
        }

        [WinFormsFact]
        public void DataGridViewElement_RaiseDataError_InvokeWithDataGridView_Success()
        {
            var eventArgs = new DataGridViewDataErrorEventArgs(new Exception(), 1, 2, DataGridViewDataErrorContexts.Formatting);
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(new AmbientProperties());
            mockSite
                .Setup(s => s.Name)
                .Returns((string)null);
            using var control = new DataGridView
            {
                RowCount = 1,
                ColumnCount = 1,
                Site = mockSite.Object
            };
            var element = new SubDataGridViewCell();
            control.Rows[0].Cells[0] = element;

            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(sender, control);
                Assert.Same(eventArgs, e);
                callCount++;
            }

            // Call with handler.
            control.DataError += handler;
            element.RaiseDataError(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.DataError -= handler;
            element.RaiseDataError(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void DataGridViewElement_RaiseDataError_NullE_ThrowsNullReferenceException()
        {
            using var control = new DataGridView
            {
                RowCount = 1,
                ColumnCount = 1
            };
            var element = new SubDataGridViewCell();
            control.Rows[0].Cells[0] = element;
            Assert.Throws<NullReferenceException>(() => element.RaiseDataError(null));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void DataGridViewElement_RaiseMouseWheel_Invoke_Nop(MouseEventArgs eventArgs)
        {
            var element = new SubDataGridViewElement();
            element.RaiseMouseWheel(eventArgs);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void DataGridViewElement_RaiseMouseWheel_InvokeWithDataGridView_Success(MouseEventArgs eventArgs)
        {
            using var control = new DataGridView
            {
                RowCount = 1,
                ColumnCount = 1
            };
            var element = new SubDataGridViewCell();
            control.Rows[0].Cells[0] = element;

            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(sender, control);
                Assert.Same(eventArgs, e);
                callCount++;
            }

            // Call with handler.
            control.MouseWheel += handler;
            element.RaiseMouseWheel(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.MouseWheel -= handler;
            element.RaiseMouseWheel(eventArgs);
            Assert.Equal(1, callCount);
        }

        private class SubDataGridViewCell : DataGridViewCell
        {
            public new void RaiseCellClick(DataGridViewCellEventArgs e) => base.RaiseCellClick(e);

            public new void RaiseCellContentClick(DataGridViewCellEventArgs e) => base.RaiseCellContentClick(e);

            public new void RaiseCellContentDoubleClick(DataGridViewCellEventArgs e) => base.RaiseCellContentDoubleClick(e);

            public new void RaiseCellValueChanged(DataGridViewCellEventArgs e) => base.RaiseCellValueChanged(e);

            public new void RaiseDataError(DataGridViewDataErrorEventArgs e) => base.RaiseDataError(e);

            public new void RaiseMouseWheel(MouseEventArgs e) => base.RaiseMouseWheel(e);
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
