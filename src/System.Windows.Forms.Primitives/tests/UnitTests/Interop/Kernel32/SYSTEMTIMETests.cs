// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;

namespace System.Windows.Forms.Tests.Interop.Kernel32
{
    public class SYSTEMTIMETests
    {
        [Fact]
        public unsafe void SYSTEMTIME_Sizeof_ReturnsExpected()
        {
            Assert.Equal(16, Marshal.SizeOf<PInvoke.SYSTEMTIME>());
            Assert.Equal(16, sizeof(PInvoke.SYSTEMTIME));
        }

        [Fact]
        public void SYSTEMTIME_Ctor_Default()
        {
            var st = new PInvoke.SYSTEMTIME();

            Assert.Equal(0, st.wYear);
            Assert.Equal(0, st.wMonth);
            Assert.Equal(0, st.wDayOfWeek);
            Assert.Equal(0, st.wDay);
            Assert.Equal(0, st.wHour);
            Assert.Equal(0, st.wMinute);
            Assert.Equal(0, st.wSecond);
            Assert.Equal(0, st.wMilliseconds);
        }

        [Fact]
        public void SYSTEMTIME_CastToDateTime_ReturnsExpected()
        {
            var st = new PInvoke.SYSTEMTIME()
            {
                wYear = 2021,
                wMonth = 5,
                wDay = 3,
                wHour = 6,
                wMinute = 15,
                wSecond = 30,
                wMilliseconds = 50
            };

            DateTime dt = st; // cast to DateTime implicitly

            Assert.Equal(st.wYear, dt.Year);
            Assert.Equal(st.wMonth, dt.Month);
            Assert.Equal(DayOfWeek.Monday, dt.DayOfWeek);
            Assert.Equal(st.wDay, dt.Day);
            Assert.Equal(st.wHour, dt.Hour);
            Assert.Equal(st.wMinute, dt.Minute);
            Assert.Equal(st.wSecond, dt.Second);
            Assert.Equal(st.wMilliseconds, dt.Millisecond);
        }

        [Fact]
        public void SYSTEMTIME_CastToDateTime_ThrowsException_IfArgumentsAreIncorrect()
        {
            var st = new PInvoke.SYSTEMTIME()
            {
                wYear = 9999,
                wMonth = 99,
                wDay = 99,
                wHour = 99,
                wMinute = 99,
                wSecond = 99,
                wMilliseconds = 9999
            };
            DateTime dt;

            Assert.Throws<ArgumentOutOfRangeException>(() => dt = st); // cast to DateTime implicitly with incorrect arguments
        }

        [Fact]
        public void SYSTEMTIME_CastToDateTime_ReturnsMinValue_IfValueIsDefault()
        {
            var st = new PInvoke.SYSTEMTIME();
            DateTime dt;

            using (new NoAssertContext())
            {
                dt = st; // cast to DateTime implicitly
            }

            Assert.Equal(DateTime.MinValue, dt);
        }
    }
}
