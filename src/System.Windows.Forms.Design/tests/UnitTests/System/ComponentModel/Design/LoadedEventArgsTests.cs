// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;

namespace System.ComponentModel.Design.Tests;

public class LoadedEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Bool_ICollection_TestData()
    {
        yield return new object[] { true, null };
        yield return new object[] { false, Array.Empty<object>() };
        yield return new object[] { true, new object[] { null } };
    }

    [Theory]
    [MemberData(nameof(Ctor_Bool_ICollection_TestData))]
    public void Ctor_Bool_ICollection(bool succeeded, ICollection errors)
    {
        LoadedEventArgs e = new(succeeded, errors);
        Assert.Equal(succeeded, e.HasSucceeded);
        if (errors is null)
        {
            Assert.Empty(e.Errors);
        }
        else
        {
            Assert.Same(errors, e.Errors);
        }
    }
}
