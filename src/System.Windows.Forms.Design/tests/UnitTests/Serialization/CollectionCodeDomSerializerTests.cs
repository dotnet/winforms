// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel.Design.Serialization;

namespace System.Windows.Forms.Design.Serialization.Tests;

public class CollectionCodeDomSerializerTests
{
    [Fact]
    public void CollectionCodeDomSerializer_Constructor()
    {
        CollectionCodeDomSerializer underTest = CollectionCodeDomSerializer.Default;
        Assert.NotNull(underTest);
    }
}
