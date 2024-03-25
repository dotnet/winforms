// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

namespace System.Windows.Forms.Design.Tests.Serialization;

public class SerializableAttributeTests
{
    [Fact]
    public void EnsureSerializableAttribute()
    {
        BinarySerialization.EnsureSerializableAttribute(
            typeof(Behavior.Behavior).Assembly,
            new HashSet<string>
            {
                typeof(OleDragDropHandler.CfCodeToolboxItem).FullName,
                typeof(ExceptionCollection).FullName,
                typeof(CodeDomSerializerException).FullName,
                typeof(CodeDomComponentSerializationService.AssemblyNameInfo).FullName,
                typeof(CodeDomComponentSerializationService.CodeDomComponentSerializationState).FullName,
                { "System.ComponentModel.Design.Serialization.CodeDomComponentSerializationService+CodeDomSerializationStore" },
                { "System.Windows.Forms.Design.Behavior.DesignerActionKeyboardBehavior+<>c" }
            });
    }
}
