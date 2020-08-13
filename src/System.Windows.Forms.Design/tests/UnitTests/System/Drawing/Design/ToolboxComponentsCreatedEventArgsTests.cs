// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using Xunit;

namespace System.Drawing.Design.Tests
{
    public class ToolboxComponentsCreatedEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_IComponentArray_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { Array.Empty<IComponent>() };
            yield return new object[] { new IComponent[] { null } };
        }

        [Theory]
        [MemberData(nameof(Ctor_IComponentArray_TestData))]
        public void Ctor_IComponentArray(IComponent[] components)
        {
            var e = new ToolboxComponentsCreatedEventArgs(components);
            if (components is null)
            {
                Assert.Null(e.Components);
            }
            else
            {
                Assert.Equal(components, e.Components);
                Assert.NotSame(components, e.Components);
                Assert.Equal(e.Components, e.Components);
                Assert.NotSame(e.Components, e.Components);
            }
        }
    }
}
