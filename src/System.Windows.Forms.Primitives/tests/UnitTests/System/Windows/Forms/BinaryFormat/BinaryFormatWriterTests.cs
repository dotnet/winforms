// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization.Formatters.Binary;

namespace System.Windows.Forms.BinaryFormat.Tests;

public class BinaryFormatWriterTests
{
    [Theory]
    [InlineData("Hello World.")]
    [InlineData("")]
    [InlineData("\0")]
    [InlineData("Embedded\0 Null.")]
    public void WriteString(string testString)
    {
        using MemoryStream stream = new();
        BinaryFormatWriter.WriteString(stream, testString);

        stream.Position = 0;
        using var formatterScope = new BinaryFormatterScope(enable: true);
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        BinaryFormatter formatter = new();
#pragma warning restore
        object deserialized = formatter.Deserialize(stream);
        deserialized.Should().Be(testString);
    }
}
