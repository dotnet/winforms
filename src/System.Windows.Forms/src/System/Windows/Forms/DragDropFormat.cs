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
        private STGMEDIUM _medium;

        /// <summary>
        ///  Represents a private drag and drop storage medium used for data transfer.
        /// </summary>
        public STGMEDIUM Medium
        {
            get
            {
                if (_release)
                {
                    // Handle when the data object retains ownership of the storage medium.
                    if (DragDropHelper.CopyStgMedium(ref _medium, _formatEtc, out STGMEDIUM _mediumOut))
                    {
                        return _mediumOut;
                    }
                    else
                    {
                        Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"Copy storage medium unsuccessful {_formatName}");
                        return default;
                    }
                }
                else
                {
                    return _medium;
                }
            }
        }

        public DragDropFormat(FORMATETC pFormatetc, STGMEDIUM pMedium, bool fRelease)
        {
            _formatName = DataFormats.GetFormat(pFormatetc.cfFormat).Name;
            _formatEtc = pFormatetc;
            _release = fRelease;

            if (_release)
            {
                _medium = pMedium;
            }
            else
            {
                // Handle when the caller retains ownership of the storage medium.
                if (DragDropHelper.CopyStgMedium(ref pMedium, _formatEtc, out STGMEDIUM _mediumCopy))
                {
                    _medium = _mediumCopy;
                }
                else
                {
                    Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"Copy storage medium unsuccessful {_formatName}");
                    _medium = default;
                }
            }
        }

        ~DragDropFormat()
        {
            Ole32.ReleaseStgMedium(ref _medium);
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"DragDropFormat {_formatName} storage medium released.");
        }
    }
}
