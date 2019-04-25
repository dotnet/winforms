// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridColumnStyleTests
    {
        [Fact]
        public void DataGridColumnStyle_DefaultProperty_Get_ReturnsExpected()
        {
            PropertyDescriptor property = TypeDescriptor.GetDefaultProperty(typeof(DataGridColumnStyle));
            Assert.Equal("HeaderText", property.Name);
        }
    }
}
