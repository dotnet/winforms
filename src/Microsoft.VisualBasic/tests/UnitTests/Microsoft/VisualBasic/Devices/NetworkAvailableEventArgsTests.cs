// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.VisualBasic.Devices.Tests;

public class NetworkAvailableEventArgsTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Ctor_Bool(bool networkAvailable)
    {
        NetworkAvailableEventArgs args = new(networkAvailable);
        Assert.Equal(networkAvailable, args.IsNetworkAvailable);
    }
}
