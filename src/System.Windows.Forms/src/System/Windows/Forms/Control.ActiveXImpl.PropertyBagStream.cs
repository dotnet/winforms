// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using Windows.Win32.System.Com;
using Windows.Win32.System.Com.StructuredStorage;

namespace System.Windows.Forms
{
    public partial class Control
    {
        private unsafe partial class ActiveXImpl
        {
            /// <summary>
            ///  This is a property bag implementation that sits on a stream. It can read and write the bag to the stream.
            /// </summary>
            private class PropertyBagStream : IPropertyBag.Interface
            {
                private Hashtable _bag = new Hashtable();

                internal void Read(IStream* istream)
                {
                    // Visual Basic's memory streams don't support seeking, so we have to work around this limitation
                    // here. We do this by copying the contents of the stream into a MemoryStream object.
                    Stream stream = new DataStreamFromComStream(istream);
                    const int PAGE_SIZE = 0x1000; // one page (4096b)
                    byte[] streamData = new byte[PAGE_SIZE];
                    int offset = 0;

                    int count = stream.Read(streamData, offset, PAGE_SIZE);
                    int totalCount = count;

                    while (count == PAGE_SIZE)
                    {
                        byte[] newChunk = new byte[streamData.Length + PAGE_SIZE];
                        Array.Copy(streamData, newChunk, streamData.Length);
                        streamData = newChunk;

                        offset += PAGE_SIZE;
                        count = stream.Read(streamData, offset, PAGE_SIZE);
                        totalCount += count;
                    }

                    stream = new MemoryStream(streamData);

                    BinaryFormatter formatter = new BinaryFormatter();
                    try
                    {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                        _bag = (Hashtable)formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                    }
                    catch (Exception e)
                    {
                        if (ClientUtils.IsCriticalException(e))
                        {
                            throw;
                        }

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

                    if (!_bag.Contains(pszPropName))
                    {
                        *pVar = default;
                        return HRESULT.E_INVALIDARG;
                    }

                    *pVar = (VARIANT)_bag[pszPropName]!;
                    return HRESULT.S_OK;
                }

                HRESULT IPropertyBag.Interface.Write(PCWSTR pszPropName, VARIANT* pVar)
                {
                    if (pVar is null)
                    {
                        return HRESULT.E_POINTER;
                    }

                    _bag[pszPropName] = *pVar;
                    return HRESULT.S_OK;
                }

                internal void Write(IStream* istream)
                {
                    Stream stream = new DataStreamFromComStream(istream);
                    BinaryFormatter formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                    formatter.Serialize(stream, _bag);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                }
            }
        }
    }
}
