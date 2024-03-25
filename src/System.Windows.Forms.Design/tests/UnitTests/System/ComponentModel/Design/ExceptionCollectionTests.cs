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
            collection.Exceptions.Should().BeNull();
        }
        else
        {
            collection.Exceptions.Should().BeEquivalentTo(exceptions);
            collection.Exceptions.Should().NotBeSameAs(exceptions);
            collection.Exceptions.Should().BeEquivalentTo(collection.Exceptions);
            collection.Exceptions.Should().NotBeSameAs(collection.Exceptions);
        }
    }

    [Fact]
    public void ExceptionCollection_Ctor_ArguementException()
    {
        ArrayList exceptions = [1, 2, 3];
        Action act = () => new ExceptionCollection(exceptions);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ExceptionCollection_Serialize_ThrowsNotSupportedException()
    {
        using BinaryFormatterScope formatterScope = new(enable: false);
        using MemoryStream stream = new();
        BinaryFormatter formatter = new();
        ExceptionCollection collection = new(new ArrayList());
        Action act = () => formatter.Serialize(stream, collection);
        act.Should().Throw<NotSupportedException>();
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
            deserialized.Exceptions.Should().BeNull();
        }
        else
        {
            for(int i = 0; i < exceptions.Count; i++)
            {
                ((Exception)deserialized.Exceptions[i]).Message.Should().Be(((Exception)exceptions[i]).Message);
            }

            deserialized.Exceptions.Should().NotBeSameAs(exceptions);
            deserialized.Exceptions.Should().BeEquivalentTo(deserialized.Exceptions);
            deserialized.Exceptions.Should().NotBeSameAs(deserialized.Exceptions);
        }
    }

    [Fact]
    public void ExceptionCollection_GetObjectData_ThrowsPlatformNotSupportedException()
    {
        ExceptionCollection collection = new(new ArrayList());
        Action act = () => collection.GetObjectData(null, new StreamingContext());
        act.Should().Throw<ArgumentNullException>();
    }
}
