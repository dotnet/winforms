// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        internal interface IMouseHookClient
        {
            /// <summary>
            /// OnClickHooked Event
            /// </summary>
            /// <returns>true if the click is handled; false to pass it on</returns>
            bool OnClickHooked();
        }
    }
}
