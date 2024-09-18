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

    /// <summary>
    ///  The ping API exposed by <see cref="Network"/> is in units of Milliseconds, Pinging
    ///  local server takes approximately 1 Millisecond so there is no reliable way
    ///  to test a short timeout that for a timeout exception.
    /// </summary>
    [Fact]
    public void Ping_LongTimeout_Success()
    {
        Network network = new();
        network.Ping("127.0.0.1", 100).Should().BeTrue();
    }

    [Fact]
    public void Ping_Success()
    {
        Network network = new();
        network.Ping("127.0.0.1").Should().BeTrue();
    }

    [Fact]
    public void Ping_Throw()
    {
        Network network = new();
        Assert.Throws<ArgumentNullException>(() => network.Ping((string)null));
    }

    [Fact]
    public void PingUri_Success()
    {
        Network network = new();
        network.Ping(new Uri("http://127.0.0.1")).Should().BeTrue();
    }

    [Fact]
    public void PingUri_Throw()
    {
        Network network = new();
        Action action = () => network.Ping((Uri)null);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void PingUriTimeout_Throw()
    {
        Network network = new();
        Action action = () => network.Ping((Uri)null, 1);
        action.Should().Throw<ArgumentNullException>();
    }

    // Not tested:
    //    Public Sub UploadFile(...) [multiple overloads]
}
