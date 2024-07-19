// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FormatTests.Common;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace FormatTests.Formatter;

public class BinaryFormatterSerializer : ISerializer
{
    private static readonly StreamingContext s_context = new(StreamingContextStates.All, null);

    public static object Deserialize(
        Stream stream,
        SerializationBinder? binder,
        FormatterAssemblyStyle assemblyMatching = FormatterAssemblyStyle.Simple,
        ISurrogateSelector? surrogateSelector = null)
    {
        BinaryFormatter formatter = new(surrogateSelector, s_context)
        {
            Binder = binder,
            AssemblyFormat = assemblyMatching
        };

        return formatter.Deserialize(stream);
    }
}
