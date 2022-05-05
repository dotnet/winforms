// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices.ComTypes;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a private format used for data transfer by the drag-and-drop helpers.
    /// </summary>
    internal class DragDropFormat : IDisposable
    {
        private short _cfFormat;
        private STGMEDIUM _medium;

        public STGMEDIUM Medium => _medium;

        /// <summary>
        ///  Initializes a new instance of the <see cref="DragDropFormat"/> class using
        ///  the specified format, storage medium, and owner.
        /// </summary>
        public DragDropFormat(short cfFormat, STGMEDIUM pMedium, bool fRelease)
        {
            _cfFormat = cfFormat;
            _medium = HandleOwner(cfFormat, pMedium, fRelease);
        }

        /// <summary>
        ///  Returns a copy of the storage mediumn in this instance.
        /// </summary>
        public STGMEDIUM GetData()
        {
            return CopyMedium(_cfFormat, _medium);
        }

        /// <summary>
        ///  Refreshes the storage medium in this instance.
        /// </summary>
        public void RefreshData(short cfFormat, STGMEDIUM pMedium, bool fRelease)
        {
            if (!cfFormat.Equals(_cfFormat))
            {
                return;
            }

            ReleaseMedium(_medium);
            _cfFormat = cfFormat;
            _medium = HandleOwner(cfFormat, pMedium, fRelease);
            return;
        }

        /// <summary>
        ///  Handles whether the data object or the caller owns the storage medium.
        /// </summary>
        private static STGMEDIUM HandleOwner(short cfFormat, STGMEDIUM pMedium, bool fRelease)
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
        private static STGMEDIUM CopyMedium(short cfFormat, STGMEDIUM medium)
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
        private static void ReleaseMedium(STGMEDIUM medium)
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
