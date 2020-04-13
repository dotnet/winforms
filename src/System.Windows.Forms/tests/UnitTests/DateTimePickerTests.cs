// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DateTimePickerTests
    {
        [Fact]
        public void DateTimePicker_Constructor()
        {
            var dtp = new DateTimePicker();

            Assert.NotNull(dtp);
            Assert.Equal(DateTimePickerFormat.Long, dtp.Format);
        }

        [WinFormsFact]
        public void DateTimePicker_SysTimeToDateTime_DoesnThrowException_If_SYSTEMTIME_IsIncorrect()
        {
            // An empty SYSTEMTIME has year, month and day as 0, but DateTime can't have these parameters.
            // So an empty SYSTEMTIME is incorrect in this case.
            Interop.Kernel32.SYSTEMTIME systemTime = new Interop.Kernel32.SYSTEMTIME();
            DateTime dateTime = DateTimePicker.SysTimeToDateTime(systemTime);
            Assert.Equal(DateTime.MinValue, dateTime);
        }
    }
}
