// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using Xunit;

namespace System.ComponentModel.Design.Serialization.Tests
{
    public class RootContextTests
    {
        [Fact]
        public void RootContext_Ctor_CodeExpression_Object()
        {
            var expression = new CodeExpression();
            var value = new object();
            var context = new RootContext(expression, value);
            Assert.Same(expression, context.Expression);
            Assert.Same(value, context.Value);
        }

        [Fact]
        public void RootContext_Ctor_NullExpression_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("expression", () => new RootContext(null, new object()));
        }

        [Fact]
        public void RootContext_Ctor_NullValue_ThrowsArgumentNullException()
        {
            var expression = new CodeExpression();
            Assert.Throws<ArgumentNullException>("value", () => new RootContext(expression, null));
        }
    }
}
