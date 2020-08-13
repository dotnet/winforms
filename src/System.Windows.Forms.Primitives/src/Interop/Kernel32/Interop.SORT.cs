// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal partial class Kernel32
    {
        public enum SORT : byte
        {
            DEFAULT = 0x0,
            INVARIANT_MATH = 0x1,
            JAPANESE_XJIS = 0x0,
            JAPANESE_UNICODE = 0x1,
            JAPANESE_RADICALSTROKE = 0x4,
            CHINESE_BIG5 = 0x0,
            CHINESE_PRCP = 0x0,
            CHINESE_UNICODE = 0x1,
            CHINESE_PRC = 0x2,
            CHINESE_BOPOMOFO = 0x3,
            CHINESE_RADICALSTROKE = 0x4,
            KOREAN_KSC = 0x0,
            KOREAN_UNICODE = 0x1,
            GERMAN_PHONE_BOOK = 0x1,
            HUNGARIAN_DEFAULT = 0x0,
            HUNGARIAN_TECHNICAL = 0x1,
            GEORGIAN_TRADITIONAL = 0x0,
            GEORGIAN_MODERN = 0x1,
        }
    }
}
