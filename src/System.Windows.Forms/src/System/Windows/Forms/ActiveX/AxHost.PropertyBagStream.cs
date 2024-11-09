// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Formats.Nrbf;
using System.Runtime.Serialization.Formatters.Binary;
using System.Private.Windows.Core.BinaryFormat;
using Windows.Win32.System.Com;
using Windows.Win32.System.Com.StructuredStorage;
using Windows.Win32.System.Variant;
using System.Windows.Forms.Nrbf;

namespace System.Windows.Forms;

public abstract unsafe partial class AxHost
{
    internal class PropertyBagStream : IPropertyBag.Interface
    {
        private readonly Hashtable _bag;

        internal PropertyBagStream() => _bag = [];

        internal PropertyBagStream(Stream stream)
        {
            long position = stream.Position;
            try
            {
                SerializationRecord rootRecord = stream.Decode();
                if (rootRecord.TryGetPrimitiveHashtable(out _bag!))
                {
                    return;
                }
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
                // Don't usually expect to fall into this case as we should usually not have anything
                // in the stream other than primitive VARIANTs, which are handled. If there are arrays or
                // interface pointers we'd hit this.
                Debug.WriteLine($"PropertyBagStream: {nameof(NrbfDecoder)} failed with {ex.Message}");
            }

            try
            {
                stream.Position = position;
#pragma warning disable SYSLIB0011 // Type or member is obsolete
#pragma warning disable CA2300 // Do not use insecure deserializer BinaryFormatter
#pragma warning disable CA2301 // Do not call BinaryFormatter.Deserialize without first setting BinaryFormatter.Binder
#pragma warning disable CA2302 // Ensure BinaryFormatter.Binder is set before calling BinaryFormatter.Deserialize
                _bag = (Hashtable)new BinaryFormatter().Deserialize(stream); // CodeQL[SM03722, SM04191] : BinaryFormatter is intended to be used as a fallback for unsupported types. Users must explicitly opt into this behavior"
            }
            catch (Exception inner) when (!inner.IsCriticalException())
            {
                Debug.Fail($"PropertyBagStream: {nameof(BinaryFormatter)} failed with {inner.Message}");
#pragma warning restore CA2300
#pragma warning restore CA2301
#pragma warning restore CA2302
#pragma warning restore SYSLIB0011

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
                Debug.WriteLine($"PropertyBagStream.Save: {nameof(NrbfDecoder)} failed with {ex.Message}");

                stream.Position = position;
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                new BinaryFormatter().Serialize(stream, _bag);
#pragma warning restore SYSLIB0011
            }
        }
    }
}
