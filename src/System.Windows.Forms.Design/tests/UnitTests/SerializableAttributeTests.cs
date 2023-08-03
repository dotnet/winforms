// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
                { "System.Windows.Forms.Design.Behavior.DesignerActionKeyboardBehavior+<>c" }
            });
    }
}
