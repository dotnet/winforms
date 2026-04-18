// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.CodeDom;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using Moq;

namespace System.Windows.Forms.Design.Serialization.Tests;

public class CodeDomSerializerTests
{
    [Fact]
    public void CodeDomSerializer_Constructor()
    {
        CodeDomSerializer underTest = new();
        Assert.NotNull(underTest);
    }

    [Fact]
    public void CodeDomSerializer_Deserialize_GenericTypeRef()
    {
        CodeDomSerializer underTest = new();
        Type type = typeof(List<int?>);
        CodeTypeOfExpression expression = new(new CodeTypeReference(type));

        Mock<IDesignerSerializationManager> mockManager = new(MockBehavior.Strict);
        mockManager
            .Setup(s => s.GetService(typeof(TypeDescriptionProviderService)))
            .Returns(null);
        mockManager
            .Setup(s => s.GetType("System.Int32"))
            .Returns(typeof(int));
        mockManager
            .Setup(s => s.GetType($"System.Collections.Generic.List`1[[System.Nullable`1[[{typeof(int).AssemblyQualifiedName}]]]]"))
            .Returns(type);

        object result = underTest.Deserialize(mockManager.Object, expression);
        Assert.Equal(type, result);
    }
}
