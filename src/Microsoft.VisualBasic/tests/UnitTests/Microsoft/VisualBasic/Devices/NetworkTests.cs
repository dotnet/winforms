// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.VisualBasic.Devices.Tests;

public class NetworkTests
{
    [Fact]
    public void IsAvailable()
    {
        Network network = new();
        Assert.Equal(System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable(), network.IsAvailable);
    }

    [Fact]
    public void Ping_ShortTimeout_Success()
    {
        Network network = new();
        Assert.True(network.Ping("127.0.0.1", 1));
    }

    [Fact]
    public void Ping_Success()
    {
        Network network = new();
        Assert.True(network.Ping("127.0.0.1"));
    }

    [Fact]
    public void Ping_Throw()
    {
        Network network = new();
        Assert.Throws<ArgumentNullException>(() => network.Ping((string)null));
    }

    [Fact]
    public void PingUri_ShortTimeout_Success()
    {
        Network network = new();
        Assert.True(network.Ping(new Uri("http://127.0.0.1"), 1));
    }

    [Fact]
    public void PingUri_Success()
    {
        Network network = new();
        Assert.True(network.Ping(new Uri("http://127.0.0.1")));
    }

    [Fact]
    public void PingUri_Throw()
    {
        Network network = new();
        Assert.Throws<ArgumentNullException>(() => network.Ping((Uri)null));
    }

    [Fact]
    public void PingUriTimeout_Throw()
    {
        Network network = new();
        Assert.Throws<ArgumentNullException>(() => network.Ping((Uri)null, 1));
    }

    // Not tested:
    //    Public Sub UploadFile(...) [multiple overloads]
}
