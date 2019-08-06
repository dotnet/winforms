// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Mso
    {
        /// <summary>
        ///  MSO Component registration flags
        /// </summary>
        [Flags]
        public enum msocrf : uint
        {
            NeedIdleTime = 1,
            NeedPeriodicIdleTime = 2,
            PreTranslateKeys = 4,
            PreTranslateAll = 8,
            NeedSpecActiveNotifs = 16,
            NeedAllActiveNotifs = 32,
            ExclusiveBorderSpace = 64,
            ExclusiveActivation = 128,
            NeedAllMacEvents = 256,
            Master = 512
        }
    }
}
