// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the MaskInputRejected event.
    /// </summary>
    public class MaskInputRejectedEventArgs : EventArgs
    {
        public MaskInputRejectedEventArgs(int position, MaskedTextResultHint rejectionHint)
        {
            Position = position;
            RejectionHint = rejectionHint;
        }

        /// <summary>
        ///  The position where the test failed the mask constraint.
        /// </summary>
        public int Position { get; }

        /// <summary>
        ///  Retreives a hint on why the input is rejected.
        /// </summary>
        public MaskedTextResultHint RejectionHint { get; }
    }
}
