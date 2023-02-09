// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using Windows.Win32.System.Com;
using Windows.Win32.System.Com.StructuredStorage;

namespace System.Windows.Forms;

public abstract unsafe partial class AxHost
{
    internal class PropertyBagStream : IPropertyBag.Interface
    {
        private Hashtable _bag = new();

        internal void Read(Stream stream)
        {
            BinaryFormatter formatter = new();
            try
            {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                _bag = (Hashtable)formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
            }
            catch
            {
                // Error reading.  Just init an empty hashtable.
                _bag = new Hashtable();
            }
        }

        HRESULT IPropertyBag.Interface.Read(PCWSTR pszPropName, VARIANT* pVar, IErrorLog* pErrorLog)
        {
            if (pVar is null || pszPropName.Value is null)
            {
                return HRESULT.E_POINTER;
            }

            string name = pszPropName.ToString();

            s_axHTraceSwitch.TraceVerbose($"Reading property {name} from OCXState propertybag.");

            if (!_bag.Contains(name))
            {
                *pVar = default;
                return HRESULT.E_INVALIDARG;
            }

            object? value = _bag[name];
            *pVar = VARIANT.FromObject(value);
            s_axHTraceSwitch.TraceVerbose($"\tValue={value ?? "<null>"}");

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

            s_axHTraceSwitch.TraceVerbose($"Writing property {name} [{*pVar}] into OCXState propertybag.");
            _bag[name] = value;
            return HRESULT.S_OK;
        }

        internal void Write(Stream stream)
        {
            BinaryFormatter formatter = new();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            formatter.Serialize(stream, _bag);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
        }
    }
}
