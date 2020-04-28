// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class ConvertEventArgsTests
    {
        [Theory]
        [InlineData("value", typeof(int))]
        [InlineData(null, null)]
        public void Ctor_Object_Type(object value, Type desiredType)
        {
            var e = new ConvertEventArgs(value, desiredType);
            Assert.Equal(value, e.Value);
            Assert.Equal(desiredType, e.DesiredType);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(1)]
        public void Value_Set_GetReturnsExpected(object value)
        {
            var e = new ConvertEventArgs("value", typeof(int))
            {
                Value = value
            };
            Assert.Equal(value, e.Value);
        }
    }
}
