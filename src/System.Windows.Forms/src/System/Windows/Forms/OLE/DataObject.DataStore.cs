// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
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
        }

        public virtual object? GetData(string format, bool autoConvert)
        {
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

                            if (baseVar is not null and not MemoryStream)
                            {
                                original = null;
                                break;
                            }
                        }
                    }
                }
            }

            return original ?? baseVar;
        }

        public virtual object? GetData(string format) => GetData(format, autoConvert: true);

        public virtual object? GetData(Type format) => GetData(format.FullName!);

        public virtual void SetData(string format, bool autoConvert, object? data)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                ArgumentNullException.ThrowIfNull(format);
                throw new ArgumentException(SR.DataObjectWhitespaceEmptyFormatNotAllowed, nameof(format));
            }

            // We do not have proper support for Dibs, so if the user explicitly asked
            // for Dib and provided a Bitmap object we can't convert. Instead, publish as an HBITMAP
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

        public virtual void SetData(string format, object? data) => SetData(format, autoConvert: true, data);

        public virtual void SetData(Type format, object? data)
        {
            ArgumentNullException.ThrowIfNull(format);
            SetData(format.FullName!, data);
        }

        public virtual void SetData(object? data)
        {
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
            return GetDataPresent(format.FullName!);
        }

        public virtual bool GetDataPresent(string format, bool autoConvert)
        {
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
                Debug.Assert(formats is not null, "Null returned from GetFormats");

                for (int i = 0; i < formats.Length; i++)
                {
                    Debug.Assert(formats[i] is not null, $"Null format inside of formats at index {i}");
                    if (format.Equals(formats[i]))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public virtual bool GetDataPresent(string format) => GetDataPresent(format, autoConvert: true);

        public virtual string[] GetFormats(bool autoConvert)
        {
            Debug.Assert(_data is not null, "data collection can't be null");
            Debug.Assert(_data.Keys is not null, "data Keys collection can't be null");

            string[] baseVar = new string[_data.Keys.Count];
            _data.Keys.CopyTo(baseVar, 0);
            Debug.Assert(baseVar is not null, "Collections should never return NULL arrays!!!");

            if (autoConvert)
            {
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

                baseVar = [.. distinctFormats];
            }

            return baseVar;
        }

        public virtual string[] GetFormats() => GetFormats(autoConvert: true);
    }
}
