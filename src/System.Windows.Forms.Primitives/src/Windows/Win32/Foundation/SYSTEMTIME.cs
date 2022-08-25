// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace Windows.Win32.Foundation
{
    internal partial struct SYSTEMTIME
    {
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

        /// <summary>
        ///  Converts <see cref="DateTime"/> value with a 1 second granularity.
        /// </summary>
        public static implicit operator SYSTEMTIME(DateTime time) => new SYSTEMTIME
        {
            wYear = (ushort)time.Year,
            wMonth = (ushort)time.Month,
            wDayOfWeek = (ushort)time.DayOfWeek,
            wDay = (ushort)time.Day,
            wHour = (ushort)time.Hour,
            wMinute = (ushort)time.Minute,
            wSecond = (ushort)time.Second,
            wMilliseconds = 0
        };
    }
}
