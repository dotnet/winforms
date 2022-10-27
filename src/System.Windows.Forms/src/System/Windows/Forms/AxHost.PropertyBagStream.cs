// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using Windows.Win32.System.Com;
using Windows.Win32.System.Com.StructuredStorage;

namespace System.Windows.Forms
{
    public abstract unsafe partial class AxHost
    {
        internal class PropertyBagStream : IPropertyBag.Interface
        {
            private Hashtable _bag = new();

            internal void Read(Stream stream)
            {
                BinaryFormatter formatter = new BinaryFormatter();
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
                if (pVar is null)
                {
                    return HRESULT.E_POINTER;
                }

                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"Reading property {pszPropName} from OCXState propertybag.");

                if (!_bag.Contains(pszPropName))
                {
                    *pVar = default;
                    return HRESULT.E_INVALIDARG;
                }

                *pVar = (VARIANT)_bag[pszPropName];
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"\tValue={*pVar}");

                // The EE returns a VT_EMPTY for a null. The problem is that visual basic6 expects the caller to respect the
                // "hint" it gives in the VariantType. For eg., for a VT_BSTR, it expects that the callee will null
                // out the BSTR field of the variant. Since, the EE or us cannot do anything about this, we will return
                // a E_INVALIDARG rather than let visual basic6 crash.

                return (*pVar).Equals(default(VARIANT)) ? HRESULT.E_INVALIDARG : HRESULT.S_OK;
            }

            HRESULT IPropertyBag.Interface.Write(PCWSTR pszPropName, VARIANT* pVar)
            {
                if (pVar is null)
                {
                    return HRESULT.E_POINTER;
                }

                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"Writing property {pszPropName} [{*pVar}] into OCXState propertybag.");
                if (!pVar->GetType().IsSerializable)
                {
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"\t {pVar->GetType().FullName} is not serializable.");
                    return HRESULT.S_OK;
                }

                _bag[pszPropName] = *pVar;
                return HRESULT.S_OK;
            }

            internal void Write(Stream stream)
            {
                BinaryFormatter formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                formatter.Serialize(stream, _bag);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
            }
        }
    }
}
