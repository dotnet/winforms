// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListControlConvertEventArgsTests
    {
        [Theory]
        [InlineData("value", typeof(int), 1)]
        [InlineData(null, null, null)]
        public void Ctor_Object_Type(object value, Type desiredType, object listItem)
        {
            var e = new ListControlConvertEventArgs(value, desiredType, listItem);
            Assert.Equal(value, e.Value);
            Assert.Equal(desiredType, e.DesiredType);
            Assert.Equal(listItem, e.ListItem);
        }
    }
}
