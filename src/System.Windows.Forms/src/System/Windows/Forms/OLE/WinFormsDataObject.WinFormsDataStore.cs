// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Private.Windows.Core.OLE;
using CoreSR = System.Private.Windows.Core.Resources.SR;

namespace System.Windows.Forms;

internal partial class WinFormsDataObject
{
    internal sealed class WinFormsDataStore : DataStore
    {
        public override void SetData(string format, bool autoConvert, object? data)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                ArgumentNullException.ThrowIfNull(format);
                throw new ArgumentException(CoreSR.DataObjectWhitespaceEmptyFormatNotAllowed, nameof(format));
            }

            // We do not have proper support for Dibs, so if the user explicitly asked
            // for Dib and provided a Bitmap object we can't convert. Instead, publish as an HBITMAP
            // and let the system provide the conversion for us.
            if (data is Bitmap && format.Equals(DesktopDataFormats.DibConstant))
            {
                format = autoConvert ? DesktopDataFormats.BitmapConstant : throw new NotSupportedException(SR.DataObjectDibNotSupported);
            }

            _mappedData[format] = new DataStoreEntry(data, autoConvert);
        }
    }
}
