// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///  Defines the Priority of a SnapLine.  During the drag operation, when more than
    ///  one SnapLine is visible at any given time - only the highest priority SnapLines
    ///  are rendered.
    /// </summary>
    public enum SnapLinePriority
    {
        Low = 1,

        Medium = 2,

        High = 3,

        Always = 4
    }
}
