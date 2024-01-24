// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Core;

public class PrivateCoreTests
{
    [Fact]
    public void PrivateCore_HasNoPublicTypes()
    {
        // There should be no public types in this assembly.
        typeof(BufferScope<>).Assembly.GetExportedTypes().Should().BeEmpty();
    }
}
