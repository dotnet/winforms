// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    public class DataGridViewAutoSizeColumnsModeEventArgs : EventArgs
    {
        public DataGridViewAutoSizeColumnsModeEventArgs(DataGridViewAutoSizeColumnMode[] previousModes)
        {
            PreviousModes = previousModes;
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")] // Returning a collection would be overkill.
        public DataGridViewAutoSizeColumnMode[] PreviousModes { get; }
    }
}
