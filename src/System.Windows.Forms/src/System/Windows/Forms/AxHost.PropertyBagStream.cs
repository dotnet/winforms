// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using static Interop;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        internal class PropertyBagStream : Oleaut32.IPropertyBag
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

            HRESULT Oleaut32.IPropertyBag.Read(string pszPropName, out object pVar, Oleaut32.IErrorLog pErrorLog)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"Reading property {pszPropName} from OCXState propertybag.");

                if (!_bag.Contains(pszPropName))
                {
                    pVar = null;
                    return HRESULT.Values.E_INVALIDARG;
                }

                pVar = _bag[pszPropName];
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"\tValue={pVar ?? "<null>"}");

                // The EE returns a VT_EMPTY for a null. The problem is that visual basic6 expects the caller to respect the
                // "hint" it gives in the VariantType. For eg., for a VT_BSTR, it expects that the callee will null
                // out the BSTR field of the variant. Since, the EE or us cannot do anything about this, we will return
                // a E_INVALIDARG rather than let visual basic6 crash.

                return (pVar is null) ? HRESULT.Values.E_INVALIDARG : HRESULT.Values.S_OK;
            }

            HRESULT Oleaut32.IPropertyBag.Write(string pszPropName, ref object pVar)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"Writing property {pszPropName} [{pVar}] into OCXState propertybag.");
                if (pVar is not null && !pVar.GetType().IsSerializable)
                {
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"\t {pVar.GetType().FullName} is not serializable.");
                    return HRESULT.Values.S_OK;
                }

                _bag[pszPropName] = pVar;
                return HRESULT.Values.S_OK;
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
