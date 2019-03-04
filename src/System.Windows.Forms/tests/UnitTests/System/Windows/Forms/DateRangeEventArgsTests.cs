// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DateRangeEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_DateTime_DateTime_TestData()
        {
            yield return new object[] { DateTime.MinValue, DateTime.MinValue };
            yield return new object[] { new DateTime(10), new DateTime(0) };
        }

        [Theory]
        [MemberData(nameof(Ctor_DateTime_DateTime_TestData))]
        public void Ctor_DateTime_DateTime(DateTime start, DateTime end)
        {
            var e = new DateRangeEventArgs(start, end);
            Assert.Equal(start, e.Start);
            Assert.Equal(end, e.End);
        }
    }
}
