﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  An expression context is an object that is placed on the context stack and contains the most relevant expression during serialization.  For example, take the following statement:
    ///  button1.Text = "Hello";
    ///  During serialization several serializers will be responsible for creating this single statement.  One of those serializers will be responsible for writing "Hello".  There are times when that serializer may need to know the context in which it is creating its expression.  In the above example, this isn't needed, but take this slightly modified example:
    ///  button1.Text = rm.GetString("button1_Text");
    ///  Here, the serializer responsible for writing the resource expression needs to know the names of the target objects.  The ExpressionContext class can be used for this.  As each serializer creates an expression and invokes a serializer to handle a smaller part of the statement as a whole, the serializer pushes an expression context on the context stack.  Each expression context has a parent property that locates the next expression context on the stack, which provides a way for easy traversal.
    /// </summary>
    public sealed class ExpressionContext
    {
        public ExpressionContext(CodeExpression expression, Type expressionType, object owner, object presetValue)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            ExpressionType = expressionType ?? throw new ArgumentNullException(nameof(expressionType));
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            PresetValue = presetValue;
        }

        public ExpressionContext(CodeExpression expression, Type expressionType, object owner) : this(expression, expressionType, owner, null)
        {
        }

        /// <summary>
        ///  The expression this context represents.
        /// </summary>
        public CodeExpression Expression { get; }

        /// <summary>
        ///  The type of the expression. This can be used to determine if a
        ///  cast is needed when assigning  to the expression.
        /// </summary>
        public Type ExpressionType { get; }

        /// <summary>
        ///  The object owning this expression. For example, if the expression
        ///  was a property reference to button1's Text property, Owner would
        ///  return button1.
        /// </summary>
        public object Owner { get; }

        /// <summary>
        ///  Contains the preset value of an expression, should one exist.
        ///  For example, if the expression is a property reference expression
        ///  referring to the Controls property of a button, PresetValue will
        ///  contain the instance of Controls property because the property is
        ///  read-only and preset by the object to contain a value. On the other
        ///  hand, a property such as Text or Visible does not have a preset
        ///  value and therefore the PresetValue property will return null.
        ///  Serializers can use this information to guide serialization.
        ///  For example, take the following two snippets of code:
        ///  Padding p = new Padding();
        ///  p.Left = 5;
        ///  button1.Padding = p;
        ///  button1.Padding.Left = 5;
        ///  The serializer of the Padding class needs to know if it should
        ///  generate the first or second form.  The first form would be
        ///  generated by default.  The second form will only be generated
        ///  if there is an ExpressionContext on the stack that contains a
        ///  PresetValue equal to the value of the Padding object currently
        ///  being serialized.
        /// </summary>
        public object PresetValue { get; }
    }
}
