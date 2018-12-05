// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;

    /// <devdoc>
    ///     Provides information about a Binding Completed event.
    /// </devdoc>
    public class BindingCompleteEventArgs : CancelEventArgs {
        private Binding binding;
        private BindingCompleteState state;
        private BindingCompleteContext context;
        private	string errorText;
        private	Exception exception;

        /// <devdoc>
        ///    Constructor for BindingCompleteEventArgs.
        /// </devdoc>
        public BindingCompleteEventArgs(Binding binding,
                                        BindingCompleteState state,
                                        BindingCompleteContext context,
                                        string errorText,
                                        Exception exception,
                                        bool cancel) : base(cancel) {
            this.binding = binding;
            this.state = state;
            this.context = context;
            this.errorText = (errorText == null) ? string.Empty : errorText;
            this.exception = exception;
        }

        /// <devdoc>
        ///    Constructor for BindingCompleteEventArgs.
        /// </devdoc>
        public BindingCompleteEventArgs(Binding binding,
                                        BindingCompleteState state,
                                        BindingCompleteContext context,
                                        string errorText,
                                        Exception exception) : this(binding, state, context, errorText, exception, true) {
        }

        /// <devdoc>
        ///    Constructor for BindingCompleteEventArgs.
        /// </devdoc>
        public BindingCompleteEventArgs(Binding binding,
                                        BindingCompleteState state,
                                        BindingCompleteContext context,
                                        string errorText) : this(binding, state, context, errorText, null, true) {
        }

        /// <devdoc>
        ///    Constructor for BindingCompleteEventArgs.
        /// </devdoc>
        public BindingCompleteEventArgs(Binding binding,
                                        BindingCompleteState state,
                                        BindingCompleteContext context) : this(binding, state, context, string.Empty, null, false) {
        }

        /// <devdoc>
        /// </devdoc>
        public Binding Binding {
            get {
                return this.binding;
            }
        }

        /// <devdoc>
        /// </devdoc>
        public BindingCompleteState BindingCompleteState {
            get {
                return this.state;
            }
        }

        /// <devdoc>
        /// </devdoc>
        public BindingCompleteContext BindingCompleteContext {
            get {
                return this.context;
            }
        }

        /// <devdoc>
        /// </devdoc>
        public string ErrorText {
            get {
                return this.errorText;
            }
        }

        /// <devdoc>
        /// </devdoc>
        public Exception Exception
        {
            get {
                return this.exception;
            }
        }
    }
}
