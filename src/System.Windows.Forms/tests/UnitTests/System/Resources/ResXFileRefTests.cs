// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text;

namespace System.Resources.Tests;

// NB: doesn't require thread affinity
public class ResXFileRefTests
{
    [Fact]
    public void ResXFileRef_Constructor()
    {
        var fileName = "SomeFile";
        var typeName = "SomeType";

        var fileRef = new ResXFileRef(fileName, typeName);

        Assert.Equal(fileName, fileRef.FileName);
        Assert.Equal(typeName, fileRef.TypeName);
        Assert.Null(fileRef.TextFileEncoding);
    }

    [Fact]
    public void ResXFileRef_EncodingConstructor()
    {
        var fileName = "SomeFile";
        var typeName = "SomeType";
        Encoding encoding = Encoding.Default;

        var fileRef = new ResXFileRef(fileName, typeName, encoding);

        Assert.Equal(fileName, fileRef.FileName);
        Assert.Equal(typeName, fileRef.TypeName);
        Assert.Equal(encoding, fileRef.TextFileEncoding);
    }
}
