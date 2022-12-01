// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using static System.Runtime.InteropServices.ComWrappers;
using Com = Windows.Win32.System.Com;

namespace System.Windows.Forms
{
    public partial class DataObject
    {
        /// <summary>
        ///  Part of IComDataObject, used to interop with OLE.
        /// </summary>
        private class FormatEnumerator : IEnumFORMATETC, Com.IManagedWrapper
        {
            private IDataObject _parent;
            private readonly List<FORMATETC> _formats = new List<FORMATETC>();
            private int _current;
            private static ComInterfaceTable? s_comInterfaceTable;

            public FormatEnumerator(IDataObject parent) : this(parent, parent.GetFormats())
            {
                CompModSwitches.DataObject.TraceVerbose($"FormatEnumerator: Constructed: {parent}");
            }

            private FormatEnumerator(FormatEnumerator source)
            {
                _parent = source._parent;
                _current = 0;
                _formats.AddRange(source._formats);
            }

            public FormatEnumerator(IDataObject parent, string[]? formats)
            {
                CompModSwitches.DataObject.TraceVerbose($"FormatEnumerator: Constructed: {parent}, string[{(formats?.Length ?? 0)}]");

                _parent = parent;

                if (formats is not null)
                {
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

                        _formats.Add(temp);
                    }
                }
            }

            public int Next(int celt, FORMATETC[] rgelt, int[]? pceltFetched)
            {
                CompModSwitches.DataObject.TraceVerbose("FormatEnumerator: Next");
                if (_current < _formats.Count && celt > 0)
                {
                    FORMATETC current = _formats[_current];
                    rgelt[0].cfFormat = current.cfFormat;
                    rgelt[0].tymed = current.tymed;
                    rgelt[0].dwAspect = DVASPECT.DVASPECT_CONTENT;
                    rgelt[0].ptd = IntPtr.Zero;
                    rgelt[0].lindex = -1;

                    if (pceltFetched is not null)
                    {
                        pceltFetched[0] = 1;
                    }

                    _current++;
                }
                else
                {
                    if (pceltFetched is not null)
                    {
                        pceltFetched[0] = 0;
                    }

                    return (int)HRESULT.S_FALSE;
                }

                return (int)HRESULT.S_OK;
            }

            public int Skip(int celt)
            {
                CompModSwitches.DataObject.TraceVerbose("FormatEnumerator: Skip");
                if (_current + celt >= _formats.Count)
                {
                    return (int)HRESULT.S_FALSE;
                }

                _current += celt;
                return (int)HRESULT.S_OK;
            }

            public int Reset()
            {
                CompModSwitches.DataObject.TraceVerbose("FormatEnumerator: Reset");
                _current = 0;
                return (int)HRESULT.S_OK;
            }

            public void Clone(out IEnumFORMATETC ppenum)
            {
                CompModSwitches.DataObject.TraceVerbose("FormatEnumerator: Clone");
                ppenum = new FormatEnumerator(this);
            }

            unsafe ComInterfaceTable Com.IManagedWrapper.GetComInterfaceTable()
            {
                if (s_comInterfaceTable is null)
                {
                    Com.IEnumFORMATETC.Vtbl* vtable = (Com.IEnumFORMATETC.Vtbl*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(Com.IEnumFORMATETC.Vtbl), sizeof(Com.IEnumFORMATETC.Vtbl));
                    Interop.WinFormsComWrappers.PopulateIUnknownVTable((Com.IUnknown.Vtbl*)vtable);

                    Interop.WinFormsComWrappers.IEnumFORMATETCVtbl.PopulateVTable(vtable);

                    ComInterfaceEntry* wrapperEntry = (ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IEnumFORMATETC), sizeof(ComInterfaceEntry));
                    wrapperEntry[0].IID = *IID.Get<Com.IEnumFORMATETC>();
                    wrapperEntry[0].Vtable = (nint)(void*)vtable;

                    s_comInterfaceTable = new ComInterfaceTable()
                    {
                        Entries = wrapperEntry,
                        Count = 1
                    };
                }

                return (ComInterfaceTable)s_comInterfaceTable;
            }
        }
    }
}
