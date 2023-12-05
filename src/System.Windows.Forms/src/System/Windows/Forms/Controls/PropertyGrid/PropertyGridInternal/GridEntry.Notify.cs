// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.PropertyGridInternal;

internal abstract partial class GridEntry
{
    public enum Notify
    {
        /// <summary>
        ///  Asks the object to reset it's value.
        /// </summary>
        Reset = 1,

        /// <summary>
        ///  Asks if the object's value can be reset.
        /// </summary>
        CanReset = 2,

        /// <summary>
        ///  Tells the object of a double-click event.
        /// </summary>
        DoubleClick = 3,

        /// <summary>
        ///  Asks if the object value should be persisted.
        /// </summary>
        ShouldPersist = 4,

        /// <summary>
        ///  Tells the object of a return pressed event.
        /// </summary>
        Return = 5
    }
}
