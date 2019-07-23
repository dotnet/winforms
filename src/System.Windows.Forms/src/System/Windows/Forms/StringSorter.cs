// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This class provides methods to perform locale based comparison of strings
    ///  and sorting of arrays.
    /// </summary>
    internal sealed class StringSorter
    {
        /// <summary>
        ///  Ignore case when comparing two strings. When this flag is specified in
        ///  calls to compare() and sort(), two strings are considered equal if they
        ///  differ only in casing.
        /// </summary>
        public const int IgnoreCase = 0x00000001;

        /// <summary>
        ///  Do not differentiate between Hiragana and Katakana characters. When this
        ///  flag is specified in calls to compare() and sort(), corresponding
        ///  Hiragana and Katakana characters compare as equal.
        /// </summary>
        public const int IgnoreKanaType = 0x00010000;

        /// <summary>
        ///  Ignore nonspacing marks (accents, diacritics, and vowel marks). When
        ///  this flag is specified in calls to compare() and sort(), strings compare
        ///  as equal if they differ only in how characters are accented.
        /// </summary>
        public const int IgnoreNonSpace = 0x00000002;

        /// <summary>
        ///  Ignore symbols. When this flag is specified in calls to compare() and
        ///  sort(), strings compare as equal if they differ only in what symbol
        ///  characters they contain.
        /// </summary>
        public const int IgnoreSymbols = 0x00000004;

        /// <summary>
        ///  Ignore character widths. When this flag is specified in calls to
        ///  compare() and sort(), string comparisons do not differentiate between a
        ///  single-ubyte character and the same character as a double-ubyte character.
        /// </summary>
        public const int IgnoreWidth = 0x00020000;

        /// <summary>
        ///  Treat punctuation the same as symbols. Typically, strings are compared
        ///  using what is called a "word sort" technique. In a word sort, all
        ///  punctuation marks and other nonalphanumeric characters, except for the
        ///  hyphen and the apostrophe, come before any alphanumeric character. The
        ///  hyphen and the apostrophe are treated differently than the other
        ///  nonalphanumeric symbols, in order to ensure that words such as "coop"
        ///  and "co-op" stay together within a sorted list. If the STRINGSORT flag
        ///  is specified, strings are compared using what is called a "string sort"
        ///  technique. In a string sort, the hyphen and apostrophe are treated just
        ///  like any other nonalphanumeric symbols: they come before the
        ///  alphanumeric symbols.
        /// </summary>
        public const int StringSort = 0x00001000;

        /// <summary>
        ///  Descending sort. When this flag is specified in a call to sort(), the
        ///  strings are sorted in descending order. This flag should not be used in
        ///  calls to compare().
        /// </summary>
        public const int Descending = unchecked((int)0x80000000);

        private const int CompareOptions = IgnoreCase | IgnoreKanaType |
            IgnoreNonSpace | IgnoreSymbols | IgnoreWidth | StringSort;

        private readonly string[] keys;
        private readonly object[] items;
        private readonly int lcid;
        private readonly int options;
        private readonly bool descending;

        /// <summary>
        ///  Constructs a StringSorter. Instances are created by the sort() routines,
        ///  but never by the user.
        /// </summary>
        private StringSorter(CultureInfo culture, string[] keys, object[] items, int options)
        {
            if (keys == null)
            {
                if (items is string[])
                {
                    keys = (string[])items;
                    items = null;
                }
                else
                {
                    keys = new string[items.Length];
                    for (int i = 0; i < items.Length; i++)
                    {
                        object item = items[i];
                        if (item != null)
                        {
                            keys[i] = item.ToString();
                        }
                    }
                }
            }
            this.keys = keys;
            this.items = items;
            lcid = culture == null ? SafeNativeMethods.GetThreadLocale() : culture.LCID;
            this.options = options & CompareOptions;
            descending = (options & Descending) != 0;
        }

        internal static int ArrayLength(object[] array)
        {
            if (array == null)
            {
                return 0;
            }
            else
            {
                return array.Length;
            }
        }

        /// <summary>
        ///  Compares two strings using the default locale and no additional string
        ///  comparison flags.
        /// </summary>
        public static int Compare(string s1, string s2)
        {
            return Compare(SafeNativeMethods.GetThreadLocale(), s1, s2, 0);
        }

        /// <summary>
        ///  Compares two strings using the default locale with the given set of
        ///  string comparison flags.
        /// </summary>
        public static int Compare(string s1, string s2, int options)
        {
            return Compare(SafeNativeMethods.GetThreadLocale(), s1, s2, options);
        }

        /// <summary>
        ///  Compares two strings using a given locale and a given set of comparison
        ///  flags. If the two strings are of different lengths, they are compared up
        ///  to the length of the shortest one. If they are equal to that point, then
        ///  the return value will indicate that the longer string is greater. Notice
        ///  that if the return value is 0, the two strings are "equal" in the
        ///  collation sense, though not necessarily identical.
        ///  A <embedcode term='null'/> string always sorts before a non-null string. Two
        /// <embedcode term='null'/> strings are considered equal.
        ///  The <paramref name='options'/> parameter is a combination of zero or more of
        ///  the following flags: <code>IGNORECASE</code>, <code>IGNOREKANATYPE</code>,
        /// <code>IGNORENONSPACE</code>, <code>IGNORESYMBOLS</code>,
        /// <code>IGNOREWIDTH</code>, and <code>STRINGSORT</code>.
        /// </summary>
        public static int Compare(CultureInfo culture, string s1, string s2, int options)
        {
            return Compare(culture.LCID, s1, s2, options);
        }

        private static int Compare(int lcid, string s1, string s2, int options)
        {
            if (s1 == null)
            {
                return s2 == null ? 0 : -1;
            }

            if (s2 == null)
            {
                return 1;
            }

            return string.Compare(s1, s2, false, CultureInfo.CurrentCulture);
        }

        private int CompareKeys(string s1, string s2)
        {
            int result = Compare(lcid, s1, s2, options);
            return descending ? -result : result;
        }

        /// <summary>
        ///  Implementation of Quicksort algorithm. Within the outer <code>do</code>
        ///  loop, the method recurses on the shorter side and loops on the longer
        ///  side. This bounds the recursive depth by log2(n) in the worst case.
        ///  Otherwise, worst case recursive depth would be n.
        /// </summary>
        private void QuickSort(int left, int right)
        {
            do
            {
                int i = left;
                int j = right;
                string s = keys[(i + j) >> 1];
                do
                {
                    while (CompareKeys(keys[i], s) < 0)
                    {
                        i++;
                    }

                    while (CompareKeys(s, keys[j]) < 0)
                    {
                        j--;
                    }

                    if (i > j)
                    {
                        break;
                    }

                    if (i < j)
                    {
                        string key = keys[i];
                        keys[i] = keys[j];
                        keys[j] = key;
                        if (items != null)
                        {
                            object item = items[i];
                            items[i] = items[j];
                            items[j] = item;
                        }
                    }
                    i++;
                    j--;
                } while (i <= j);
                if (j - left <= right - i)
                {
                    if (left < j)
                    {
                        QuickSort(left, j);
                    }

                    left = i;
                }
                else
                {
                    if (i < right)
                    {
                        QuickSort(i, right);
                    }

                    right = j;
                }
            } while (left < right);
        }

        /// <summary>
        ///  Sorts an object array based on the string representation of the
        ///  elements. If the <code>items</code> parameter is not a string array, the
        /// <code>toString</code> method of each of the elements is called to
        ///  produce the string representation. The objects are then sorted by their
        ///  string representations using the default locale.
        /// </summary>
        public static void Sort(object[] items)
        {
            Sort(null, null, items, 0, ArrayLength(items), 0);
        }

        /// <summary>
        ///  Sorts a range in an object array based on the string representation of
        ///  the elements. If the <code>items</code> parameter is not a string array,
        ///  the <code>toString</code> method of each of the elements is called to
        ///  produce the string representation. The objects are then sorted by their
        ///  string representations using the default locale.
        /// </summary>
        public static void Sort(object[] items, int index, int count)
        {
            Sort(null, null, items, index, count, 0);
        }

        /// <summary>
        ///  Sorts a string array and an object array based on the elements of the
        ///  string array. The arrays are sorted using the default locale.
        /// </summary>
        public static void Sort(string[] keys, object[] items)
        {
            Sort(null, keys, items, 0, ArrayLength(items), 0);
        }

        /// <summary>
        ///  Sorts a range in a string array and a range in an object array based on
        ///  the elements of the string array. The arrays are sorted using the
        ///  default locale.
        /// </summary>
        public static void Sort(string[] keys, object[] items, int index, int count)
        {
            Sort(null, keys, items, index, count, 0);
        }

        /// <summary>
        ///  Sorts an object array based on the string representation of the
        ///  elements. If the <code>items</code> parameter is not a string array, the
        /// <code>toString</code> method of each of the elements is called to
        ///  produce the string representation. The objects are then sorted by their
        ///  string representations using the default locale and the given sorting
        ///  options.
        /// </summary>
        public static void Sort(object[] items, int options)
        {
            Sort(null, null, items, 0, ArrayLength(items), options);
        }

        /// <summary>
        ///  Sorts a range in an object array based on the string representation of
        ///  the elements. If the <code>items</code> parameter is not a string array,
        ///  the <code>toString</code> method of each of the elements is called to
        ///  produce the string representation. The objects are then sorted by their
        ///  string representations using the default locale and the given sorting
        ///  options.
        /// </summary>
        public static void Sort(object[] items, int index, int count, int options)
        {
            Sort(null, null, items, index, count, options);
        }

        /// <summary>
        ///  Sorts a string array and an object array based on the elements of the
        ///  string array. The arrays are sorted using the default locale and the
        ///  given sorting options.
        /// </summary>
        public static void Sort(string[] keys, object[] items, int options)
        {
            Sort(null, keys, items, 0, ArrayLength(items), options);
        }

        /// <summary>
        ///  Sorts a range in a string array and a range in an object array based on
        ///  the elements of the string array. The arrays are sorted using the
        ///  default locale and the given sorting options.
        /// </summary>
        public static void Sort(string[] keys, object[] items, int index, int count, int options)
        {
            Sort(null, keys, items, index, count, options);
        }

        /// <summary>
        ///  Sorts an object array based on the string representation of the
        ///  elements. If the <code>items</code> parameter is not a string array, the
        /// <code>toString</code> method of each of the elements is called to
        ///  produce the string representation. The objects are then sorted by their
        ///  string representations using the given locale and the given sorting
        ///  options.
        /// </summary>
        public static void Sort(CultureInfo culture, object[] items, int options)
        {
            Sort(culture, null, items, 0, ArrayLength(items), options);
        }

        /// <summary>
        ///  Sorts a range in an object array based on the string representation of
        ///  the elements. If the <code>items</code> parameter is not a string array,
        ///  the <code>toString</code> method of each of the elements is called to
        ///  produce the string representation. The objects are then sorted by their
        ///  string representations using the given locale and the given sorting
        ///  options.
        /// </summary>
        public static void Sort(CultureInfo culture, object[] items, int index, int count, int options)
        {
            Sort(culture, null, items, index, count, options);
        }

        /// <summary>
        ///  Sorts a string array and an object array based on the elements of the
        ///  string array. The arrays are sorted using the given locale and the
        ///  given sorting options.
        /// </summary>
        public static void Sort(CultureInfo culture, string[] keys, object[] items, int options)
        {
            Sort(culture, keys, items, 0, ArrayLength(items), options);
        }

        /// <summary>
        ///  Sorts a range in a string array and a range in an object array based on
        ///  the elements of the string array. Elements in the <code>keys</code>
        ///  array specify the sort keys for corresponding elements in the
        /// <code>items</code> array. The range of elements given by the
        /// <code>index</code> and <code>count</code> parameters is sorted in both
        ///  arrays according to the given locale and sorting options.
        ///  If the <code>keys</code> parameter is <code>null</code>, the sort keys
        ///  are instead computed by calling the <code>toString</code> method of each
        ///  element in the <code>items</code> array.
        /// <code>null</code> keys always sort before a non-null keys.
        ///  The <code>options</code> parameter is a combination of zero or more of
        ///  the following flags: <code>IGNORECASE</code>, <code>IGNOREKANATYPE</code>,
        /// <code>IGNORENONSPACE</code>, <code>IGNORESYMBOLS</code>,
        /// <code>IGNOREWIDTH</code>, <code>STRINGSORT</code>, and
        /// <code>DESCENDING</code>.
        /// </summary>
        public static void Sort(CultureInfo culture, string[] keys, object[] items, int index, int count, int options)
        {
            // keys and items have to be the same length
            //
            if ((items == null)
                || ((keys != null) && (keys.Length != items.Length)))
            {
                throw new ArgumentException(string.Format(SR.ArraysNotSameSize,
                                                                   "keys",
                                                                   "items"));
            }

            if (count > 1)
            {
                StringSorter sorter = new StringSorter(culture, keys, items, options);
                sorter.QuickSort(index, index + count - 1);
            }
        }
    }
}
