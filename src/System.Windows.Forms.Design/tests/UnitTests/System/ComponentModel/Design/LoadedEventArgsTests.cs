// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class LoadedEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_Bool_ICollection_TestData()
        {
            yield return new object[] { true, null };
            yield return new object[] { false, Array.Empty<object>() };
            yield return new object[] { true, new object[] { null } };
        }

        [Theory]
        [MemberData(nameof(Ctor_Bool_ICollection_TestData))]
        public void Ctor_Bool_ICollection(bool succeeded, ICollection errors)
        {
            var e = new LoadedEventArgs(succeeded, errors);
            Assert.Equal(succeeded, e.HasSucceeded);
            if (errors is null)
            {
                Assert.Empty(e.Errors);
            }
            else
            {
                Assert.Same(errors, e.Errors);
            }
        }
    }
}
