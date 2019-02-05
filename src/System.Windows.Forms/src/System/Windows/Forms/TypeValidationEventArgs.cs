// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the TypeValidationEventHandler event.
    /// </summary>
    public class TypeValidationEventArgs : EventArgs
    {
        public TypeValidationEventArgs(Type validatingType, bool isValidInput, object returnValue, string message)
        {
            ValidatingType = validatingType;
            IsValidInput = isValidInput;
            ReturnValue = returnValue;
            Message = message;
        }

        /// <devdoc>
        /// The position where the test failed the mask constraint.
        /// </devdoc>
        public Type ValidatingType { get; }

        /// <devdoc>
        /// The exception thrown by the validating object while performing the data validation.
        /// </devdoc>
        public bool IsValidInput { get; }

        /// <devdoc>
        /// A message about the validation operation. Intended to be populated with
        /// an exception information if any thrown.
        /// </devdoc>
        public string Message { get; }

        /// <devdoc>
        /// The value returned from the Parse method.
        /// </devdoc>
        public object ReturnValue { get; }

        /// <devdoc>
        /// Specifies whether focus should be allowed to be shifted from the control.
        /// </devdoc>
        public bool Cancel { get; set; }
    }
}
