// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.ComponentModel.Design.Serialization;
using Xunit;

namespace System.Windows.Forms.Design.Serialization.Tests
{
    public class ExpressionContextTests
    {
        [Fact]
        public void ExpressionContext_Constructor()
        {
            CodeExpression expression = new CodeExpression();
            Type type = expression.GetType();
            object owner = null;
            object presetValue = null;

            var underTest = new ExpressionContext(expression, type, owner, presetValue);
            Assert.NotNull(underTest);
            Assert.Equal(expression, underTest.Expression);
            Assert.Equal(expression.GetType(), underTest.ExpressionType);
            Assert.Equal(type, underTest.GetType());
        }
    }
}
