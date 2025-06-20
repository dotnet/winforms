// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Controls.Tests;

public class ImageListConverterTests
{
    internal ImageListConverter Converter { get; } = new();

    [Fact]
    public void Ctor_Default_SetsBaseType() =>
        Converter.Should().BeAssignableTo<ComponentConverter>();

    [Fact]
    public void GetPropertiesSupported_ReturnsTrue() =>
        Converter.GetPropertiesSupported(null).Should().BeTrue();
}
