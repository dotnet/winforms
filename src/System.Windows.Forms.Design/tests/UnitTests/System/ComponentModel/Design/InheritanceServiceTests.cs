// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class InheritanceServicerTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void InheritanceService_Constructor()
        {
            var underTest = new InheritanceService();
            Assert.NotNull(underTest);
        }
    }
}
