// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class TypeValidationEventArgsTests
{
    [Theory]
    [InlineData(typeof(int), true, "returnValue", "message")]
    [InlineData(typeof(int), true, "returnValue", "")]
    [InlineData(null, false, null, null)]
    public void Ctor_Type_Object_Object_String(Type validatingType, bool isValidInput, object returnValue, string message)
    {
        TypeValidationEventArgs e = new(validatingType, isValidInput, returnValue, message);
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
        TypeValidationEventArgs e = new(typeof(int), true, "returnValue", "message")
        {
            Cancel = value
        };
        Assert.Equal(value, e.Cancel);
    }
}
