// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class PropertyGridView
{
    internal interface IMouseHookClient
    {
        /// <summary>
        ///  Returns true if the click is handled, false to pass it on.
        /// </summary>
        bool OnClickHooked();
    }
}
