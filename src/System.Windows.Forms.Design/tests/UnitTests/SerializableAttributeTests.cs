// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Design.Tests.Serialization
{
    public class SerializableAttributeTests
    {
        [Fact]
        public void EnsureSerializableAttribute()
        {
            BinarySerialization.EnsureSerializableAttribute(
                typeof(Behavior.Behavior).Assembly, 
                new List<string>
                {
                    { "System.Windows.Forms.Design.Behavior.DesignerActionKeyboardBehavior+<>c"}
                });
        }
    }
}
