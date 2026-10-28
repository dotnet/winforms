// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.Reflection.Metadata;
using System.Runtime.Serialization;

namespace System.Private.Windows.Ole;

internal sealed partial class DataStore<TOleServices>
#if NET
    : IDataObjectInternal where TOleServices : IOleServices
#else
    : IDataObjectInternal where TOleServices : IOleServices, new()
#endif
{
#if NETFRAMEWORK
    private static readonly TOleServices s_oleServices = new();
#endif

    private readonly Dictionary<string, DataStoreEntry> _mappedData = new(BackCompatibleStringComparer.Default);
    // Protect dictionary state only. Validation, conversion and other callouts must run outside this gate.
#if NET9_0_OR_GREATER
    private readonly Lock _syncRoot = new();
#else
    private readonly object _syncRoot = new();
#endif

    private bool TryGetDataInternal<T>(
        string format,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        data = default;
        if (string.IsNullOrWhiteSpace(format))
        {
            return false;
        }

        if (TryGetData(format, ref autoConvert, out data))
        {
            return true;
        }

        if (!autoConvert)
        {
            return false;
        }

        List<string> mappedFormats = [];
        DataFormatNames.AddMappedFormats(format, mappedFormats);

        foreach (string mappedFormat in mappedFormats)
        {
            Debug.Assert(mappedFormat != format);
            if (TryGetData(mappedFormat, ref autoConvert, out data))
            {
                return true;
            }
        }

        return false;

        bool TryGetData(string format, ref bool autoConvert, [NotNullWhen(true)] out T? data)
        {
            DataStoreEntry entry;
            bool found;
            lock (_syncRoot)
            {
                found = _mappedData.TryGetValue(format, out entry);
            }

            if (found)
            {
                autoConvert |= entry.AutoConvert;

                if (entry.Data is T value)
                {
                    data = value;
                    return true;
                }
#if OLE_JSON
                else if (entry.Data is JsonData<T> jsonData)
                {
                    data = (T)jsonData.Deserialize();
                    return true;
                }
#endif
            }

            data = default;
            return false;
        }
    }

    public object? GetData(string format, bool autoConvert)
    {
        TryGetDataInternal(format, autoConvert, out object? data);
        return data;
    }

    public object? GetData(string format) => GetData(format, autoConvert: true);

    public object? GetData(Type format) => GetData(format.FullName!);

    public void SetData(string format, bool autoConvert, object? data)
    {
        DataStoreEntry entry = CreateEntry(ref format, autoConvert, data);
        lock (_syncRoot)
        {
            _mappedData[format] = entry;
        }
    }

    private static DataStoreEntry CreateEntry(ref string format, bool autoConvert, object? data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(format, nameof(format));
#if NET
        TOleServices.ValidateDataStoreData(ref format, autoConvert, data);
#else
        s_oleServices.ValidateDataStoreData(ref format, autoConvert, data);
#endif
        return new DataStoreEntry(data, autoConvert);
    }

    public void SetData(string format, object? data) => SetData(format, autoConvert: true, data);

    public void SetData(Type format, object? data)
    {
        ArgumentNullException.ThrowIfNull(format);
        SetData(format.FullName.OrThrowIfNull(), data);
    }

    /// <inheritdoc cref="IDataObjectInternal.SetData(object?)"/>
    /// <remarks>
    ///  <para>
    ///   This is the only method that has special behavior for <see cref="ISerializable"/> objects.
    ///  </para>
    /// </remarks>
    public void SetData(object? data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data is ISerializable && !GetDataPresent(DataFormatNames.Serializable, autoConvert: false))
        {
            string format = DataFormatNames.Serializable;
            DataStoreEntry entry = CreateEntry(ref format, autoConvert: true, data);
            lock (_syncRoot)
            {
                // Validation can reenter this store or allow another writer to publish the first entry.
                if (!_mappedData.ContainsKey(DataFormatNames.Serializable))
                {
                    _mappedData[format] = entry;
                }
            }
        }

        SetData(data.GetType(), data);
    }

    public bool GetDataPresent(Type format) => GetDataPresent(format.FullName!);

    public bool GetDataPresent(string format, bool autoConvert)
    {
        if (string.IsNullOrWhiteSpace(format))
        {
            return false;
        }

        if (!autoConvert)
        {
            lock (_syncRoot)
            {
                return _mappedData.ContainsKey(format);
            }
        }

        string[] formats = GetFormats(autoConvert);

        for (int i = 0; i < formats.Length; i++)
        {
            if (format.Equals(formats[i], StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public bool GetDataPresent(string format) => GetDataPresent(format, autoConvert: true);

    public string[] GetFormats(bool autoConvert)
    {
        KeyValuePair<string, DataStoreEntry>[] entries;
        lock (_syncRoot)
        {
            if (!autoConvert)
            {
                string[] definedFormats = new string[_mappedData.Count];
                _mappedData.Keys.CopyTo(definedFormats, 0);
                return definedFormats;
            }

            entries = new KeyValuePair<string, DataStoreEntry>[_mappedData.Count];
            ((ICollection<KeyValuePair<string, DataStoreEntry>>)_mappedData).CopyTo(entries, 0);
        }

        // Since we are only adding elements to the HashSet, the order will be preserved.
        HashSet<string> distinctFormats =
#if NET
            new(entries.Length);
#else
            new();
#endif

        foreach (KeyValuePair<string, DataStoreEntry> entry in entries)
        {
            distinctFormats.Add(entry.Key);
            if (entry.Value.AutoConvert)
            {
                DataFormatNames.AddMappedFormats(entry.Key, distinctFormats);
            }
        }

        return [.. distinctFormats];
    }

    public string[] GetFormats() => GetFormats(autoConvert: true);

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        Func<TypeName, Type?> resolver,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
            TryGetDataInternal(format, autoConvert, out data);

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
            TryGetDataInternal(format, autoConvert, out data);

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
            TryGetDataInternal(format, autoConvert: false, out data);

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
            TryGetDataInternal(typeof(T).FullName!, autoConvert: false, out data);
}
