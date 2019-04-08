// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.ComponentModel.Design
{
    /// <summary>
    /// Represents the method that will handle a Loaded event.
    /// </summary>
    public delegate void LoadedEventHandler(object sender, LoadedEventArgs e);

    /// <summary>
    /// Provides additional information for the Loaded event.
    /// </summary>
    public sealed class LoadedEventArgs : EventArgs
    {
        private readonly bool _succeeded;
        private readonly ICollection _errors;
        /// <summary>
        /// Creates a new LoadedEventArgs object.
        /// </summary>
        public LoadedEventArgs(bool succeeded, ICollection errors)
        {
            _succeeded = succeeded;
            _errors = errors;
            if (_errors == null)
            {
                _errors = new object[0];
            }
        }

        /// <summary>
        /// A collection of errors that occurred while the designer was loading.
        /// </summary>
        public ICollection Errors
        {
            get => _errors;
        }

        /// <summary>
        /// True to indicate the designer load was successful. Even successful loads can have errors, if the errors were not too servere to prevent the designer from loading.
        /// </summary>
        public bool HasSucceeded
        {
            get => _succeeded;
        }
    }
}
