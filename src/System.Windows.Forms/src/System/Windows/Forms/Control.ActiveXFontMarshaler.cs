// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class Control
    {
        /// <summary>
        ///  This is a marshaler object that knows how to marshal IFont to Font and back.
        /// </summary>
        private class ActiveXFontMarshaler : ICustomMarshaler
        {
            private static ActiveXFontMarshaler s_instance;

            public void CleanUpManagedData(object obj)
            {
            }

            public void CleanUpNativeData(IntPtr pObj) => Marshal.Release(pObj);

            internal static ICustomMarshaler GetInstance(string cookie)
            {
                return s_instance ??= new ActiveXFontMarshaler();
            }

            public int GetNativeDataSize() => -1; // not a value type, so use -1

            public IntPtr MarshalManagedToNative(object obj)
            {
                Font font = (Font)obj;
                User32.LOGFONTW logFont = User32.LOGFONTW.FromFont(font);
                var fontDesc = new Oleaut32.FONTDESC
                {
                    cbSizeOfStruct = (uint)Marshal.SizeOf<Oleaut32.FONTDESC>(),
                    lpstrName = font.Name,
                    cySize = (long)(font.SizeInPoints * 10000),
                    sWeight = (short)logFont.lfWeight,
                    sCharset = logFont.lfCharSet,
                    fItalic = font.Italic.ToBOOL(),
                    fUnderline = font.Underline.ToBOOL(),
                    fStrikethrough = font.Strikeout.ToBOOL(),
                };
                Guid iid = typeof(Ole32.IFont).GUID;
                Ole32.IFont oleFont = Oleaut32.OleCreateFontIndirect(ref fontDesc, ref iid);
                IntPtr pFont = Marshal.GetIUnknownForObject(oleFont);

                int hr = Marshal.QueryInterface(pFont, ref iid, out IntPtr pIFont);

                Marshal.Release(pFont);

                if (((HRESULT)hr).Failed())
                {
                    Marshal.ThrowExceptionForHR(hr);
                }

                return pIFont;
            }

            public object MarshalNativeToManaged(IntPtr pObj)
            {
                Ole32.IFont nativeFont = (Ole32.IFont)Marshal.GetObjectForIUnknown(pObj);
                try
                {
                    return Font.FromHfont(nativeFont.hFont);
                }
                catch (Exception e) when (!ClientUtils.IsCriticalException(e))
                {
                    return DefaultFont;
                }
            }
        }
    }
}
