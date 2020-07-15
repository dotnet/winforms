// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Thread safe cache of <see cref="Gdi32.HFONT"/> objects created from <see cref="Font"/> objects.
    /// </summary>
    /// <remarks>
    ///  This adds a slight overhead (112 bytes) to creating the <see cref="Gdi32.HFONT"/>, but saves 56 bytes and 98%+
    ///  of the time on each cache request that hits (taking less than 1 us as opposed to 50-100+ us).
    ///
    ///  This cache is optimized for retrieval speed and limiting the number of unused GDI handles we're caching while
    ///  hopefully handling the majority of application use cases. There is a limit of 65K GDI handles system wide and
    ///  10K (default) per process.
    /// </remarks>
    internal sealed partial class FontCache : IDisposable
    {
        private readonly LinkedList<Data> _list = new LinkedList<Data>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private int _count;
        private bool _clean;
        private readonly int _softLimit;
        private readonly int _hardLimit;

        // Note: These defaults are according to the ones in GDI+ but those are not necessarily the same as the system
        // default font.  The GetSystemDefaultHFont() method should be used if needed.
        private const string DefaultFaceName = "Microsoft Sans Serif";
        private const byte True = 1;
        private const byte False = 0;

        /// <summary>
        ///  Create a <see cref="FontCache"/> with the specified collection limits.
        /// </summary>
        /// <param name="softLimit">
        ///  If there are more than this amount of fonts in the cache, collection of garbage collected, unused fonts
        ///  will happen on the next <see cref="Gdi32.HFONT"/> retrieval.
        /// </param>
        /// <param name="hardLimit">
        ///  If there are more than this amount of fonts in the cache, collection of enough unused fonts (garbage
        ///  collected or not) to bring the total under this limit will happen on the next <see cref="Gdi32.HFONT"/>
        ///  retrieval.
        /// </param>
        public FontCache(int softLimit = 20, int hardLimit = 40)
        {
            Debug.Assert(softLimit > 0 && hardLimit > 0);
            Debug.Assert(softLimit <= hardLimit);

            _softLimit = softLimit;
            _hardLimit = hardLimit;
        }

        /// <summary>
        ///  Gets a ref-counting scope containing the <see cref="Gdi32.HFONT"/> that matches the specified
        ///  <paramref name="font"/> and <paramref name="quality"/>. The scope MUST be disposed to release the ref
        ///  count accurately. Use the result in a using statement to avoid leaking fonts.
        /// </summary>
        public FontScope GetHFONT(Font font, Gdi32.QUALITY quality)
        {
            if (font is null)
                throw new ArgumentNullException(nameof(font));

            if (_clean)
            {
                _lock.EnterWriteLock();
                try
                {
                    Clean();
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }

            _lock.EnterReadLock();
            try
            {
                if (Find(font, quality, out FontScope scope))
                {
                    return scope;
                }
            }
            finally
            {
                _lock.ExitReadLock();
            }

            _lock.EnterWriteLock();
            try
            {
                return Add(font, quality);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            bool Find(Font font, Gdi32.QUALITY quality, out FontScope scope)
            {
                scope = default;
                LinkedListNode<Data>? node = _list.First;

                while (node != null)
                {
                    Data data = node.Value;

                    if (!data.Font.TryGetTarget(out Font? currentFont))
                    {
                        // Found a dead node, mark ourselves for cleaning
                        _clean = true;
                    }
                    else if (font == currentFont && quality == data.Quality)
                    {
                        // Current node is still alive
                        scope = new FontScope(data);
                        return true;
                    }

                    node = node.Next;
                }

                return false;
            }

            FontScope Add(Font font, Gdi32.QUALITY quality)
            {
                // Need to try and find the node again as it may have been created on another thread.
                if (Find(font, quality, out FontScope scope))
                {
                    return scope;
                }

                Data node = new Data(font, quality);
                _list.AddFirst(node);
                _count++;
                if (_count > _softLimit)
                {
                    _clean = true;
                }
                return new FontScope(node);
            }

            void Clean()
            {
                if (!_clean)
                {
                    return;
                }

                _clean = false;

                // Get rid of all collected Font nodes where the HFONT is unused

                LinkedListNode<Data>? node = _list.First;

                while (node != null)
                {
                    Data data = node.Value;
                    LinkedListNode<Data>? nextNode = node.Next;

                    if (!data.Font.TryGetTarget(out Font? _))
                    {
                        if (data.RefCount == 0)
                        {
                            // Font was garbage collected, and the HFONT isn't currently used
                            Gdi32.DeleteObject(data.HFONT);
                            _list.Remove(node);
                            _count--;
                        }
                        else
                        {
                            // Mark ourselves for clean again so we can get this later
                            _clean = true;
                        }
                    }

                    node = nextNode;
                }

                // If the count is over hard limit, expunge excess entries from the end of the list
                // where the ref count is 0.

                int overage = _count - _hardLimit;

                if (overage <= 0)
                {
                    return;
                }

                node = _list.Last;

                while (node != null && overage > 0)
                {
                    Data data = node.Value;
                    LinkedListNode<Data>? priorNode = node.Previous;

                    if (data.RefCount == 0)
                    {
                        // HFONT isn't currently in use, remove the node.
                        Gdi32.DeleteObject(data.HFONT);
                        _list.Remove(node);
                        _count--;
                        overage--;
                    }

                    node = priorNode;
                }

                _clean |= _count > _softLimit;
            }
        }

        /// <summary>
        ///  Contructs a WindowsFont object from an existing System.Drawing.Font object (GDI+), based on the screen dc
        ///  MapMode and resolution (normally: MM_TEXT and 96 dpi).
        /// </summary>
        private static Gdi32.HFONT FromFont(Font font, Gdi32.QUALITY fontQuality = Gdi32.QUALITY.DEFAULT)
        {
            string familyName = font.FontFamily.Name;

            // Strip vertical-font mark from the name if needed.
            if (familyName != null && familyName.Length > 1 && familyName[0] == '@')
            {
                familyName = familyName.Substring(1);
            }

            // Note: Creating the WindowsFont from Font using a LOGFONT structure from GDI+ (Font.ToLogFont(logFont))
            // may sound like a better choice (more accurate) for doing this but tests show that is not the case (see
            // WindowsFontTests test suite), the results are the same.  Also, that approach has some issues when the
            // Font is created in a different application domain since the LOGFONT cannot be marshalled properly.

            // Now, creating it using the Font.SizeInPoints makes it GraphicsUnit-independent.

            Debug.Assert(font.SizeInPoints > 0.0f, "size has a negative value.");

            // Get the font height from the specified size.  size is in point units and height in logical
            // units (pixels when using MM_TEXT) so we need to make the conversion using the number of
            // pixels per logical inch along the screen height. (1 point = 1/72 inch.)
            int pixelsY = (int)Math.Ceiling(DpiHelper.DeviceDpi * font.SizeInPoints / 72);

            // The lfHeight represents the font cell height (line spacing) which includes the internal
            // leading; we specify a negative size value (in pixels) for the height so the font mapper
            // provides the closest match for the character height rather than the cell height (MSDN).

            User32.LOGFONTW logFont = new User32.LOGFONTW()
            {
                lfHeight = -pixelsY,
                lfCharSet = font.GdiCharSet,
                lfOutPrecision = Gdi32.OUT_PRECIS.TT,
                lfQuality = fontQuality,
                lfWeight = (font.Style & FontStyle.Bold) == FontStyle.Bold ? Gdi32.FW.BOLD : Gdi32.FW.NORMAL,
                lfItalic = (font.Style & FontStyle.Italic) == FontStyle.Italic ? True : False,
                lfUnderline = (font.Style & FontStyle.Underline) == FontStyle.Underline ? True : False,
                lfStrikeOut = (font.Style & FontStyle.Strikeout) == FontStyle.Strikeout ? True : False,
                FaceName = familyName
            };

            if (logFont.FaceName.Length == 0)
            {
                logFont.FaceName = DefaultFaceName;
            }

            Gdi32.HFONT hfont = Gdi32.CreateFontIndirectW(ref logFont);

            if (hfont.IsNull)
            {
                logFont.FaceName = DefaultFaceName;
                logFont.lfOutPrecision = Gdi32.OUT_PRECIS.TT_ONLY; // TrueType only.

                hfont = Gdi32.CreateFontIndirectW(ref logFont);
            }

            return hfont;
        }

        public void Dispose()
        {
            _lock.EnterWriteLock();
            try
            {
                var node = _list.First;
                while (node != null)
                {
                    var nextNode = node.Next;
                    Gdi32.DeleteObject(node.Value.HFONT);
                    _list.Remove(node);
                    node = nextNode;
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}
