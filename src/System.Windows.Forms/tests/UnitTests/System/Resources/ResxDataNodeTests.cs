// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Resources.Tests
{
    // NB: doesn't require thread affinity
    public class ResxDataNodeTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void ResxDataNode_ResXFileRefConstructor()
        {
            var nodeName = "Node";
            var fileRef = new ResXFileRef(string.Empty, string.Empty);
            var dataNode = new ResXDataNode(nodeName, fileRef);

            Assert.Equal(nodeName, dataNode.Name);
            Assert.Same(fileRef, dataNode.FileRef);
        }
    }
}
