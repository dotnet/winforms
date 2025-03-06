// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Collections.Generic;

/// <summary>
///  Helpers for working with non generic collections.
/// </summary>
internal static class CollectionHelper
{
    /// <summary>
    ///  Copies to arrays matching `HashTable.CopyTo`'s behavior.
    /// </summary>
    public static void HashtableCopyTo<TKey, TValue>(this IDictionary<TKey, TValue> source, Array target, int index)
    {
        ArgumentNullException.ThrowIfNull(target);

        if (target.Rank != 1)
            throw new ArgumentException(SR.Arg_RankMultiDimNotSupported, nameof(target));
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)index, (uint)target.Length, nameof(index));
        if (target.GetLowerBound(0) != 0)
            throw new ArgumentException(SR.Arg_NonZeroLowerBound, nameof(target));
        if (target.Length - index < source.Count)
            throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

        if (target is KeyValuePair<TKey, TValue>[] pairsTarget)
        {
            foreach (KeyValuePair<TKey, TValue> kvp in source)
            {
                pairsTarget[index++] = kvp;
            }
        }
        else if (target is DictionaryEntry[] dictionaryTarget)
        {
            foreach (KeyValuePair<TKey, TValue> kvp in source)
            {
                if (kvp.Key is not null)
                {
                    dictionaryTarget[index++] = new DictionaryEntry(kvp.Key, kvp.Value);
                }
            }
        }
        else
        {
            if (target is not object[] objects)
            {
                throw new ArgumentException(SR.Argument_IncompatibleArrayType);
            }

            try
            {
                foreach (KeyValuePair<TKey, TValue> kvp in source)
                {
                    if (kvp.Key is not null)
                    {
                        objects[index++] = new DictionaryEntry(kvp.Key, kvp.Value);
                    }
                }
            }
            catch (ArrayTypeMismatchException)
            {
                throw new ArgumentException(SR.Argument_IncompatibleArrayType);
            }
        }
    }
}
