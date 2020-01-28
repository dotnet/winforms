// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides information about a Binding Completed event.
    /// </summary>
    public class BindingCompleteEventArgs : CancelEventArgs
    {
        /// <summary>
        ///  Constructor for BindingCompleteEventArgs.
        /// </summary>
        public BindingCompleteEventArgs(Binding binding,
                                        BindingCompleteState state,
                                        BindingCompleteContext context,
                                        string errorText,
                                        Exception exception,
                                        bool cancel) : base(cancel)
        {
            Binding = binding;
            BindingCompleteState = state;
            BindingCompleteContext = context;
            ErrorText = errorText ?? string.Empty;
            Exception = exception;
        }

        /// <summary>
        ///  Constructor for BindingCompleteEventArgs.
        /// </summary>
        public BindingCompleteEventArgs(Binding binding,
                                        BindingCompleteState state,
                                        BindingCompleteContext context,
                                        string errorText,
                                        Exception exception) : this(binding, state, context, errorText, exception, true)
        {
        }

        /// <summary>
        ///  Constructor for BindingCompleteEventArgs.
        /// </summary>
        public BindingCompleteEventArgs(Binding binding,
                                        BindingCompleteState state,
                                        BindingCompleteContext context,
                                        string errorText) : this(binding, state, context, errorText, null, true)
        {
        }

        /// <summary>
        ///  Constructor for BindingCompleteEventArgs.
        /// </summary>
        public BindingCompleteEventArgs(Binding binding,
                                        BindingCompleteState state,
                                        BindingCompleteContext context) : this(binding, state, context, string.Empty, null, false)
        {
        }

        public Binding Binding { get; }

        public BindingCompleteState BindingCompleteState { get; }

        public BindingCompleteContext BindingCompleteContext { get; }

        public string ErrorText { get; }

        public Exception Exception { get; }
    }
}
