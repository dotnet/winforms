// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using static Interop;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        internal class PropertyBagStream : Ole32.IPropertyBag
        {
            private Hashtable bag = new Hashtable();

            internal void Read(Stream stream)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    bag = (Hashtable)formatter.Deserialize(stream);
                }
                catch
                {
                    // Error reading.  Just init an empty hashtable.
                    bag = new Hashtable();
                }
            }

            HRESULT Ole32.IPropertyBag.Read(string pszPropName, ref object pVar, Ole32.IErrorLog pErrorLog)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Reading property " + pszPropName + " from OCXState propertybag.");

                if (!bag.Contains(pszPropName))
                {
                    return HRESULT.E_INVALIDARG;
                }

                pVar = bag[pszPropName];
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "\tValue=" + ((pVar == null) ? "<null>" : pVar.ToString()));

                // The EE returns a VT_EMPTY for a null. The problem is that visual basic6 expects the caller to respect the
                // "hint" it gives in the VariantType. For eg., for a VT_BSTR, it expects that the callee will null
                // out the BSTR field of the variant. Since, the EE or us cannot do anything about this, we will return
                // a E_INVALIDARG rather than let visual basic6 crash.
                //
                return (pVar == null) ? HRESULT.E_INVALIDARG : HRESULT.S_OK;
            }

            HRESULT Ole32.IPropertyBag.Write(string pszPropName, ref object pVar)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Writing property " + pszPropName + " [" + pVar + "] into OCXState propertybag.");
                if (pVar != null && !pVar.GetType().IsSerializable)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "\t " + pVar.GetType().FullName + " is not serializable.");
                    return HRESULT.S_OK;
                }

                bag[pszPropName] = pVar;
                return HRESULT.S_OK;
            }

            internal void Write(Stream stream)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, bag);
            }
        }
    }
}
