// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System.ComponentModel;
    using System.Diagnostics;

    /// <devdoc>
    ///     MaskInputRejectedEventArgs.  Provides data for the MaskInputRejected event.
    /// </devdoc>
    public class MaskInputRejectedEventArgs : EventArgs
    {
        private int position;
        MaskedTextResultHint hint;
        
        public MaskInputRejectedEventArgs(int position, MaskedTextResultHint rejectionHint)
        {
            Debug.Assert(!MaskedTextProvider.GetOperationResultFromHint(rejectionHint), "Rejection hint is not on a failure.");
            this.position = position;
            this.hint     = rejectionHint;
        }

        /// <devdoc>
        ///     The position where the test failed the mask constraint.
        /// </devdoc>
        public int Position
        {
            get 
            { 
                return this.position; 
            }
        }

        /// <devdoc>
        ///     Retreives a hint on why the input is rejected.
        /// </devdoc>
        public MaskedTextResultHint RejectionHint
        {
            get 
            { 
                return this.hint; 
            }
        }
    }
}
