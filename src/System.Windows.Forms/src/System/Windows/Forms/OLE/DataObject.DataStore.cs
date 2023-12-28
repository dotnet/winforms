// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;

namespace System.Windows.Forms;

public partial class DataObject
{
    private class DataStore : IDataObject
    {
        private class DataStoreEntry
        {
            public object? Data { get; }
            public bool AutoConvert { get; }

            public DataStoreEntry(object? data, bool autoConvert)
            {
                Data = data;
                AutoConvert = autoConvert;
            }
        }

        private readonly Dictionary<string, DataStoreEntry> _data = new(BackCompatibleStringComparer.Default);

        public DataStore()
        {
            CompModSwitches.DataObject.TraceVerbose("DataStore: Constructed DataStore");
        }

        public virtual object? GetData(string format, bool autoConvert)
        {
            CompModSwitches.DataObject.TraceVerbose($"DataStore: GetData: {format}, {autoConvert}");
            if (string.IsNullOrWhiteSpace(format))
            {
                return null;
            }

            object? baseVar = null;
            if (_data.TryGetValue(format, out DataStoreEntry? dse))
            {
                baseVar = dse.Data;
            }

            object? original = baseVar;

            if (autoConvert
                && (dse is null || dse.AutoConvert)
                && (baseVar is null || baseVar is MemoryStream))
            {
                string[]? mappedFormats = GetMappedFormats(format);
                if (mappedFormats is not null)
                {
                    for (int i = 0; i < mappedFormats.Length; i++)
                    {
                        if (!format.Equals(mappedFormats[i]))
                        {
                            if (_data.TryGetValue(mappedFormats[i], out DataStoreEntry? found))
                            {
                                baseVar = found.Data;
                            }

                            if (baseVar is not null && baseVar is not MemoryStream)
                            {
                                original = null;
                                break;
                            }
                        }
                    }
                }
            }

            if (original is not null)
            {
                return original;
            }
            else
            {
                return baseVar;
            }
        }

        public virtual object? GetData(string format)
        {
            CompModSwitches.DataObject.TraceVerbose($"DataStore: GetData: {format}");
            return GetData(format, true);
        }

        public virtual object? GetData(Type format)
        {
            CompModSwitches.DataObject.TraceVerbose($"DataStore: GetData: {format.FullName}");
            return GetData(format.FullName!);
        }

        public virtual void SetData(string format, bool autoConvert, object? data)
        {
            CompModSwitches.DataObject.TraceVerbose($"DataStore: SetData: {format}, {autoConvert}, {data?.ToString() ?? "(null)"}");
            if (string.IsNullOrWhiteSpace(format))
            {
                ArgumentNullException.ThrowIfNull(format);

                throw new ArgumentException(SR.DataObjectWhitespaceEmptyFormatNotAllowed, nameof(format));
            }

            // We do not have proper support for Dibs, so if the user explicitly asked
            // for Dib and provided a Bitmap object we can't convert.  Instead, publish as an HBITMAP
            // and let the system provide the conversion for us.
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

            _data[format] = new DataStoreEntry(data, autoConvert);
        }

        public virtual void SetData(string format, object? data)
        {
            CompModSwitches.DataObject.TraceVerbose($"DataStore: SetData: {format}, {data?.ToString() ?? "(null)"}");
            SetData(format, true, data);
        }

        public virtual void SetData(Type format, object? data)
        {
            CompModSwitches.DataObject.TraceVerbose($"DataStore: SetData: {format?.FullName ?? "(null)"}, {data?.ToString() ?? "(null)"}");
            ArgumentNullException.ThrowIfNull(format);

            SetData(format.FullName!, data);
        }

        public virtual void SetData(object? data)
        {
            CompModSwitches.DataObject.TraceVerbose($"DataStore: SetData: {data?.ToString() ?? "(null)"}");
            ArgumentNullException.ThrowIfNull(data);

            if (data is ISerializable
                && !_data.ContainsKey(DataFormats.Serializable))
            {
                SetData(DataFormats.Serializable, data);
            }

            SetData(data.GetType(), data);
        }

        public virtual bool GetDataPresent(Type format)
        {
            CompModSwitches.DataObject.TraceVerbose($"DataStore: GetDataPresent: {format.FullName}");
            return GetDataPresent(format.FullName!);
        }

        public virtual bool GetDataPresent(string format, bool autoConvert)
        {
            CompModSwitches.DataObject.TraceVerbose($"DataStore: GetDataPresent: {format}, {autoConvert}");
            if (string.IsNullOrWhiteSpace(format))
            {
                return false;
            }

            if (!autoConvert)
            {
                Debug.Assert(_data is not null, "data must be non-null");
                return _data.ContainsKey(format);
            }
            else
            {
                string[] formats = GetFormats(autoConvert);
                CompModSwitches.DataObject.TraceVerbose($"DataStore:  got {formats.Length} formats from get formats");
                Debug.Assert(formats is not null, "Null returned from GetFormats");
                for (int i = 0; i < formats.Length; i++)
                {
                    Debug.Assert(formats[i] is not null, $"Null format inside of formats at index {i}");
                    if (format.Equals(formats[i]))
                    {
                        CompModSwitches.DataObject.TraceVerbose("DataStore: GetDataPresent: returning true");
                        return true;
                    }
                }

                CompModSwitches.DataObject.TraceVerbose("DataStore: GetDataPresent: returning false");
                return false;
            }
        }

        public virtual bool GetDataPresent(string format)
        {
            CompModSwitches.DataObject.TraceVerbose($"DataStore: GetDataPresent: {format}");
            return GetDataPresent(format, true);
        }

        public virtual string[] GetFormats(bool autoConvert)
        {
            CompModSwitches.DataObject.TraceVerbose($"DataStore: GetFormats: {autoConvert}");
            Debug.Assert(_data is not null, "data collection can't be null");
            Debug.Assert(_data.Keys is not null, "data Keys collection can't be null");

            string[] baseVar = new string[_data.Keys.Count];
            _data.Keys.CopyTo(baseVar, 0);
            Debug.Assert(baseVar is not null, "Collections should never return NULL arrays!!!");
            if (autoConvert)
            {
                CompModSwitches.DataObject.TraceVerbose("DataStore: applying autoConvert");

                // Since we are only adding elements to the HashSet, the order will be preserved.
                int baseVarLength = baseVar.Length;
                HashSet<string> distinctFormats = new(baseVarLength);
                for (int i = 0; i < baseVarLength; i++)
                {
                    Debug.Assert(_data[baseVar[i]] is not null, $"Null item in data collection with key '{baseVar[i]}'");
                    if (_data[baseVar[i]]!.AutoConvert)
                    {
                        string[] cur = GetMappedFormats(baseVar[i])!;
                        Debug.Assert(cur is not null, $"GetMappedFormats returned null for '{baseVar[i]}'");
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

            CompModSwitches.DataObject.TraceVerbose($"DataStore: returning {baseVar.Length} formats from GetFormats");
            return baseVar;
        }

        public virtual string[] GetFormats()
        {
            CompModSwitches.DataObject.TraceVerbose("DataStore: GetFormats");
            return GetFormats(true);
        }
    }
}
