// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class BindingManagerDataErrorEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Exception_TestData()
    {
        yield return new object[] { new InvalidOperationException() };
        yield return new object[] { null };
    }

    [Theory]
    [MemberData(nameof(Ctor_Exception_TestData))]
    public void Ctor_Exception(Exception exception)
    {
        BindingManagerDataErrorEventArgs e = new(exception);
        Assert.Equal(exception, e.Exception);
    }
}
