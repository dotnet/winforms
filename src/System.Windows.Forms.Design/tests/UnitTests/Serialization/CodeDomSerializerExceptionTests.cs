// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.ComponentModel.Design.Serialization;
using Moq;
using Xunit;

namespace System.Windows.Forms.Design.Serialization.Tests
{
    public class CodeDomSerializerExceptionTests
    {
        Mock<IDesignerSerializationManager> _mockDesignerSerializationManager;
        
        [Fact]
        public void CodeDomSerializerException_Constructor_Message_Pragma()
        {
            var pragma = new CodeLinePragma();
            var underTest = new CodeDomSerializerException("message", pragma);
            Assert.NotNull(underTest);
            Assert.Equal(pragma, underTest.LinePragma);
        }

        [Fact]
        public void CodeDomSerializerException_Constructor_Exception_Pragma()
        {
            var ex = new Exception();
            var pragma = new CodeLinePragma();
            var underTest = new CodeDomSerializerException(ex, pragma);
            Assert.NotNull(underTest);
            Assert.Equal(pragma, underTest.LinePragma);
        }

        [Fact]
        public void CodeDomSerializerException_Constructor_SerializationManager()
        {
            _mockDesignerSerializationManager = new Mock<IDesignerSerializationManager>(MockBehavior.Strict);
            var underTest = new CodeDomSerializerException("message", _mockDesignerSerializationManager.Object);
            Assert.NotNull(underTest);
        }

        [Fact]
        public void CodeDomSerializerException_Constructor_Exception_SerializationManager()
        {
            var ex = new Exception();
            _mockDesignerSerializationManager = new Mock<IDesignerSerializationManager>(MockBehavior.Strict);
            var underTest = new CodeDomSerializerException(ex, _mockDesignerSerializationManager.Object);
            Assert.NotNull(underTest);
        }
    }
}
