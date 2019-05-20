// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design.Serialization;
using Xunit;

namespace System.Windows.Forms.Design.Serialization.Tests
{
    public class CodeDomSerializerTests
    {
        [Fact]
        public void CodeDomSerializer_Constructor()
        {
            var underTest = new CodeDomSerializer();
            Assert.NotNull(underTest);
        }
    }
}
