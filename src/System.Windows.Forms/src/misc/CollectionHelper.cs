namespace System.Collections.Generic
{
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
            if (index < 0 || (uint)index > (uint)target.Length)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_NeedNonNegNum);
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
                object[]? objects = target as object[];
                if (objects is null)
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
}
