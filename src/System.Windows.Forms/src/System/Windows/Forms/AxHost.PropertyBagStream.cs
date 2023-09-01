﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms.BinaryFormat;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using Windows.Win32.System.Com.StructuredStorage;

namespace System.Windows.Forms;

public abstract unsafe partial class AxHost
{
    internal class PropertyBagStream : IPropertyBag.Interface
    {
        private readonly Hashtable _bag;

        internal PropertyBagStream() => _bag = new();

        internal PropertyBagStream(Stream stream)
        {
            long position = stream.Position;
            try
            {
                BinaryFormattedObject format = new(stream, leaveOpen: true);
                if (format.TryGetPrimitiveHashtable(out _bag!))
                {
                    return;
                }
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
                // Don't usually expect to fall into this case as we should usually not have anything
                // in the stream other than primitive VARIANTs, which are handled. If there are arrays or
                // interface pointers we'd hit this.
                Debug.WriteLine($"PropertyBagStream: {nameof(BinaryFormattedObject)} failed with {ex.Message}");
            }

            try
            {
                stream.Position = position;
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                _bag = (Hashtable)new BinaryFormatter().Deserialize(stream);
            }
            catch (Exception inner) when (!inner.IsCriticalException())
            {
                Debug.Fail($"PropertyBagStream: {nameof(BinaryFormatter)} failed with {inner.Message}");
#pragma warning restore SYSLIB0011

                // Error reading. Just init an empty hashtable.
                _bag = new();
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

            object? value = _bag[name];
            *pVar = VARIANT.FromObject(value);

            // The EE returns a VT_EMPTY for a null. The problem is that Visual Basic 6 expects the caller to respect
            // the "hint" it gives in the VariantType. For eg., for a VT_BSTR, it expects that the callee will null
            // out the BSTR field of the variant. Since the EE or us cannot do anything about this, we will return
            // a E_INVALIDARG rather than let Visual Basic 6 crash.

            return (*pVar).Equals(default(VARIANT)) ? HRESULT.E_INVALIDARG : HRESULT.S_OK;
        }

        HRESULT IPropertyBag.Interface.Write(PCWSTR pszPropName, VARIANT* pVar)
        {
            if (pVar is null || pszPropName.Value is null)
            {
                return HRESULT.E_POINTER;
            }

            string name = pszPropName.ToString();
            object? value = pVar->ToObject();

            _bag[name] = value;
            return HRESULT.S_OK;
        }

        internal void Save(Stream stream)
        {
            long position = stream.Position;

            try
            {
                BinaryFormatWriter.WritePrimitiveHashtable(stream, _bag);
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
                Debug.WriteLine($"PropertyBagStream.Save: {nameof(BinaryFormattedObject)} failed with {ex.Message}");

                stream.Position = position;
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                new BinaryFormatter().Serialize(stream, _bag);
#pragma warning restore SYSLIB0011
            }
        }
    }
}
