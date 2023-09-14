// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.Controls;

internal partial struct LVITEMW
{
    /// <summary>
    ///  Set the new text. The text length is limited by <see cref="cchTextMax"/>.
    ///  A value of <see cref="cchTextMax"/> will be updated to the length of <paramref name="text"/> + 1.
    /// </summary>
    /// <param name="text">The text to set.</param>
    public unsafe void UpdateText(ReadOnlySpan<char> text)
    {
        if (cchTextMax <= text.Length)
        {
            text = text[..(cchTextMax - 1)];
        }
        else
        {
            cchTextMax = text.Length + 1;
        }

        // Create a span from the pszText and copy the input text to it
        Span<char> targetSpan = new(pszText, cchTextMax);
        text.CopyTo(targetSpan);
        pszText.Value[text.Length] = '\0';
    }
}
