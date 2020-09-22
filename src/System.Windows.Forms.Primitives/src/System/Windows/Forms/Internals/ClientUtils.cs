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

        // Sequential version
        // assumes sequential enum members 0,1,2,3,4 -etc.
        //
        public static bool IsEnumValid(Enum enumValue, int value, int minValue, int maxValue)
        {
            bool valid = (value >= minValue) && (value <= maxValue);
#if DEBUG
            Debug_SequentialEnumIsDefinedCheck(enumValue, minValue, maxValue);
#endif
            return valid;
        }

        // Useful for sequential enum values which only use powers of two 0,1,2,4,8 etc: IsEnumValid(val, min, max, 1)
        // Valid example: TextImageRelation 0,1,2,4,8 - only one bit can ever be on, and the value is between 0 and 8.
        //
        //   ClientUtils.IsEnumValid((int)(relation), /*min*/(int)TextImageRelation.None, (int)TextImageRelation.TextBeforeImage,1);
        //
        public static bool IsEnumValid(Enum enumValue, int value, int minValue, int maxValue, int maxNumberOfBitsOn)
        {
            System.Diagnostics.Debug.Assert(maxNumberOfBitsOn >= 0 && maxNumberOfBitsOn < 32, "expect this to be greater than zero and less than 32");

            bool valid = (value >= minValue) && (value <= maxValue);
            //Note: if it's 0, it'll have no bits on.  If it's a power of 2, it'll have 1.
            valid = (valid && BitOperations.PopCount((uint)value) <= maxNumberOfBitsOn);
#if DEBUG
            Debug_NonSequentialEnumIsDefinedCheck(enumValue, minValue, maxValue, maxNumberOfBitsOn, valid);
#endif
            return valid;
        }

        // Useful for enums that are a subset of a bitmask
        // Valid example: EdgeEffects  0, 0x800 (FillInterior), 0x1000 (Flat), 0x4000(Soft), 0x8000(Mono)
        //
        //   ClientUtils.IsEnumValid((int)(effects), /*mask*/ FillInterior | Flat | Soft | Mono,
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

        // Useful for cases where you have discontiguous members of the enum.
        // Valid example: AutoComplete source.
        // if (!ClientUtils.IsEnumValid(value, AutoCompleteSource.None,
        //                                            AutoCompleteSource.AllSystemSources
        //                                            AutoCompleteSource.AllUrl,
        //                                            AutoCompleteSource.CustomSource,
        //                                            AutoCompleteSource.FileSystem,
        //                                            AutoCompleteSource.FileSystemDirectories,
        //                                            AutoCompleteSource.HistoryList,
        //                                            AutoCompleteSource.ListItems,
        //                                            AutoCompleteSource.RecentlyUsedList))
        //
        // PERF tip: put the default value in the enum towards the front of the argument list.
        public static bool IsEnumValid_NotSequential(Enum enumValue, int value, params int[] enumValues)
        {
            Debug.Assert(Enum.GetValues(enumValue.GetType()).Length == enumValues.Length, "Not all the enum members were passed in.");
            for (int i = 0; i < enumValues.Length; i++)
            {
                if (enumValues[i] == value)
                {
                    return true;
                }
            }
            return false;
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

#if DEBUG
        [ThreadStatic]
        private static Hashtable? enumValueInfo;
        public const int MAXCACHE = 300;  // we think we're going to get O(100) of these, put in a tripwire if it gets larger.

        private class SequentialEnumInfo
        {
            public SequentialEnumInfo(Type t)
            {
                int actualMinimum = int.MaxValue;
                int actualMaximum = int.MinValue;
                int countEnumVals = 0;

                foreach (int iVal in Enum.GetValues(t))
                {
                    actualMinimum = Math.Min(actualMinimum, iVal);
                    actualMaximum = Math.Max(actualMaximum, iVal);
                    countEnumVals++;
                }

                if (countEnumVals - 1 != (actualMaximum - actualMinimum))
                {
                    Debug.Fail("this enum cannot be sequential.");
                }
                MinValue = actualMinimum;
                MaxValue = actualMaximum;
            }
            public int MinValue;
            public int MaxValue;
        }

        private static void Debug_SequentialEnumIsDefinedCheck(Enum value, int minVal, int maxVal)
        {
            Type t = value.GetType();

            if (enumValueInfo is null)
            {
                enumValueInfo = new Hashtable();
            }

            SequentialEnumInfo? sequentialEnumInfo = null;

            if (enumValueInfo.ContainsKey(t))
            {
                sequentialEnumInfo = enumValueInfo[t] as SequentialEnumInfo;
            }
            if (sequentialEnumInfo is null)
            {
                sequentialEnumInfo = new SequentialEnumInfo(t);

                if (enumValueInfo.Count > MAXCACHE)
                {
                    // see comment next to MAXCACHE declaration.
                    Debug.Fail("cache is too bloated, clearing out, we need to revisit this.");
                    enumValueInfo.Clear();
                }
                enumValueInfo[t] = sequentialEnumInfo;
            }
            if (minVal != sequentialEnumInfo.MinValue)
            {
                // put string allocation in the IF block so the common case doesnt build up the string.
                System.Diagnostics.Debug.Fail("Minimum passed in is not the actual minimum for the enum.  Consider changing the parameters or using a different function.");
            }
            if (maxVal != sequentialEnumInfo.MaxValue)
            {
                // put string allocation in the IF block so the common case doesnt build up the string.
                Debug.Fail("Maximum passed in is not the actual maximum for the enum.  Consider changing the parameters or using a different function.");
            }
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

        private static void Debug_NonSequentialEnumIsDefinedCheck(Enum value, int minVal, int maxVal, int maxBitsOn, bool isValid) {
               Type t = value.GetType();
               int actualMinimum = int.MaxValue;
               int actualMaximum = int.MinValue;
               int checkedValue = Convert.ToInt32(value, CultureInfo.InvariantCulture);
               int maxBitsFound = 0;
               bool foundValue = false;
            foreach (int iVal in Enum.GetValues(t)){
                actualMinimum = Math.Min(actualMinimum, iVal);
                   actualMaximum = Math.Max(actualMaximum, iVal);
                   maxBitsFound = Math.Max(maxBitsFound, BitOperations.PopCount((uint)iVal));
                   if (checkedValue == iVal) {
                       foundValue = true;
                   }
               }
               if (minVal != actualMinimum) {
                    // put string allocation in the IF block so the common case doesnt build up the string.
                   System.Diagnostics.Debug.Fail( "Minimum passed in is not the actual minimum for the enum.  Consider changing the parameters or using a different function.");
               }
               if (maxVal != actualMaximum) {
                    // put string allocation in the IF block so the common case doesnt build up the string.
                   System.Diagnostics.Debug.Fail("Maximum passed in is not the actual maximum for the enum.  Consider changing the parameters or using a different function.");
               }

               if (maxBitsFound != maxBitsOn) {
                   System.Diagnostics.Debug.Fail("Incorrect usage of IsEnumValid function. The bits set to 1 in this enum was found to be: " + maxBitsFound.ToString(CultureInfo.InvariantCulture) + "this does not match what's passed in: " + maxBitsOn.ToString(CultureInfo.InvariantCulture));
               }
               if (foundValue != isValid) {
                    System.Diagnostics.Debug.Fail(string.Format(CultureInfo.InvariantCulture, "Returning {0} but we actually {1} found the value in the enum! Consider using a different overload to IsValidEnum.", isValid, ((foundValue) ? "have" : "have not")));
               }
           }
#endif
    }
}
