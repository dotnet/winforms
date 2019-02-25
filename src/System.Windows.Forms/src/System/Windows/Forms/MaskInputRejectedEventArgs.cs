// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the MaskInputRejected event.
    /// </devdoc>
    public class MaskInputRejectedEventArgs : EventArgs
    {
        public MaskInputRejectedEventArgs(int position, MaskedTextResultHint rejectionHint)
        {
            Position = position;
            RejectionHint = rejectionHint;
        }

        /// <devdoc>
        /// The position where the test failed the mask constraint.
        /// </devdoc>
        public int Position { get; }

        /// <devdoc>
        /// Retreives a hint on why the input is rejected.
        /// </devdoc>
        public MaskedTextResultHint RejectionHint { get; }
    }
}
