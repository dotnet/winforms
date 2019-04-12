// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.CodeDom.Compiler;
using Moq;
using Xunit;

namespace System.ComponentModel.Design.Serialization.Tests
{
    public class TypeCodeDomSerializerTests
    {
        [Fact]
        public void TypeCodeDomSerializer_Constructor()
        {
            var underTest = new TypeCodeDomSerializer();
            Assert.NotNull(underTest);
        }

        [Fact]
        public void TypeCodeDomSerializer_Serialize_Manager_Null ()
        {
            var underTest = new TypeCodeDomSerializer();
            Assert.Throws<ArgumentNullException>(() => underTest.Serialize(null, null, null));
        }

        [Fact]
        public void TypeCodeDomSerializer_Serialize_Root_Null()
        {
            var mockSerializationManager = new Mock<DesignerSerializationManager>(MockBehavior.Strict);
            var underTest = new TypeCodeDomSerializer();
            Assert.Throws<ArgumentNullException>(() => underTest.Serialize(mockSerializationManager.Object, null, null));
        }

        [Fact]
        public void TypeCodeDomSerializer_Serialize()
        {
            var manager = new Mock<IDesignerSerializationManager>(MockBehavior.Loose);
            manager.Setup(m => m.GetName(typeof(object))).Returns("string");
            manager.Setup(m => m.Context).Returns(new ContextStack());
            var root = new object();
            var underTest = new TypeCodeDomSerializer();
#if DEBUG
            Assert.NotNull(underTest.Serialize(manager.Object, root, null));
#else
            Assert.Throws<InvalidOperationException>(() => underTest.Deserialize(manager.Object, declaration.Object));
#endif
        }

        [Fact]
        public void TypeCodeDomSerializer_Deserialize_Manager_Null()
        {
            var underTest = new TypeCodeDomSerializer();
            Assert.Throws<ArgumentNullException>(() => underTest.Deserialize(null, null));
        }

        [Fact]
        public void TypeCodeDomSerializer_Deserialize_CodeTypeDec_Null()
        {
            var mockSerializationManager = new Mock<DesignerSerializationManager>(MockBehavior.Strict);
            var underTest = new TypeCodeDomSerializer();
            Assert.Throws<ArgumentNullException>(() => underTest.Deserialize(mockSerializationManager.Object, null));
        }

        [Fact]
        public void TypeCodeDomSerializer_Deserialize_SerializeEx()
        {
            var manager = new Mock<IDesignerSerializationManager>(MockBehavior.Strict);
            manager.Setup(m => m.GetName(typeof(object))).Returns("string");
            manager.Setup(m => m.Context).Returns(new ContextStack());
            var codeDomProvider = new Mock<CodeDomProvider>(MockBehavior.Strict);
            codeDomProvider.Setup(cdp => cdp.LanguageOptions).Returns(new LanguageOptions());
            manager.Setup(m => m.GetService(typeof(CodeDomProvider))).Returns(codeDomProvider.Object);
            var declaration = new Mock<CodeTypeDeclaration>(MockBehavior.Strict);
            var underTest = new TypeCodeDomSerializer();
#if DEBUG
            Assert.Throws<CodeDomSerializerException>(() => underTest.Deserialize(manager.Object, declaration.Object));
#else
            Assert.Throws<InvalidOperationException>(() => underTest.Deserialize(manager.Object, declaration.Object));
#endif
        }
    }
}
