// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    /// <summary>
    ///  The event class that is created when the selected GridItem in the PropertyGrid is changed by the user.
    /// </summary>
    public class SelectedGridItemChangedEventArgs : EventArgs
    {
        /// <summary>
        ///  Constructs a SelectedGridItemChangedEventArgs object.
        /// </summary>
        public SelectedGridItemChangedEventArgs(GridItem oldSel, GridItem newSel)
        {
            OldSelection = oldSel;
            NewSelection = newSel;
        }

        /// <summary>
        ///  The previously selected GridItem object. This can be null.
        /// </summary>
        public GridItem OldSelection { get; }

        /// <summary>
        ///  The newly selected GridItem object
        /// </summary>
        public GridItem NewSelection { get; }
    }
}
