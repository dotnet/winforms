// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices.ComTypes;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents an arbitrary private drag-and-drop format used for data transfer by the drag-image manager.
    /// </summary>
    internal class DragDropFormat : IDisposable
    {
        private short _cfFormat;
        private STGMEDIUM _medium;

        public STGMEDIUM Medium => _medium;

        public DragDropFormat(short cfFormat, STGMEDIUM pMedium, bool fRelease)
        {
            _cfFormat = cfFormat;
            _medium = HandleMedium(cfFormat, pMedium, fRelease);
        }

        /// <summary>
        ///  Returns a copy of the storage medium.
        /// </summary>
        public STGMEDIUM GetMedium()
        {
            return CopyMedium(_cfFormat, _medium);
        }

        /// <summary>
        ///  Updates the storage medium.
        /// </summary>
        public bool UpdateMedium(short cfFormat, STGMEDIUM pMedium, bool fRelease)
        {
            if (!cfFormat.Equals(_cfFormat))
            {
                return false;
            }

            ReleaseMedium(_medium);
            _cfFormat = cfFormat;
            _medium = HandleMedium(cfFormat, pMedium, fRelease);
            return true;
        }

        /// <summary>
        ///  Handles whether the data object or the caller owns the storage medium.
        /// </summary>
        private STGMEDIUM HandleMedium(short cfFormat, STGMEDIUM pMedium, bool fRelease)
        {
            STGMEDIUM medium;

            if (fRelease)
            {
                // Handle when the data object owns the storage medium.
                medium = pMedium;
            }
            else
            {
                // Handle when the caller owns the storage medium.
                medium = CopyMedium(cfFormat, pMedium);
            }

            return medium;
        }

        /// <summary>
        ///  Returns a copy of the specified storage medium.
        /// </summary>
        private STGMEDIUM CopyMedium(short cfFormat, STGMEDIUM medium)
        {
            if (DragDropHelper.CopyMedium(cfFormat, ref medium, out STGMEDIUM mediumCopy))
            {
                return mediumCopy;
            }

            return default;
        }

        /// <summary>
        ///  Frees the specified the storage medium.
        /// </summary>
        private void ReleaseMedium(STGMEDIUM medium)
        {
            Ole32.ReleaseStgMedium(ref medium);
        }

        public void Dispose()
        {
            ReleaseMedium(_medium);
            GC.SuppressFinalize(this);
        }

        ~DragDropFormat()
        {
            Dispose();
        }
    }
}
