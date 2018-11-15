// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;
    using System.ComponentModel;

    /// <include file='doc\QuestionEventArgs.uex' path='docs/doc[@for="QuestionEventArgs"]/*' />
    /// <devdoc>
    /// </devdoc>
    public class QuestionEventArgs : EventArgs
    {
        private bool response;

        /// <include file='doc\QuestionEventArgs.uex' path='docs/doc[@for="QuestionEventArgs.QuestionEventArgs1"]/*' />
        /// <devdoc>
        /// </devdoc>
        public QuestionEventArgs()
        {
            this.response = false;
        }

        /// <include file='doc\QuestionEventArgs.uex' path='docs/doc[@for="QuestionEventArgs.QuestionEventArgs2"]/*' />
        /// <devdoc>
        /// </devdoc>
        public QuestionEventArgs(bool response)
        {
            this.response = response;
        }

        /// <include file='doc\QuestionEventArgs.uex' path='docs/doc[@for="QuestionEventArgs.Reponse"]/*' />
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

