// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using static Interop;

namespace System.Windows.Forms
{
    public partial class DataObject
    {
        /// <summary>
        ///  Part of IComDataObject, used to interop with OLE.
        /// </summary>
        private class FormatEnumerator : IEnumFORMATETC
        {
            internal IDataObject parent = null;
            internal ArrayList formats = new ArrayList();
            internal int current = 0;

            public FormatEnumerator(IDataObject parent) : this(parent, parent.GetFormats())
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "FormatEnumerator: Constructed: " + parent.ToString());
            }

            public FormatEnumerator(IDataObject parent, FORMATETC[] formats)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "FormatEnumerator: Constructed: " + parent.ToString() + ", FORMATETC[]" + formats.Length.ToString(CultureInfo.InvariantCulture));
                this.formats.Clear();
                this.parent = parent;
                current = 0;
                if (formats != null)
                {
                    if (parent is DataObject dataObject && dataObject.RestrictedFormats)
                    {
                        if (!Clipboard.IsFormatValid(formats))
                        {
                            throw new Security.SecurityException(SR.ClipboardSecurityException);
                        }
                    }

                    for (int i = 0; i < formats.Length; i++)
                    {
                        FORMATETC currentFormat = formats[i];

                        FORMATETC temp = new FORMATETC
                        {
                            cfFormat = currentFormat.cfFormat,
                            dwAspect = currentFormat.dwAspect,
                            ptd = currentFormat.ptd,
                            lindex = currentFormat.lindex,
                            tymed = currentFormat.tymed
                        };
                        this.formats.Add(temp);
                    }
                }
            }

            public FormatEnumerator(IDataObject parent, string[] formats)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "FormatEnumerator: Constructed: " + parent.ToString() + ", string[]" + formats.Length.ToString(CultureInfo.InvariantCulture));

                this.parent = parent;
                this.formats.Clear();

                if (formats != null)
                {
                    if (parent is DataObject dataObject && dataObject.RestrictedFormats)
                    {
                        if (!Clipboard.IsFormatValid(formats))
                        {
                            throw new Security.SecurityException(SR.ClipboardSecurityException);
                        }
                    }

                    for (int i = 0; i < formats.Length; i++)
                    {
                        string format = formats[i];
                        FORMATETC temp = new FORMATETC
                        {
                            cfFormat = unchecked((short)(ushort)(DataFormats.GetFormat(format).Id)),
                            dwAspect = DVASPECT.DVASPECT_CONTENT,
                            ptd = IntPtr.Zero,
                            lindex = -1
                        };

                        if (format.Equals(DataFormats.Bitmap))
                        {
                            temp.tymed = TYMED.TYMED_GDI;
                        }
                        else if (format.Equals(DataFormats.EnhancedMetafile))
                        {
                            temp.tymed = TYMED.TYMED_ENHMF;
                        }
                        else if (format.Equals(DataFormats.Text)
                                 || format.Equals(DataFormats.UnicodeText)
                                 || format.Equals(DataFormats.StringFormat)
                                 || format.Equals(DataFormats.Rtf)
                                 || format.Equals(DataFormats.CommaSeparatedValue)
                                 || format.Equals(DataFormats.FileDrop)
                                 || format.Equals(CF_DEPRECATED_FILENAME)
                                 || format.Equals(CF_DEPRECATED_FILENAMEW))
                        {
                            temp.tymed = TYMED.TYMED_HGLOBAL;
                        }
                        else
                        {
                            temp.tymed = TYMED.TYMED_HGLOBAL;
                        }

                        if (temp.tymed != 0)
                        {
                            this.formats.Add(temp);
                        }
                    }
                }
            }

            public int Next(int celt, FORMATETC[] rgelt, int[] pceltFetched)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "FormatEnumerator: Next");
                if (this.current < formats.Count && celt > 0)
                {
                    FORMATETC current = (FORMATETC)formats[this.current];
                    rgelt[0].cfFormat = current.cfFormat;
                    rgelt[0].tymed = current.tymed;
                    rgelt[0].dwAspect = DVASPECT.DVASPECT_CONTENT;
                    rgelt[0].ptd = IntPtr.Zero;
                    rgelt[0].lindex = -1;

                    if (pceltFetched != null)
                    {
                        pceltFetched[0] = 1;
                    }
                    this.current++;
                }
                else
                {
                    if (pceltFetched != null)
                    {
                        pceltFetched[0] = 0;
                    }
                    return (int)HRESULT.S_FALSE;
                }
                return (int)HRESULT.S_OK;
            }

            public int Skip(int celt)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "FormatEnumerator: Skip");
                if (current + celt >= formats.Count)
                {
                    return (int)HRESULT.S_FALSE;
                }
                current += celt;
                return (int)HRESULT.S_OK;
            }

            public int Reset()
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "FormatEnumerator: Reset");
                current = 0;
                return (int)HRESULT.S_OK;
            }

            public void Clone(out IEnumFORMATETC ppenum)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "FormatEnumerator: Clone");
                FORMATETC[] temp = new FORMATETC[formats.Count];
                formats.CopyTo(temp, 0);
                ppenum = new FormatEnumerator(parent, temp);
            }
        }
    }
}
