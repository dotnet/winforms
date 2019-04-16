// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design.Serialization;
using Xunit;

namespace System.Windows.Forms.Design.Serialization.Tests
{
    public class CodeDomComponentSerializationServiceTests
    {
        [Fact]
        public void CodeDomComponentSerializationService_Constructor()
        {
            IServiceProvider provider = new DesignerSerializationManager();
            var underTest = new CodeDomComponentSerializationService(provider);
            Assert.NotNull(underTest);
        }
    }
}
