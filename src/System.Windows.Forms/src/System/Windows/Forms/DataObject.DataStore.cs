// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace System.Windows.Forms
{
    public partial class DataObject
    {
        private class DataStore : IDataObject
        {
            private class DataStoreEntry
            {
                public object data;
                public bool autoConvert;

                public DataStoreEntry(object data, bool autoConvert)
                {
                    this.data = data;
                    this.autoConvert = autoConvert;
                }
            }

            private readonly Hashtable data = new Hashtable(BackCompatibleStringComparer.Default);

            public DataStore()
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: Constructed DataStore");
            }

            public virtual object GetData(string format, bool autoConvert)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetData: " + format + ", " + autoConvert.ToString());
                if (string.IsNullOrWhiteSpace(format))
                {
                    return null;
                }

                DataStoreEntry dse = (DataStoreEntry)data[format];
                object baseVar = null;
                if (dse != null)
                {
                    baseVar = dse.data;
                }
                object original = baseVar;

                if (autoConvert
                    && (dse is null || dse.autoConvert)
                    && (baseVar is null || baseVar is MemoryStream))
                {
                    string[] mappedFormats = GetMappedFormats(format);
                    if (mappedFormats != null)
                    {
                        for (int i = 0; i < mappedFormats.Length; i++)
                        {
                            if (!format.Equals(mappedFormats[i]))
                            {
                                DataStoreEntry found = (DataStoreEntry)data[mappedFormats[i]];
                                if (found != null)
                                {
                                    baseVar = found.data;
                                }
                                if (baseVar != null && !(baseVar is MemoryStream))
                                {
                                    original = null;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (original != null)
                {
                    return original;
                }
                else
                {
                    return baseVar;
                }
            }

            public virtual object GetData(string format)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetData: " + format);
                return GetData(format, true);
            }

            public virtual object GetData(Type format)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetData: " + format.FullName);
                return GetData(format.FullName);
            }

            public virtual void SetData(string format, bool autoConvert, object data)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: SetData: " + format + ", " + autoConvert.ToString() + ", " + data?.ToString() ?? "(null)");
                if (string.IsNullOrWhiteSpace(format))
                {
                    if (format is null)
                    {
                        throw new ArgumentNullException(nameof(format));
                    }

                    throw new ArgumentException(SR.DataObjectWhitespaceEmptyFormatNotAllowed, nameof(format));
                }

                // We do not have proper support for Dibs, so if the user explicitly asked
                // for Dib and provided a Bitmap object we can't convert.  Instead, publish as an HBITMAP
                // and let the system provide the conversion for us.
                //
                if (data is Bitmap && format.Equals(DataFormats.Dib))
                {
                    if (autoConvert)
                    {
                        format = DataFormats.Bitmap;
                    }
                    else
                    {
                        throw new NotSupportedException(SR.DataObjectDibNotSupported);
                    }
                }

                this.data[format] = new DataStoreEntry(data, autoConvert);
            }

            public virtual void SetData(string format, object data)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: SetData: " + format + ", " + data?.ToString() ?? "(null)");
                SetData(format, true, data);
            }

            public virtual void SetData(Type format, object data)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: SetData: " + format?.FullName ?? "(null)" + ", " + data?.ToString() ?? "(null)");
                if (format is null)
                {
                    throw new ArgumentNullException(nameof(format));
                }

                SetData(format.FullName, data);
            }

            public virtual void SetData(object data)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: SetData: " + data?.ToString() ?? "(null)");
                if (data is null)
                {
                    throw new ArgumentNullException(nameof(data));
                }

                if (data is ISerializable
                    && !this.data.ContainsKey(DataFormats.Serializable))
                {
                    SetData(DataFormats.Serializable, data);
                }

                SetData(data.GetType(), data);
            }

            public virtual bool GetDataPresent(Type format)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetDataPresent: " + format.FullName);
                return GetDataPresent(format.FullName);
            }

            public virtual bool GetDataPresent(string format, bool autoConvert)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetDataPresent: " + format + ", " + autoConvert.ToString());
                if (string.IsNullOrWhiteSpace(format))
                {
                    return false;
                }

                if (!autoConvert)
                {
                    Debug.Assert(data != null, "data must be non-null");
                    return data.ContainsKey(format);
                }
                else
                {
                    string[] formats = GetFormats(autoConvert);
                    Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore:  got " + formats.Length.ToString(CultureInfo.InvariantCulture) + " formats from get formats");
                    Debug.Assert(formats != null, "Null returned from GetFormats");
                    for (int i = 0; i < formats.Length; i++)
                    {
                        Debug.Assert(formats[i] != null, "Null format inside of formats at index " + i.ToString(CultureInfo.InvariantCulture));
                        if (format.Equals(formats[i]))
                        {
                            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetDataPresent: returning true");
                            return true;
                        }
                    }
                    Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetDataPresent: returning false");
                    return false;
                }
            }

            public virtual bool GetDataPresent(string format)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetDataPresent: " + format);
                return GetDataPresent(format, true);
            }

            public virtual string[] GetFormats(bool autoConvert)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetFormats: " + autoConvert.ToString());
                Debug.Assert(data != null, "data collection can't be null");
                Debug.Assert(data.Keys != null, "data Keys collection can't be null");

                string[] baseVar = new string[data.Keys.Count];
                data.Keys.CopyTo(baseVar, 0);
                Debug.Assert(baseVar != null, "Collections should never return NULL arrays!!!");
                if (autoConvert)
                {
                    Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: applying autoConvert");

                    // Since we are only adding elements to the HashSet, the order will be preserved.
                    int baseVarLength = baseVar.Length;
                    HashSet<string> distinctFormats = new HashSet<string>(baseVarLength);
                    for (int i = 0; i < baseVarLength; i++)
                    {
                        Debug.Assert(data[baseVar[i]] != null, "Null item in data collection with key '" + baseVar[i] + "'");
                        if (((DataStoreEntry)data[baseVar[i]]).autoConvert)
                        {
                            string[] cur = GetMappedFormats(baseVar[i]);
                            Debug.Assert(cur != null, "GetMappedFormats returned null for '" + baseVar[i] + "'");
                            for (int j = 0; j < cur.Length; j++)
                            {
                                distinctFormats.Add(cur[j]);
                            }
                        }
                        else
                        {
                            distinctFormats.Add(baseVar[i]);
                        }
                    }

                    baseVar = distinctFormats.ToArray();
                }
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: returing " + baseVar.Length.ToString(CultureInfo.InvariantCulture) + " formats from GetFormats");
                return baseVar;
            }

            public virtual string[] GetFormats()
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetFormats");
                return GetFormats(true);
            }
        }
    }
}
