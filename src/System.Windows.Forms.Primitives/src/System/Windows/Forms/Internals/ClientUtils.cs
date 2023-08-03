// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Windows.Forms;

internal static class ClientUtils
{
    private enum CharType
    {
        None,
        Word,
        NonWord
    }

    /// <summary>
    ///  Imitates the backwards word selection logic of the native SHAutoComplete Ctrl+Backspace handler.
    ///  The selection will consist of any run of word characters and any run of non-word characters at the end of that word.
    ///  If the selection reaches the second character in the input, and the first character is non-word, it is also selected.
    ///  Here, word characters are equivalent to the "\w" regex class but with UnicodeCategory.ConnectorPunctuation excluded.
    /// </summary>
    public static int GetWordBoundaryStart(string text, int endIndex)
    {
        bool seenWord = false;
        CharType lastSeen = CharType.None;
        int index = endIndex - 1;
        for (; index >= 0; index--)
        {
            char character = text[index];
            if (char.IsSurrogate(character))
            {
                break;
            }

            bool isWord = char.IsLetterOrDigit(character) ||
                CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark;
            if ((isWord && lastSeen == CharType.NonWord && seenWord) ||
                (!isWord && lastSeen == CharType.Word && index != 0))
            {
                break;
            }

            seenWord |= isWord;
            lastSeen = isWord ? CharType.Word : CharType.NonWord;
        }

        return index + 1;
    }
}
