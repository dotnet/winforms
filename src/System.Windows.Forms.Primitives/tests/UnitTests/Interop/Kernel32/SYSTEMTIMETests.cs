// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Windows.Forms.Tests.Interop.Kernel32;

public class SYSTEMTIMETests
{
    [Fact]
    public unsafe void SYSTEMTIME_Sizeof_ReturnsExpected()
    {
        Assert.Equal(16, Marshal.SizeOf<SYSTEMTIME>());
        Assert.Equal(16, sizeof(SYSTEMTIME));
    }

    [Fact]
    public void SYSTEMTIME_Ctor_Default()
    {
        SYSTEMTIME st = default;

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
        SYSTEMTIME st = new()
        {
            wYear = 2021,
            wMonth = 5,
            wDay = 3,
            wHour = 6,
            wMinute = 15,
            wSecond = 30,
            wMilliseconds = 50
        };

        DateTime dt = (DateTime)st; // cast to DateTime implicitly

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
        SYSTEMTIME st = new()
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

        Assert.Throws<ArgumentOutOfRangeException>(() => dt = (DateTime)st); // cast to DateTime implicitly with incorrect arguments
    }

    [Fact]
    public void SYSTEMTIME_CastToDateTime_ReturnsMinValue_IfValueIsDefault()
    {
        SYSTEMTIME st = default;
        DateTime dt;

        using (new NoAssertContext())
        {
            dt = (DateTime)st; // cast to DateTime implicitly
        }

        Assert.Equal(DateTime.MinValue, dt);
    }
}
