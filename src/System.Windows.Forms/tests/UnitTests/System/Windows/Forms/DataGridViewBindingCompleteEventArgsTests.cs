// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class DataGridViewBindingCompleteEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        [Theory]
        [InlineData(ListChangedType.ItemAdded)]
        [InlineData(ListChangedType.Reset - 1)]
        public void Ctor_ListChangedType(ListChangedType listChangedType)
        {
            var e = new DataGridViewBindingCompleteEventArgs(listChangedType);
            Assert.Equal(listChangedType, e.ListChangedType);
        }
    }
}
