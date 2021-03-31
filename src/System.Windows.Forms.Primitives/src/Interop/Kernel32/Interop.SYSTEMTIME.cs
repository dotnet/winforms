// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

internal partial class Interop
{
    internal partial class Kernel32
    {
        public struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;

            public static implicit operator DateTime(SYSTEMTIME sysTime)
            {
                if (sysTime.wYear <= 0 || sysTime.wMonth <= 0 || sysTime.wDay <= 0)
                {
                    Debug.Fail("Incorrect SYSTEMTIME info!");
                    return DateTime.MinValue;
                }

                // DateTime gets DayOfWeek automatically
                return new DateTime(sysTime.wYear,
                    sysTime.wMonth, sysTime.wDay, sysTime.wHour,
                    sysTime.wMinute, sysTime.wSecond, sysTime.wMilliseconds);
            }
        }
    }
}
