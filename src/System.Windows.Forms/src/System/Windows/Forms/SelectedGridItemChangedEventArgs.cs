// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// The event class that is created when the selected GridItem in the PropertyGrid is changed by the user.
    /// </devdoc>
    public class SelectedGridItemChangedEventArgs : EventArgs
    {
        /// <devdoc>
        /// Constructs a SelectedGridItemChangedEventArgs object.
        /// </devdoc>
        public SelectedGridItemChangedEventArgs(GridItem oldSel, GridItem newSel)
        {
            OldSelection = oldSel;
            NewSelection = newSel;
        }

        /// <devdoc>
        /// The previously selected GridItem object. This can be null.
        /// </devdoc>
        public GridItem OldSelection { get; }

        /// <devdoc>
        /// The newly selected GridItem object
        /// </devdoc>
        public GridItem NewSelection { get; }
   }
}
