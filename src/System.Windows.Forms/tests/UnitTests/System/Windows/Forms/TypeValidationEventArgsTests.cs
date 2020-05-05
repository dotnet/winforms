// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class TypeValidationEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        [Theory]
        [InlineData(typeof(int), true, "returnValue", "message")]
        [InlineData(typeof(int), true, "returnValue", "")]
        [InlineData(null, false, null, null)]
        public void Ctor_Type_Object_Object_String(Type validatingType, bool isValidInput, object returnValue, string message)
        {
            var e = new TypeValidationEventArgs(validatingType, isValidInput, returnValue, message);
            Assert.Equal(validatingType, e.ValidatingType);
            Assert.Equal(isValidInput, e.IsValidInput);
            Assert.Equal(returnValue, e.ReturnValue);
            Assert.Equal(message, e.Message);
            Assert.False(e.Cancel);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Cancel_Set_GetReturnsExpected(bool value)
        {
            var e = new TypeValidationEventArgs(typeof(int), true, "returnValue", "message")
            {
                Cancel = value
            };
            Assert.Equal(value, e.Cancel);
        }
    }
}
