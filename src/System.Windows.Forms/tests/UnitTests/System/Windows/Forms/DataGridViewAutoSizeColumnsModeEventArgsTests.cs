// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class DataGridViewAutoSizeColumnsModeEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_DataGridViewAutoSizeColumnModeArray_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { Array.Empty<DataGridViewAutoSizeColumnMode>() };
            yield return new object[] { new DataGridViewAutoSizeColumnMode[] { DataGridViewAutoSizeColumnMode.AllCells, (DataGridViewAutoSizeColumnMode)(DataGridViewAutoSizeColumnMode.None - 1) } };
        }

        [Theory]
        [MemberData(nameof(Ctor_DataGridViewAutoSizeColumnModeArray_TestData))]
        public void Ctor_DataGridViewAutoSizeColumnModeArray(DataGridViewAutoSizeColumnMode[] previousModes)
        {
            var e = new DataGridViewAutoSizeColumnsModeEventArgs(previousModes);
            Assert.Equal(previousModes, e.PreviousModes);
        }
    }
}
