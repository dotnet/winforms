// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Formats.Nrbf;
using System.Private.Windows.Core.BinaryFormat;
using System.Windows.Forms.Nrbf;
using Windows.Win32.System.Com;
using Windows.Win32.System.Com.StructuredStorage;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms;

public partial class Control
{
    private unsafe partial class ActiveXImpl
    {
        /// <summary>
        ///  This is a property bag implementation that sits on a stream. It can read and write the bag to the stream.
        /// </summary>
        private class PropertyBagStream : IPropertyBag.Interface, IManagedWrapper<IPropertyBag>
        {
            private Hashtable _bag = [];

            internal void Read(IStream* istream)
            {
                using DataStreamFromComStream stream = new(istream);
                bool success = false;

                try
                {
                    SerializationRecord rootRecord = stream.Decode();
                    success = rootRecord.TryGetPrimitiveHashtable(out _bag!);
                }
                catch (Exception e) when (!e.IsCriticalException())
                {
                    // We should never hit this case outside of corrupted data. This bag only ever has
                    // strings put in it.
                    Debug.Fail(e.Message);
                }

                if (!success)
                {
                    // Error reading. Just init an empty hashtable.
                    _bag = [];
                }
            }

            HRESULT IPropertyBag.Interface.Read(PCWSTR pszPropName, VARIANT* pVar, IErrorLog* pErrorLog)
            {
                if (pVar is null || pszPropName.Value is null)
                {
                    return HRESULT.E_POINTER;
                }

                string name = pszPropName.ToString();
                if (!_bag.Contains(name))
                {
                    *pVar = default;
                    return HRESULT.E_INVALIDARG;
                }

                Debug.Assert(_bag[name] is string);
                *pVar = VARIANT.FromObject(_bag[name]);
                return HRESULT.S_OK;
            }

            HRESULT IPropertyBag.Interface.Write(PCWSTR pszPropName, VARIANT* pVar)
            {
                if (pVar is null || pszPropName.Value is null)
                {
                    return HRESULT.E_POINTER;
                }

                Debug.Assert(pVar->vt == VARENUM.VT_BSTR);
                _bag[pszPropName.ToString()] = pVar->ToObject();
                return HRESULT.S_OK;
            }

            internal void Write(IStream* istream)
            {
                using DataStreamFromComStream stream = new(istream);
                BinaryFormatWriter.WritePrimitiveHashtable(stream, _bag);
            }
        }
    }
}
