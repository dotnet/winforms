// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Layout;

internal static partial class LayoutUtils
{
    ///  MeasureTextCache
    ///  3000 character strings take 9 seconds to load the form
    public sealed class MeasureTextCache
    {
        // the number of preferred sizes to store
        private const int MaxCacheSize = 6;

        // the next place in the ring buffer to store a preferred size
        private int _nextCacheEntry = -1;

        // MRU of size MaxCacheSize
        private PreferredSizeCache[]? _sizeCacheList;
        private Size _unconstrainedPreferredSize = s_invalidSize;

        ///  GetTextSize
        ///  Given constraints, format flags a font and text, determine the size of the string
        ///  employs an MRU of the last several constraints passed in via a ring-buffer of size MaxCacheSize.
        ///  Assumes Text and TextFormatFlags are the same, if either were to change, a call to
        ///  InvalidateCache should be made
        public Size GetTextSize(string? text, Font? font, Size proposedConstraints, TextFormatFlags flags)
        {
            if (!TextRequiresWordBreak(text, font, proposedConstraints, flags))
            {
                // Text fits within proposed width

                // IF we're here, this means we've got text that can fit into the proposedConstraints
                // without wrapping. We've determined this because our

                // as a side effect of calling TextRequiresWordBreak,
                // unconstrainedPreferredSize is set.
                return _unconstrainedPreferredSize;
            }
            else
            {
                // Text does NOT fit within proposed width - requires WordBreak

                // IF we're here, this means that the wrapping width is smaller
                // than our max width. For example: we measure the text with infinite
                // bounding box and we determine the width to fit all the characters
                // to be 200 px wide. We would come here only for proposed widths less
                // than 200 px.

                // Create our ring buffer if we don't have one
                _sizeCacheList ??= new PreferredSizeCache[MaxCacheSize];

                // check the existing constraints from previous calls
                foreach (PreferredSizeCache sizeCache in _sizeCacheList)
                {
                    if (sizeCache.ConstrainingSize == proposedConstraints)
                    {
                        return sizeCache.PreferredSize;
                    }
                    else if ((sizeCache.ConstrainingSize.Width == proposedConstraints.Width)
                              && (sizeCache.PreferredSize.Height <= proposedConstraints.Height))
                    {
                        // Caching a common case where the width matches perfectly, and the stored preferred height
                        // is smaller or equal to the constraining size.
                        //        prefSize = GetPreferredSize(w,Int32.MaxValue);
                        //        prefSize = GetPreferredSize(w,prefSize.Height);

                        return sizeCache.PreferredSize;
                    }

                    //
                }

                // if we've gotten here, it means we don't have a cache entry, therefore
                // we should add a new one in the next available slot.
                Size prefSize = TextRenderer.MeasureText(text, font, proposedConstraints, flags);
                _nextCacheEntry = (_nextCacheEntry + 1) % MaxCacheSize;
                _sizeCacheList[_nextCacheEntry] = new PreferredSizeCache(proposedConstraints, prefSize);

                return prefSize;
            }
        }

        ///  InvalidateCache
        ///  Clears out the cached values, should be called whenever Text, Font or a TextFormatFlag has changed
        public void InvalidateCache()
        {
            _unconstrainedPreferredSize = s_invalidSize;
            _sizeCacheList = null;
        }

        ///  TextRequiresWordBreak
        ///  If you give the text all the space in the world it wants, then there should be no reason
        ///  for it to break on a word. So we find out what the unconstrained size is (Int32.MaxValue, Int32.MaxValue)
        ///  for a string - eg. 35, 13. If the size passed in has a larger width than 35, then we know that
        ///  the WordBreak flag is not necessary.
        public bool TextRequiresWordBreak(string? text, Font? font, Size size, TextFormatFlags flags)
        {
            // if the unconstrained size of the string is larger than the proposed width
            // we need the word break flag, otherwise we don't, its a perf hit to use it.
            return GetUnconstrainedSize(text, font, flags).Width > size.Width;
        }

        ///  GetUnconstrainedSize
        ///  Gets the unconstrained (Int32.MaxValue, Int32.MaxValue) size for a piece of text
        private Size GetUnconstrainedSize(string? text, Font? font, TextFormatFlags flags)
        {
            if (_unconstrainedPreferredSize == s_invalidSize)
            {
                // we also investigated setting the SingleLine flag, however this did not yield as much benefit as the word break
                // and had possibility of causing internationalization issues.

                flags &= ~TextFormatFlags.WordBreak; // rip out the wordbreak flag
                _unconstrainedPreferredSize = TextRenderer.MeasureText(text, font, s_maxSize, flags);
            }

            return _unconstrainedPreferredSize;
        }
    }
}
