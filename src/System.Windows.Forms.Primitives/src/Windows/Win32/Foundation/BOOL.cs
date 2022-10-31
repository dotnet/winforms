// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32.Foundation
{
    internal readonly partial struct BOOL
    {
        public override string ToString() => ((bool)this).ToString();

        public const int Size = sizeof(int);

        public static BOOL TRUE { get; } = new(true);

        public static BOOL FALSE { get; } = new(false);

        public static bool operator true(BOOL value) => value.Value != 0;

        public static bool operator false(BOOL value) => value.Value == 0;
    }
}
