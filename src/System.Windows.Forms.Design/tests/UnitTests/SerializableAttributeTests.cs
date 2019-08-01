// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing.Design;
using Xunit;

namespace System.Windows.Forms.Design.Tests.Serialization
{
    public class SerializableAttributeTests
    {
        [Fact]
        public void EnsureSerializableAttribute()
        {
            BinarySerialization.EnsureSerializableAttribute(
                typeof(ToolboxItem).Assembly, 
                new List<string>
                {
                    // Serialization store is binary serialized/deserialized during copy/paste scenarion in winforms designer
                    { "System.ComponentModel.Design.Serialization.CodeDomComponentSerializationService+CodeDomSerializationStore"}, // This type is private.
                    { "System.Windows.Forms.Design.Behavior.DesignerActionKeyboardBehavior+<>c"}
                });
        }
    }
}
