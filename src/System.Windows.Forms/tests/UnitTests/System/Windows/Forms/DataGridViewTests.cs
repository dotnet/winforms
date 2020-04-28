// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DataGridView_Ctor_Default()
        {
            using var dataGridView = new DataGridView();
            Assert.NotNull(dataGridView.RowTemplate);
            Assert.Same(dataGridView.RowTemplate, dataGridView.RowTemplate);
        }

        public static IEnumerable<object[]> Parent_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Control() };
            yield return new object[] { new DataGridView() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void DataGridView_Parent_Set_GetReturnsExpected(Control value)
        {
            using var control = new DataGridView
            {
                Parent = value
            };
            Assert.Same(value, control.Parent);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void DataGridView_Parent_SetWithNonNullOldParent_GetReturnsExpected(Control value)
        {
            using var oldParent = new Control();
            using var control = new DataGridView
            {
                Parent = oldParent
            };

            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.Empty(oldParent.Controls);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.Empty(oldParent.Controls);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridView_Parent_SetNonNull_AddsToControls()
        {
            using var parent = new Control();
            using var control = new DataGridView
            {
                Parent = parent
            };
            Assert.Same(parent, control.Parent);
            Assert.Same(control, Assert.Single(parent.Controls));
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Same(control, Assert.Single(parent.Controls));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void DataGridView_Parent_SetWithHandle_GetReturnsExpected(Control value)
        {
            using var control = new DataGridView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void DataGridView_Parent_SetWithHandler_CallsParentChanged()
        {
            using var parent = new Control();
            using var control = new DataGridView();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.ParentChanged += handler;

            // Set different.
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Equal(1, callCount);

            // Set same.
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Equal(1, callCount);

            // Set null.
            control.Parent = null;
            Assert.Null(control.Parent);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ParentChanged -= handler;
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void DataGridView_Parent_SetSame_ThrowsArgumentException()
        {
            using var control = new DataGridView();
            Assert.Throws<ArgumentException>(null, () => control.Parent = control);
            Assert.Null(control.Parent);
        }
    }
}
