// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace System.Windows.Forms.SourceGenerators
{
    internal class EnumValidationInfo
    {
        public ITypeSymbol EnumType { get; }
        public string ArgumentName { get; }
        public List<EnumElementInfo> Elements { get; }
        public bool IsFlags { get; }

        public EnumValidationInfo(ITypeSymbol enumType, string argumentName, bool isFlags)
        {
            EnumType = enumType;
            ArgumentName = argumentName;
            IsFlags = isFlags;
            Elements = GetEnumElements(enumType).OrderBy(e => e.Value).ToList();
        }

        private static IEnumerable<EnumElementInfo> GetEnumElements(ITypeSymbol enumType)
        {
            foreach (var member in enumType.GetMembers())
            {
                if (member is IFieldSymbol
                    {
                        IsStatic: true,
                        IsConst: true,
                        ConstantValue: int value,
                        Name: var name
                    })
                {
                    yield return new EnumElementInfo(name, value);
                }
            }
        }
    }
}
