// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class BindingManagerDataErrorEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_Exception_TestData()
        {
            yield return new object[] { new Exception() };
            yield return new object[] { null };
        }

        [Theory]
        [MemberData(nameof(Ctor_Exception_TestData))]
        public void Ctor_Exception(Exception exception)
        {
            var e = new BindingManagerDataErrorEventArgs(exception);
            Assert.Equal(exception, e.Exception);
        }
    }
}
