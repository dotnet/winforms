// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.VisualBasic.ApplicationServices.Tests;

public class UnhandledExceptionEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Bool_Exception_TestData()
    {
        yield return new object[] { true, null };
        yield return new object[] { false, new InvalidOperationException() };
    }

    [Theory]
    [MemberData(nameof(Ctor_Bool_Exception_TestData))]
    public void Ctor_Bool_Exception(bool exitApplication, Exception exception)
    {
        UnhandledExceptionEventArgs args = new(exitApplication, exception);
        Assert.Same(exception, args.Exception);
        Assert.Equal(exitApplication, args.ExitApplication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ExitApplication_Set_GetReturnsExpected(bool value)
    {
        UnhandledExceptionEventArgs args = new(true, null)
        {
            ExitApplication = value
        };
        Assert.Equal(value, args.ExitApplication);
    }
}
