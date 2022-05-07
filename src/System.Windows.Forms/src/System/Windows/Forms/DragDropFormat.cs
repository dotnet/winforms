// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
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
            return CopyData(_cfFormat, _medium);
        }

        /// <summary>
        ///  Refreshes the storage medium in this instance.
        /// </summary>
        public void RefreshData(short cfFormat, STGMEDIUM pMedium, bool fRelease)
        {
            ReleaseMedium(_medium);
            _cfFormat = cfFormat;
            _medium = HandleOwner(cfFormat, pMedium, fRelease);
        }

        /// <summary>
        /// Copies a given storage medium.
        /// </summary>
        /// <returns>
        ///  A copy of <paramref name="mediumSource"/>.
        /// </returns>
        private static STGMEDIUM CopyData(short cfFormat, STGMEDIUM mediumSource)
        {
            STGMEDIUM mediumDestination = new();

            try
            {
                switch (mediumSource.tymed)
                {
                    case TYMED.TYMED_HGLOBAL:
                    case TYMED.TYMED_FILE:
                    case TYMED.TYMED_ENHMF:
                    case TYMED.TYMED_GDI:
                    case TYMED.TYMED_MFPICT:

                        mediumDestination.unionmember = Ole32.OleDuplicateData(
                            mediumSource.unionmember,
                            cfFormat,
                            Kernel32.GMEM.MOVEABLE | Kernel32.GMEM.DDESHARE | Kernel32.GMEM.ZEROINIT);
                        if (mediumDestination.unionmember == IntPtr.Zero)
                        {
                            return default;
                        }

                        break;

                    case TYMED.TYMED_ISTORAGE:
                    case TYMED.TYMED_ISTREAM:

                        mediumDestination.unionmember = mediumSource.unionmember;
                        Marshal.AddRef(mediumSource.unionmember);
                        break;

                    default:
                    case TYMED.TYMED_NULL:

                        mediumDestination.unionmember = IntPtr.Zero;
                        break;
                }

                mediumDestination.tymed = mediumSource.tymed;
                mediumDestination.pUnkForRelease = mediumSource.pUnkForRelease;

                if (mediumSource.pUnkForRelease is not null)
                {
                    Marshal.GetIUnknownForObject(mediumSource.pUnkForRelease);
                }

                return mediumDestination;
            }
            catch
            {
                Ole32.ReleaseStgMedium(ref mediumDestination);
                return default;
            }
        }

        /// <summary>
        ///  Handles whether the data object or the caller owns the storage medium.
        /// </summary>
        private static STGMEDIUM HandleOwner(short cfFormat, STGMEDIUM pMedium, bool fRelease)
        {
            return fRelease ? pMedium : CopyData(cfFormat, pMedium);
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
