// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.Collections.Generic;
using Xunit;

namespace System.ComponentModel.Design.Serialization.Tests
{
    public class ExpressionContextTests
    {
        public static IEnumerable<object[]> Ctor_CodeExpression_Type_Object_Object_TestData()
        {
            yield return new object[] { new CodeExpression(), typeof(int), new object(), new object() };
            yield return new object[] { new CodeExpression(), typeof(int), new object(), null };
        }

        [Theory]
        [MemberData(nameof(Ctor_CodeExpression_Type_Object_Object_TestData))]
        public void ExpressionContext_Ctor_CodeExpression_Type_Object_Object_TestData(CodeExpression expression, Type expressionType, object owner, object presetValue)
        {
            var context = new ExpressionContext(expression, expressionType, owner, presetValue);
            Assert.Same(expression, context.Expression);
            Assert.Same(expressionType, context.ExpressionType);
            Assert.Same(owner, context.Owner);
            Assert.Same(presetValue, context.PresetValue);
        }
        public static IEnumerable<object[]> Ctor_CodeExpression_Type_Object_TestData()
        {
            yield return new object[] { new CodeExpression(), typeof(int), new object() };
        }

        [Theory]
        [MemberData(nameof(Ctor_CodeExpression_Type_Object_TestData))]
        public void ExpressionContext_Ctor_CodeExpression_Type_Object_TestData(CodeExpression expression, Type expressionType, object owner)
        {
            var context = new ExpressionContext(expression, expressionType, owner);
            Assert.Same(expression, context.Expression);
            Assert.Same(expressionType, context.ExpressionType);
            Assert.Same(owner, context.Owner);
            Assert.Null(context.PresetValue);
        }

        [Fact]
        public void ExpressionContext_Ctor_NullExpression_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("expression", () => new ExpressionContext(null, typeof(int), new object(), new object()));
        }

        [Fact]
        public void RootContext_Ctor_NullExpressionType_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("expressionType", () => new ExpressionContext(new CodeExpression(), null, new object(), new object()));
        }

        [Fact]
        public void RootContext_Ctor_NullOwner_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("owner", () => new ExpressionContext(new CodeExpression(), typeof(int), null, new object()));
        }
    }
}
