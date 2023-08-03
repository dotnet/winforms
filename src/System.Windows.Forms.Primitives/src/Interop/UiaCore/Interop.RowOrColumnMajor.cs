// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        [Flags]
        public enum RowOrColumnMajor
        {
            RowMajor = 0,
            ColumnMajor = 1,
            Indeterminate = 2
        }
    }
}
