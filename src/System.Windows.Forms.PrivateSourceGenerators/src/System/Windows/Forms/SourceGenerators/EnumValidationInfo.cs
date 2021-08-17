// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

namespace System.Windows.Forms.PrivateSourceGenerators
{
    internal class EnumValidationInfo
    {
        public ITypeSymbol EnumType { get; }
        public List<int> Values { get; }
        public bool IsFlags { get; }

        public EnumValidationInfo(ITypeSymbol enumType, bool isFlags)
        {
            EnumType = enumType;
            IsFlags = isFlags;
            Values = GetElementValues(enumType).OrderBy(e => e).Distinct().ToList();
        }

        private static IEnumerable<int> GetElementValues(ITypeSymbol enumType)
        {
            foreach (ISymbol member in enumType.GetMembers())
            {
                if (member is IFieldSymbol
                    {
                        IsStatic: true,
                        IsConst: true,
                        ConstantValue: int value
                    })
                {
                    yield return value;
                }
            }
        }
    }
}
