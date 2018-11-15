// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///     TypeValidationEventArgs.  Provides data for the TypeValidationEventHandler event.
    /// </summary>
    public class TypeValidationEventArgs : EventArgs
    {
        private Type   validatingType;
        private string message;
        private bool   isValidInput;
        private object returnValue;
        private bool   cancel;

        public TypeValidationEventArgs(Type validatingType, bool isValidInput, object returnValue, string message)
        {
            this.validatingType = validatingType;
            this.isValidInput   = isValidInput;
            this.returnValue    = returnValue;
            this.message        = message;
        }

        /// <devdoc>
        ///     Specifies whether focus should be allowed to be shifted from the control.
        /// </devdoc>
        public bool Cancel
        {
            get
            {
                return this.cancel;
            }
            set
            {
                this.cancel = value;
            }
        }

        /// <devdoc>
        ///     The exception thrown by the validating object while performing the data validation.
        /// </devdoc>
        public bool IsValidInput
        {
            get
            {
                return this.isValidInput;
            }
        }

        /// <devdoc>
        ///     A message about the validation operation.  Intended to be populated with an exception information if 
        ///     any thrown.
        /// </devdoc>
        public string Message
        {
            get
            {
                return this.message;
            }
        }

        /// <devdoc>
        ///     The value returned from the Parse method.
        /// </devdoc>
        public object ReturnValue
        {
            get
            {
                return this.returnValue;
            }
        }

        /// <devdoc>
        ///     The position where the test failed the mask constraint.
        /// </devdoc>
        public Type ValidatingType
        {
            get 
            { 
                return this.validatingType; 
            }
        }
    }
}
