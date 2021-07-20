// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.PropertyGridInternal
{
    internal abstract partial class GridEntry
    {
        public enum Notify
        {
            Reset = 1,
            CanReset = 2,
            DoubleClick = 3,
            ShouldPersist = 4,
            Return = 5
        }
    }
}
