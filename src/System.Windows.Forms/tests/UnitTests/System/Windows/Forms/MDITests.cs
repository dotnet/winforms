// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class MDITests : IClassFixture<ThreadExceptionFixture>
    {
        private Form _parentForm;
        public MDITests()
        {
            _parentForm = new();
            _parentForm.IsMdiContainer = true;
            _parentForm.ClientSize = new Size(640, 480);
        }

        [WinFormsFact]
        public void MDIForm_ResizeWhenMdiChildrenMinimizedAnchorBottom_Default()
        {
            // Currently Maui test scenarios run on the same instance and this flag might have been set to a non-default values in the previous tests. So, explicitly setting it to `default' value `true` to make tests reliable.
            _parentForm.MdiChildrenMinimizedAnchorBottom = true;

            using Form childForm = new()
            {
                MdiParent = _parentForm,
                WindowState = FormWindowState.Minimized
            };
            _parentForm.Show();
            childForm.Show();

            int childFormMinimizedYPositionFromBottom = _parentForm.ClientSize.Height - childForm.Top;
            _parentForm.Height += 100;

            Assert.Equal(childFormMinimizedYPositionFromBottom, _parentForm.ClientSize.Height - childForm.Top);
        }

        [WinFormsFact]
        public void MDIForm_ResizeWhenMdiChildrenMinimizedAnchorBottom_False()
        {
            _parentForm.MdiChildrenMinimizedAnchorBottom = false;

            using Form childForm = new()
            {
                MdiParent = _parentForm,
                WindowState = FormWindowState.Minimized
            };
            _parentForm.Show();
            childForm.Show();

            int childFormMinimizedTop = childForm.Top;
            _parentForm.Height += 100;

            Assert.Equal(childFormMinimizedTop, childForm.Top);
        }
    }
}
