// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal partial class Comdlg32
    {
        [Flags]
        public enum CC : uint
        {
            RGBINIT = 0x00000001,
            FULLOPEN = 0x00000002,
            PREVENTFULLOPEN = 0x00000004,
            SHOWHELP = 0x00000008,
            ENABLEHOOK = 0x00000010,
            ENABLETEMPLATE = 0x00000020,
            ENABLETEMPLATEHANDLE = 0x00000040,
            SOLIDCOLOR = 0x00000080,
            ANYCOLOR = 0x00000100,
        }
    }
}
