// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design.Serialization;

public partial class TypeCodeDomSerializer
{
    private class StatementOrderComparer : IComparer<OrderedCodeStatementCollection>
    {
        public static readonly StatementOrderComparer s_default = new();

        private StatementOrderComparer()
        {
        }

        public int Compare(OrderedCodeStatementCollection? left, OrderedCodeStatementCollection? right)
        {
            if (left is null)
            {
                return 1;
            }
            else if (right is null)
            {
                return -1;
            }
            else if (right == left)
            {
                return 0;
            }

            return left.Order - right.Order;
        }
    }
}
