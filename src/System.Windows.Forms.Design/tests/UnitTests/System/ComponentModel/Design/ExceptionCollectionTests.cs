// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.ComponentModel.Design.Tests;

public class ExceptionCollectionTests
{
    public static IEnumerable<object[]> Ctor_ArrayList_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ArrayList() };
        yield return new object[] { new ArrayList { new InvalidOperationException(), new InvalidOperationException(), new InvalidOperationException()} };
    }

    [Theory]
    [MemberData(nameof(Ctor_ArrayList_TestData))]
    public void ExceptionCollection_Ctor_ArrayList(ArrayList exceptions)
    {
        ExceptionCollection collection = new(exceptions);
        if (exceptions is null)
        {
            Assert.Null(collection.Exceptions);
        }
        else
        {
            Assert.Equal(exceptions, collection.Exceptions);
            Assert.NotSame(exceptions, collection.Exceptions);
            Assert.Equal(collection.Exceptions, collection.Exceptions);
            Assert.NotSame(collection.Exceptions, collection.Exceptions);
        }
    }

    [Fact]
    public void ExceptionCollection_Ctor_ArguementException()
    {
        ArrayList exceptions = [1, 2, 3];
        Assert.Throws<ArgumentException>(() => new ExceptionCollection(exceptions));
    }

    [Fact]
    public void ExceptionCollection_Serialize_ThrowsNotSupportedException()
    {
        using BinaryFormatterScope formatterScope = new(enable: false);
        using MemoryStream stream = new();
        BinaryFormatter formatter = new();
        ExceptionCollection collection = new(new ArrayList());
        Assert.Throws<NotSupportedException>(() => formatter.Serialize(stream, collection));
    }

    [Theory]
    [MemberData(nameof(Ctor_ArrayList_TestData))]
    public void ExceptionCollection_Serialize_Deserialize_Success(ArrayList exceptions)
    {
        using BinaryFormatterScope formatterScope = new(enable: true);
        using MemoryStream stream = new();
        BinaryFormatter formatter = new();
        var collection = new ExceptionCollection(exceptions);
        formatter.Serialize(stream, collection);

        stream.Position = 0;
        ExceptionCollection deserialized = Assert.IsType<ExceptionCollection>(formatter.Deserialize(stream));
        if (exceptions is null)
        {
            Assert.Null(deserialized.Exceptions);
        }
        else
        {
            for(int i = 0; i < exceptions.Count; i++)
            {
                Assert.Equal(((Exception)exceptions[i]).Message, ((Exception)deserialized.Exceptions[i]).Message);
            }

            Assert.NotSame(exceptions, deserialized.Exceptions);
            Assert.Equal(deserialized.Exceptions, deserialized.Exceptions);
            Assert.NotSame(deserialized.Exceptions, deserialized.Exceptions);
        }
    }

    [Fact]
    public void ExceptionCollection_GetObjectData_ThrowsPlatformNotSupportedException()
    {
        ExceptionCollection collection = new(new ArrayList());
        Assert.Throws<ArgumentNullException>(() => collection.GetObjectData(null, new StreamingContext()));
    }
}
