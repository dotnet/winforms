// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using static Interop;
using static Interop.Ole32;

namespace System.Windows.Forms.Primitives.Tests.Interop.Oleaut32
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    internal class MockAxHost
    {
        private static Guid ipictureDisp_Guid = typeof(IPictureDisp).GUID;

        public MockAxHost(string clsidString)
        {
        }

        public static IPictureDisp GetIPictureDispFromPicture(Image image)
        {
            PICTDESC desc = GetPICTDESCFromPicture(image);
            return (IPictureDisp)OleCreatePictureIndirect(ref desc, ref ipictureDisp_Guid, fOwn: BOOL.TRUE);
        }

        private static PICTDESC GetPICTDESCFromPicture(Image image)
        {
            if (image is Bitmap bmp)
            {
                return PICTDESC.FromBitmap(bmp);
            }

            if (image is Metafile mf)
            {
                return PICTDESC.FromMetafile(mf);
            }

            throw new ArgumentException("AXUnknownImage", nameof(image));
        }
    }
}
