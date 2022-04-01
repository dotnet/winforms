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
    ///  Represents a drag and drop private format.
    /// </summary>
    internal class DragDropFormat
    {
        private readonly bool _release;
        private readonly string _formatName;
        private FORMATETC _formatEtc;
        private STGMEDIUM _mediumIn;
        private STGMEDIUM _mediumOut;

        /// <summary>
        ///  Represents a drag and drop storage medium.
        /// </summary>
        public STGMEDIUM Medium
        {
            get
            {
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
        }

        public DragDropFormat(FORMATETC pFormatetc, STGMEDIUM pMedium, bool fRelease)
        {
            _formatName = DataFormats.GetFormat(pFormatetc.cfFormat).Name;
            Debug.Assert(DragDropHelper.s_formats.Contains(_formatName), "DragDropFormat constructor received an incompatible clipboard format.");
            Debug.Assert(DragDropHelper.s_tymeds.Contains(pMedium.tymed), "DragDropFormat constructor received an incompatible storage medium type.");

            _formatEtc = pFormatetc;
            _mediumIn = pMedium;
            _release = fRelease;
        }

        ~DragDropFormat()
        {
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "DragDropFormat destroyed");

            if (_release)
            {
                // Release the copied storage medium.
                Ole32.ReleaseStgMedium(ref _mediumOut);
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"DragDropFormat {_formatName} storage medium released.");
            }
        }
    }
}
