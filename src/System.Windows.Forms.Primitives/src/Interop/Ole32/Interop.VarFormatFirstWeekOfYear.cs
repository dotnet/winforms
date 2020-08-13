// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal static partial class Ole32
    {
        public enum VarFormatFirstWeekOfYear : int
        {
            SystemDefault = 0,
            ContainsJanuaryFirst = 1,
            LargerHalfInCurrentYear = 2,
            HasSevenDays = 3,
        }
    }
}
