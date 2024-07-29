// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace FormatTests.Common;

public abstract class SerializationTest
{
    private protected static Stream Serialize(
        object value,
        SerializationBinder? binder = null,
        ISurrogateSelector? surrogateSelector = null,
        FormatterTypeStyle typeStyle = FormatterTypeStyle.TypesAlways)
    {
        MemoryStream stream = new();
        BinaryFormatter formatter = new()
        {
            SurrogateSelector = surrogateSelector,
            TypeFormat = typeStyle,
            Binder = binder
        };

        formatter.Serialize(stream, value);
        stream.Position = 0;
        return stream;
    }
}
