// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class SendKeys
{
    /// <summary>
    ///  Holds a keyword and the associated VK_ for it.
    /// </summary>
    private readonly struct KeywordVk
    {
        public readonly string Keyword;
        public readonly int VK;

        public KeywordVk(string keyword, Keys key)
        {
            Keyword = keyword;
            VK = (int)key;
        }
    }
}
