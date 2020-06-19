// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.Collections;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  An expression table allows a lookup from expression to object and object to expression.  It is similar to the serialization manager's GetName and GetInstance methods, only with rich code expressions.
    /// </summary>
    internal sealed class ExpressionTable
    {
        private Hashtable _expressions;

        private Hashtable Expressions
        {
            get
            {
                if (_expressions is null)
                {
                    _expressions = new Hashtable(new ReferenceComparer());
                }
                return _expressions;
            }
        }

        internal void SetExpression(object value, CodeExpression expression, bool isPreset)
        {
            Expressions[value] = new ExpressionInfo(expression, isPreset);
        }

        internal CodeExpression GetExpression(object value)
        {
            CodeExpression expression = null;
            if (Expressions[value] is ExpressionInfo info)
            {
                expression = info.Expression;
            }
            return expression;
        }

        internal bool ContainsPresetExpression(object value)
        {
            if (Expressions[value] is ExpressionInfo info)
            {
                return info.IsPreset;
            }
            else
            {
                return false;
            }
        }

        private class ExpressionInfo
        {
            readonly CodeExpression _expression;
            readonly bool _isPreset;

            internal ExpressionInfo(CodeExpression expression, bool isPreset)
            {
                _expression = expression;
                _isPreset = isPreset;
            }

            internal CodeExpression Expression
            {
                get => _expression;
            }

            internal bool IsPreset
            {
                get => _isPreset;
            }
        }

        private class ReferenceComparer : IEqualityComparer
        {
            bool IEqualityComparer.Equals(object x, object y)
            {
                return object.ReferenceEquals(x, y);
            }

            int IEqualityComparer.GetHashCode(object x)
            {
                if (x != null)
                {
                    return x.GetHashCode();
                }
                return 0;
            }
        }
    }
}
