// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class DesignerActionUIStateChangeEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_Object_DesignerActionUIStateChangeType_TestData()
        {
            yield return new object[] { null, DesignerActionUIStateChangeType.Show - 1 };
            yield return new object[] { new object(), DesignerActionUIStateChangeType.Show };
        }

        [Theory]
        [MemberData(nameof(Ctor_Object_DesignerActionUIStateChangeType_TestData))]
        public void Ctor_Object_DesignerActionUIStateChangeType(object relatedObject, DesignerActionUIStateChangeType changeType)
        {
            var e = new DesignerActionUIStateChangeEventArgs(relatedObject, changeType);
            Assert.Same(relatedObject, e.RelatedObject);
            Assert.Equal(changeType, e.ChangeType);
        }
    }
}
