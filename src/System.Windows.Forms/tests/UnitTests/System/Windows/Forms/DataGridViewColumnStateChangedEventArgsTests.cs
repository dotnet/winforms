// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewColumnStateChangedEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_DataGridViewColumn_DataGridViewElementStates_TestData()
        {
            yield return new object[] { null, (DataGridViewElementStates)7 };
            yield return new object[] { new DataGridViewColumn(), DataGridViewElementStates.Displayed };
        }

        [Theory]
        [MemberData(nameof(Ctor_DataGridViewColumn_DataGridViewElementStates_TestData))]
        public void Ctor_DataGridViewColumn_DataGridViewElementStates(DataGridViewColumn dataGridViewColumn, DataGridViewElementStates stateChanged)
        {
            var e = new DataGridViewColumnStateChangedEventArgs(dataGridViewColumn, stateChanged);
            Assert.Equal(dataGridViewColumn, e.Column);
            Assert.Equal(stateChanged, e.StateChanged);
        }
    }
}
