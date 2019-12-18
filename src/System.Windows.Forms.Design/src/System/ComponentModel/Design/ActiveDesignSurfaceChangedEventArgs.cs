// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  The event args for the ActiveDesignSurface event.
    /// </summary>
    public class ActiveDesignSurfaceChangedEventArgs : EventArgs
    {
        /// <summary>
        ///  Creates a new ActiveDesignSurfaceChangedEventArgs instance.
        /// </summary>
        public ActiveDesignSurfaceChangedEventArgs(DesignSurface oldSurface, DesignSurface newSurface)
        {
            OldSurface = oldSurface;
            NewSurface = newSurface;
        }

        /// <summary>
        ///  Gets the design surface that is losing activation.
        /// </summary>
        public DesignSurface OldSurface { get; }

        /// <summary>
        ///  Gets the design surface that is gaining activation.
        /// </summary>
        public DesignSurface NewSurface { get; }
    }
}
