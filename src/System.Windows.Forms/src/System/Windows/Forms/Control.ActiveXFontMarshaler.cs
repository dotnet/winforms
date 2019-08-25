// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    public partial class Control
    {
        /// <summary>
        ///  This is a marshaler object that knows how to marshal IFont to Font
        ///  and back.
        /// </summary>
        private class ActiveXFontMarshaler : ICustomMarshaler
        {
            private static ActiveXFontMarshaler s_instance;

            public void CleanUpManagedData(object obj)
            {
            }

            public void CleanUpNativeData(IntPtr pObj)
            {
                Marshal.Release(pObj);
            }

            internal static ICustomMarshaler GetInstance(string cookie)
            {
                if (s_instance == null)
                {
                    s_instance = new ActiveXFontMarshaler();
                }
                return s_instance;
            }

            public int GetNativeDataSize()
                => -1; // not a value type, so use -1

            public IntPtr MarshalManagedToNative(object obj)
            {
                Font font = (Font)obj;
                NativeMethods.tagFONTDESC fontDesc = new NativeMethods.tagFONTDESC();
                NativeMethods.LOGFONTW logFont = NativeMethods.LOGFONTW.FromFont(font);

                fontDesc.lpstrName = font.Name;
                fontDesc.cySize = (long)(font.SizeInPoints * 10000);
                fontDesc.sWeight = (short)logFont.lfWeight;
                fontDesc.sCharset = logFont.lfCharSet;
                fontDesc.fItalic = font.Italic;
                fontDesc.fUnderline = font.Underline;
                fontDesc.fStrikethrough = font.Strikeout;

                Guid iid = typeof(UnsafeNativeMethods.IFont).GUID;

                UnsafeNativeMethods.IFont oleFont = UnsafeNativeMethods.OleCreateFontIndirect(fontDesc, ref iid);
                IntPtr pFont = Marshal.GetIUnknownForObject(oleFont);

                int hr = Marshal.QueryInterface(pFont, ref iid, out IntPtr pIFont);

                Marshal.Release(pFont);

                if (NativeMethods.Failed(hr))
                {
                    Marshal.ThrowExceptionForHR(hr);
                }

                return pIFont;
            }

            public object MarshalNativeToManaged(IntPtr pObj)
            {
                UnsafeNativeMethods.IFont nativeFont = (UnsafeNativeMethods.IFont)Marshal.GetObjectForIUnknown(pObj);
                IntPtr hfont = nativeFont.GetHFont();

                Font font;
                try
                {
                    font = Font.FromHfont(hfont);
                }
                catch (Exception e)
                {
                    if (ClientUtils.IsSecurityOrCriticalException(e))
                    {
                        throw;
                    }

                    font = DefaultFont;
                }

                return font;
            }
        }
    }
}
