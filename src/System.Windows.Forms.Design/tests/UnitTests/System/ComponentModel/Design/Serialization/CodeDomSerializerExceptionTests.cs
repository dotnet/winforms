﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Moq;
using Xunit;

namespace System.Windows.Forms.Design.Serialization.Tests
{
    public class CodeDomSerializerExceptionTests
    {
        public static IEnumerable<object[]> Ctor_String_CodeLinePragma_TestData()
        {
            yield return new object[] { "message", new CodeLinePragma() };
            yield return new object[] { null, null };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_CodeLinePragma_TestData))]
        public void CodeDomSerializerException_Ctor_String_CodeLinePragma(string message, CodeLinePragma linePragma)
        {
            var exception = new CodeDomSerializerException(message, linePragma);
            Assert.NotEmpty(exception.Message);
            Assert.Null(exception.InnerException);
            Assert.Same(linePragma, exception.LinePragma);
        }

        public static IEnumerable<object[]> Ctor_Exception_CodeLinePragma_TestData()
        {
            yield return new object[] { new Exception(), new CodeLinePragma() };
            yield return new object[] { null, null };
        }

        [Theory]
        [MemberData(nameof(Ctor_Exception_CodeLinePragma_TestData))]
        public void CodeDomSerializerException_Ctor_Exception_CodeLinePragma(Exception innerException, CodeLinePragma linePragma)
        {
            var exception = new CodeDomSerializerException(innerException, linePragma);
            Assert.NotEmpty(exception.Message);
            Assert.Same(innerException, exception.InnerException);
            Assert.Same(linePragma, exception.LinePragma);
        }

        public static IEnumerable<object[]> Ctor_String_IDesignerSerializationManager_TestData()
        {
            var mockDesignerSerializationManager = new Mock<IDesignerSerializationManager>(MockBehavior.Strict);
            yield return new object[] { "message", mockDesignerSerializationManager.Object };
            yield return new object[] { null, mockDesignerSerializationManager.Object };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_IDesignerSerializationManager_TestData))]
        public void CodeDomSerializerException_Ctor_String_IDesignerSerializationManager(string message, IDesignerSerializationManager manager)
        {
            var exception = new CodeDomSerializerException(message, manager);
            Assert.NotEmpty(exception.Message);
            Assert.Null(exception.InnerException);
            Assert.Null(exception.LinePragma);
        }

        public static IEnumerable<object[]> Ctor_Exception_IDesignerSerializationManager_TestData()
        {
            var mockDesignerSerializationManager = new Mock<IDesignerSerializationManager>(MockBehavior.Strict);
            yield return new object[] { new Exception(), mockDesignerSerializationManager.Object };
            yield return new object[] { null, mockDesignerSerializationManager.Object };
        }

        [Theory]
        [MemberData(nameof(Ctor_Exception_IDesignerSerializationManager_TestData))]
        public void CodeDomSerializerException_Ctor_Exception_IDesignerSerializationManager(Exception innerException, IDesignerSerializationManager manager)
        {
            var exception = new CodeDomSerializerException(innerException, manager);
            Assert.NotEmpty(exception.Message);
            Assert.Same(innerException, exception.InnerException);
            Assert.Null(exception.LinePragma);
        }

        [Fact]
        public void CodeDomSerializerException_NullManager_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("manager", () => new CodeDomSerializerException("message", (IDesignerSerializationManager)null));
            Assert.Throws<ArgumentNullException>("manager", () => new CodeDomSerializerException(new Exception(), (IDesignerSerializationManager)null));
        }

        [Fact]
        public void CodeDomSerializerException_SerializeWithoutPragma_Deserialize_Success()
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                var exception = new CodeDomSerializerException("message", (CodeLinePragma)null);
                formatter.Serialize(stream, exception);

                stream.Position = 0;
                CodeDomSerializerException deserialized = Assert.IsType<CodeDomSerializerException>(formatter.Deserialize(stream));
                Assert.Equal(exception.Message, deserialized.Message);
                Assert.Null(deserialized.LinePragma);
            }
        }

        [Fact]
        public void CodeDomSerializerException_SerializeWithPragma_Deserialize_Success()
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                var exception = new CodeDomSerializerException("message", new CodeLinePragma());
                Assert.Throws<SerializationException>(() => formatter.Serialize(stream, exception));
            }
        }

        [Fact]
        public void CodeDomSerializerException_GetObjectData_NullInfo_ThrowsArgumentNullException()
        {
            var exception = new CodeDomSerializerException("message", new CodeLinePragma());
            Assert.Throws<ArgumentNullException>("info", () => exception.GetObjectData(null, new StreamingContext()));
        }
    }
}
