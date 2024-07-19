// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization.Formatters;

namespace FormatTests.Common;

public abstract class PrimitiveTests<T> : SerializationTest<T> where T : ISerializer
{
    [Fact]
    public void TypesWhenNeeded_Integer()
    {
        int value = 42;
        Stream stream = Serialize(value, typeStyle: FormatterTypeStyle.TypesWhenNeeded);
        object deserialized = Deserialize(stream);
        deserialized.Should().Be(42);
    }
}
