// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  Provides additional information for the Loaded event.
    /// </summary>
    public sealed class LoadedEventArgs : EventArgs
    {
        /// <summary>
        ///  Creates a new LoadedEventArgs object.
        /// </summary>
        public LoadedEventArgs(bool succeeded, ICollection errors)
        {
            HasSucceeded = succeeded;
            Errors = errors ?? Array.Empty<object>();
        }

        /// <summary>
        ///  True to indicate the designer load was successful. Even successful loads can have errors, if the errors were not too servere to prevent the designer from loading.
        /// </summary>
        public bool HasSucceeded { get; }

        /// <summary>
        ///  A collection of errors that occurred while the designer was loading.
        /// </summary>
        public ICollection Errors { get; }
    }
}
