// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    public class DataGridViewAutoSizeColumnsModeEventArgs : EventArgs
    {
        private DataGridViewAutoSizeColumnMode[] previousModes;

        public DataGridViewAutoSizeColumnsModeEventArgs(DataGridViewAutoSizeColumnMode[] previousModes)
        {
            this.previousModes = previousModes;
        }

        [
            SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays") // Returning a collection would be overkill.
        ]
        public DataGridViewAutoSizeColumnMode[] PreviousModes
        {
            get
            {
                return this.previousModes;
            }
        }
    }
}
