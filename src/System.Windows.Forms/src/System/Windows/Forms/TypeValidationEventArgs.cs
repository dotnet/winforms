// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the TypeValidationEventHandler event.
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

        /// <summary>
        ///  The position where the test failed the mask constraint.
        /// </summary>
        public Type ValidatingType { get; }

        /// <summary>
        ///  The exception thrown by the validating object while performing the data validation.
        /// </summary>
        public bool IsValidInput { get; }

        /// <summary>
        ///  A message about the validation operation. Intended to be populated with
        ///  an exception information if any thrown.
        /// </summary>
        public string Message { get; }

        /// <summary>
        ///  The value returned from the Parse method.
        /// </summary>
        public object ReturnValue { get; }

        /// <summary>
        ///  Specifies whether focus should be allowed to be shifted from the control.
        /// </summary>
        public bool Cancel { get; set; }
    }
}
