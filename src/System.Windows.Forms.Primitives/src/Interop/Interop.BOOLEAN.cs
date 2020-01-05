// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

internal partial class Interop
{
    /// <summary>
    ///  Blittable version of Windows BOOLEAN type. It is convenient in situations where
    ///  manual marshalling is required, or to avoid overhead of regular BOOLEAN marshalling.
    /// </summary>
    /// <remarks>
    ///  Some Windows APIs return arbitrary integer values although the return type is defined
    ///  as BOOL. It is best to never compare BOOLEAN to TRUE. Always use bResult != BOOLEAN.FALSE
    ///  or bResult == BOOLEAN.FALSE .
    /// </remarks>
    internal enum BOOLEAN : byte
    {
        FALSE = 0,
        TRUE = 1,
    }
}

internal static class BooleanExtensions
{
    public static bool IsTrue(this BOOLEAN b) => b != BOOLEAN.FALSE;
    public static bool IsFalse(this BOOLEAN b) => b == BOOLEAN.FALSE;
    public static BOOLEAN ToBOOLEAN(this bool b) => b ? BOOLEAN.TRUE : BOOLEAN.FALSE;
}
