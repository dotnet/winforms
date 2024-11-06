// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using static System.Windows.Forms.Design.AxImporter;

namespace System.Windows.Forms.Design.Tests;

public class AxImporterTests
{
    [Fact]
    public void Constructor_ShouldThrowNotImplementedException()
    {
        Options options = new();

        Action action = () => new AxImporter(options);

        action.Should().Throw<NotImplementedException>()
            .WithMessage(SR.NotImplementedByDesign);
    }
}
