// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace Microsoft.VisualBasic.Devices.Tests
{
    public class PortsTests
    {
        // Not tested:
        //    Public Function OpenSerialPort(...) As SerialPort [multiple overloads]

        [Fact]
        public void SerialPortNames()
        {
            var ports = new Ports();
            Assert.Equal(System.IO.Ports.SerialPort.GetPortNames(), ports.SerialPortNames);
        }
    }
}
