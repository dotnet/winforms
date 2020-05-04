// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;

namespace System.Windows.Forms
{
    static internal class ClientUtils
    {
        // ExecutionEngineException is obsolete and shouldn't be used (to catch, throw or reference) anymore.
        // Pragma added to prevent converting the "type is obsolete" warning into build error.
        // File owner should fix this.
        public static bool IsCriticalException(Exception ex)
        {
            return ex is NullReferenceException
                    || ex is StackOverflowException
                    || ex is OutOfMemoryException
                    || ex is Threading.ThreadAbortException
                    || ex is ExecutionEngineException
                    || ex is IndexOutOfRangeException
                    || ex is AccessViolationException;
        }

        // Useful for enums that are a subset of a bitmask
        // Valid example: EdgeEffects  0, 0x800 (FillInterior), 0x1000 (Flat), 0x4000(Soft), 0x8000(Mono)
        //
        //   ClientUtils.IsEnumValid_Masked((int)(effects), /*mask*/ FillInterior | Flat | Soft | Mono,
        //          ,2);
        //
        public static bool IsEnumValid_Masked(Enum enumValue, int value, uint mask)
        {
            bool valid = ((value & mask) == value);

#if DEBUG
            Debug_ValidateMask(enumValue, mask);
#endif

            return valid;
        }

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

        private static void Debug_ValidateMask(Enum value, uint mask)
        {
            Type t = value.GetType();
            uint newmask = 0;
            foreach (int iVal in Enum.GetValues(t))
            {
                newmask |= (uint)iVal;
            }
            System.Diagnostics.Debug.Assert(newmask == mask, "Mask not valid in IsEnumValid!");
        }
#endif
    }
}
