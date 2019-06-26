// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class ExceptionCollectionTests
    {
        public static IEnumerable<object[]> Ctor_ValidList_TestData()
        {
            yield return new object[] { new List<Exception>() };
            yield return new object[] { new List<Exception> { new Exception(), new ArgumentException(), new SystemException() } };
        }

        [Theory]
        [MemberData(nameof(Ctor_ValidList_TestData))]
        public void ExceptionCollection_Ctor_List(List<Exception> exceptions)
        {
            var collection = new ExceptionCollection(exceptions);
            if (exceptions == null)
            {
                Assert.NotNull(collection.Exceptions);
            }
            else
            {
                Assert.Equal(exceptions, collection.Exceptions);
                Assert.Same(exceptions, collection.Exceptions);
                Assert.Equal(collection.Exceptions, collection.Exceptions);
            }
        }

        [Fact]
        public void ExceptionCollection_Ctor_NullThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionCollection(null));
        }

        [Theory]
        [MemberData(nameof(Ctor_ValidList_TestData))]
        public void ExceptionCollection_Serialize_Deserialize_Success(List<Exception> exceptions)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                var collection = new ExceptionCollection(exceptions);
                formatter.Serialize(stream, collection);

                stream.Position = 0;
                ExceptionCollection deserialized = Assert.IsType<ExceptionCollection>(formatter.Deserialize(stream));
                Assert.Equal(exceptions.ToString(), deserialized.Exceptions.ToString());
            }
        }

        [Fact]
        public void ExceptionCollection_GetObjectData_NullInfo_ThrowsArgumentNullException()
        {
            var collection = new ExceptionCollection(new List<Exception>());
            Assert.Throws<ArgumentNullException>("info", () => collection.GetObjectData(null, new StreamingContext()));
        }
    }
}
