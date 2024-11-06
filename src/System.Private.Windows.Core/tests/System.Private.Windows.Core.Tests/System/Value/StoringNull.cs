// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ValueTests;

public class StoringNull
{
    [Fact]
    public void GetIntFromStoredNull()
    {
        Value nullValue = new((object?)null);
        Assert.Throws<InvalidCastException>(() => _ = nullValue.GetValue<int>());

        Value nullFastValue = new((object?)null);
        Assert.Throws<InvalidCastException>(() => _ = nullFastValue.GetValue<int>());

        bool success = nullFastValue.TryGetValue(out int result);
        Assert.False(success);

        Assert.Equal(default, result);
    }
}
