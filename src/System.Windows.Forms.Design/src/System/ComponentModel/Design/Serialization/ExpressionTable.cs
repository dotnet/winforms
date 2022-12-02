// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  An expression table allows a lookup from expression to object and object to expression.  It is similar to the serialization manager's GetName and GetInstance methods, only with rich code expressions.
    /// </summary>
    internal sealed class ExpressionTable
    {
        private Dictionary<object, ExpressionInfo> _expressions;

        private Dictionary<object, ExpressionInfo> Expressions => _expressions ??= new(new ReferenceComparer());

        internal void SetExpression(object value, CodeExpression expression, bool isPreset)
        {
            Expressions[value] = new ExpressionInfo(expression, isPreset);
        }

        internal CodeExpression GetExpression(object value)
            => Expressions.TryGetValue(value, out ExpressionInfo info) ? info.Expression : null;

        internal bool ContainsPresetExpression(object value)
            => Expressions.TryGetValue(value, out ExpressionInfo info) && info.IsPreset;

        private class ExpressionInfo
        {
            internal ExpressionInfo(CodeExpression expression, bool isPreset)
            {
                Expression = expression;
                IsPreset = isPreset;
            }

            internal CodeExpression Expression { get; }

            internal bool IsPreset { get; }
        }

        private class ReferenceComparer : IEqualityComparer<object>
        {
            bool IEqualityComparer<object>.Equals(object x, object y)
                => ReferenceEquals(x, y);

            int IEqualityComparer<object>.GetHashCode(object x)
                => x is not null ? x.GetHashCode() : 0;
        }
    }
}
