// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;
    using System.ComponentModel;

    /// <devdoc>
    /// </devdoc>
    public class QuestionEventArgs : EventArgs
    {
        private bool response;

        /// <devdoc>
        /// </devdoc>
        public QuestionEventArgs()
        {
            this.response = false;
        }

        /// <devdoc>
        /// </devdoc>
        public QuestionEventArgs(bool response)
        {
            this.response = response;
        }

        /// <devdoc>
        /// </devdoc>
        public bool Response
        {
            get
            {
                return this.response;
            }
            set
            {
                this.response = value;
            }
        }
    }
}

