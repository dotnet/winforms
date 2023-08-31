// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;

namespace System.Windows.Forms.PrivateSourceGenerators;

internal sealed record EnumValidationInfo(EnumValidationInfo.EnumTypeInfo EnumType, EquatableArray<int> Values, bool IsFlags)
{
    public static EnumValidationInfo FromEnumType(ITypeSymbol enumType, bool isFlags)
    {
        var values = GetElementValues(enumType).OrderBy(e => e).Distinct().ToImmutableArray();
        return new EnumValidationInfo(EnumTypeInfo.FromEnumType(enumType), new EquatableArray<int>(values), isFlags);
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

    public sealed record EnumTypeInfo(string Text)
    {
        public static EnumTypeInfo FromEnumType(ITypeSymbol enumType)
        {
            return new EnumTypeInfo(enumType.ToString());
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
