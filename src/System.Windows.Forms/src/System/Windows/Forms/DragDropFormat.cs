// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a private drag and drop format used for IDataObject::SetData and IDataObject::GetData implementations.
    /// </summary>
    internal class DragDropFormat
    {
        private readonly bool _release;
        private readonly string _formatName;
        private FORMATETC _formatEtc;
        private STGMEDIUM _mediumIn;
        private STGMEDIUM _mediumOut;

        /// <summary>
        ///  Represents a private drag and drop storage medium.
        /// </summary>
        public STGMEDIUM Medium
        {
            get
            {
                if (_release)
                {
                    // Handle when the data object retains ownership of the storage medium and return a copy.
                    if (DragDropHelper.CopyDragDropStgMedium(ref _mediumIn, _formatEtc, out _mediumOut))
                    {
                        Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "DragDropFormat storage medium copied.");
                        return _mediumOut;
                    }
                    else
                    {
                        return default;
                    }
                }
                else
                {
                    return _mediumIn;
                }
            }
        }

        public DragDropFormat(FORMATETC pFormatetc, STGMEDIUM pMedium, bool fRelease)
        {
            _formatName = DataFormats.GetFormat(pFormatetc.cfFormat).Name;

            Debug.Assert(pFormatetc.ptd.Equals(IntPtr.Zero), "DragDropFormat constructur received a non-NULL target device pointer.");
            Debug.Assert(DragDropHelper.s_formats.Contains(_formatName), "DragDropFormat constructor received an incompatible clipboard format.");
            Debug.Assert(DragDropHelper.s_tymeds.Contains(pMedium.tymed), "DragDropFormat constructor received an incompatible storage medium type.");
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"DragDropFormat {_formatName} created");
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"DragDropFormat pMedium.tymed {pMedium.tymed}");
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"DragDropFormat fRelease {fRelease}");

            _formatEtc = pFormatetc;
            _release = fRelease;

            if (_release)
            {
                _mediumIn = pMedium;
            }
            else
            {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"DragDropFormat fRelease {fRelease}");

                // Handle when the caller retains ownership of the storage medium. We must copy the medium if we want to
                // keep it beyond the IDataObject::SetData method call.
                if (DragDropHelper.CopyDragDropStgMedium(ref pMedium, pFormatetc, out STGMEDIUM _mediumCopy))
                {
                    _mediumIn = _mediumCopy;
                }
            }
        }

        ~DragDropFormat()
        {
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"DragDropFormat {_formatName} destroyed");

            if (_release)
            {
                Ole32.ReleaseStgMedium(ref _mediumOut);
            }
            else
            {
                Ole32.ReleaseStgMedium(ref _mediumIn);
            }

            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"DragDropFormat {_formatName} storage medium released.");
        }
    }
}
