// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.SourceGenerators
{
    internal class EnumElementInfo
    {
        public string Name { get; }
        public int Value { get; }

        public EnumElementInfo(string name, int value)
        {
            Name = name;
            Value = value;
        }
    }
}
