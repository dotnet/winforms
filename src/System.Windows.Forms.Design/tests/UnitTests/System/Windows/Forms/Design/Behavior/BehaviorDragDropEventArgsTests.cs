// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Design.Behavior.Tests
{
    public class BehaviorDragDropEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_ICollection_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { Array.Empty<object>() };
            yield return new object[] { new object[] { null } };
        }

        [Theory]
        [MemberData(nameof(Ctor_ICollection_TestData))]
        public void Ctor_ICollection(ICollection components)
        {
            var e = new BehaviorDragDropEventArgs(components);
            Assert.Same(components, e.DragComponents);
        }
    }
}
