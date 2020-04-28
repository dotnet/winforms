// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class DataGridViewRowStateChangedEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_DataGridViewRow_DataGridViewElementStates_TestData()
        {
            yield return new object[] { null, (DataGridViewElementStates)7 };
            yield return new object[] { new DataGridViewRow(), DataGridViewElementStates.Displayed };
        }

        [Theory]
        [MemberData(nameof(Ctor_DataGridViewRow_DataGridViewElementStates_TestData))]
        public void Ctor_DataGridViewRow_DataGridViewElementStates(DataGridViewRow dataGridViewRow, DataGridViewElementStates stateChanged)
        {
            var e = new DataGridViewRowStateChangedEventArgs(dataGridViewRow, stateChanged);
            Assert.Equal(dataGridViewRow, e.Row);
            Assert.Equal(stateChanged, e.StateChanged);
        }
    }
}
