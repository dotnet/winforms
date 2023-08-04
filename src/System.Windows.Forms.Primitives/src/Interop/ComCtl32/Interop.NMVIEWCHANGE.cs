﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public struct NMVIEWCHANGE
        {
            public NMHDR nmhdr;
            public MONTH_CALDENDAR_MESSAGES_VIEW uOldView;
            public MONTH_CALDENDAR_MESSAGES_VIEW uNewView;
        }
    }
}
