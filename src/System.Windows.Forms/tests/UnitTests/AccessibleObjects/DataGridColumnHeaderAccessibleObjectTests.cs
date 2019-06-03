// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridColumnHeaderAccessibleObjectTests : DataGridColumnStyle
    {
        [Fact]
        public void Ctor_Default()
        {
            var accessibleObject = new DataGridColumnHeaderAccessibleObject();
            Assert.Equal(AccessibleRole.ColumnHeader, accessibleObject.Role);
        }

        protected internal override void Abort(int rowNum)
        {
            throw new NotImplementedException();
        }

        protected internal override bool Commit(CurrencyManager dataSource, int rowNum)
        {
            throw new NotImplementedException();
        }

        protected internal override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string displayText, bool cellIsVisible)
        {
            throw new NotImplementedException();
        }

        protected internal override Size GetPreferredSize(Graphics g, object value)
        {
            throw new NotImplementedException();
        }

        protected internal override int GetMinimumHeight()
        {
            throw new NotImplementedException();
        }

        protected internal override int GetPreferredHeight(Graphics g, object value)
        {
            throw new NotImplementedException();
        }

        protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum)
        {
            throw new NotImplementedException();
        }

        protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight)
        {
            throw new NotImplementedException();
        }
    }
}
