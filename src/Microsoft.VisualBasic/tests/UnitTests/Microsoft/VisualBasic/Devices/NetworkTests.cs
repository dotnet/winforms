// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace Microsoft.VisualBasic.Devices.Tests
{
    public class NetworkTests
    {
        [Fact]
        public void IsAvailable()
        {
            var network = new Network();
            Assert.Equal(System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable(), network.IsAvailable);
        }

        [Fact]
        public void Ping()
        {
            var network = new Network();
            Assert.True(network.Ping("127.0.0.1"));
        }

        // Not tested:
        //    Public Sub DownloadFile(...) [multiple overloads]
        //    Public Sub UploadFile(...) [multiple overloads]
    }
}
