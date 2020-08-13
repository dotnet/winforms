// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Returns an enumeration of windows and flags of how their scrollbars need to be themed when the designer is running inside Visual Studio.
    /// </summary>
    public interface IContainsThemedScrollbarWindows
    {
        IEnumerable ThemedScrollbarWindows();
    }
}
