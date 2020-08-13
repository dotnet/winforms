// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Enum defining inclusion of special characters.
    /// </summary>
    public enum MaskFormat
    {
        IncludePrompt = 0x0001,
        IncludeLiterals = 0x0002,

        // both of the above
        IncludePromptAndLiterals = 0x0003,

        // Never include special characters.
        ExcludePromptAndLiterals = 0x000
    }
}
