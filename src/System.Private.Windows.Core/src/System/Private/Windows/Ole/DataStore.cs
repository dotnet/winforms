// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.Reflection.Metadata;
using System.Runtime.Serialization;

namespace System.Private.Windows.Ole;

internal sealed partial class DataStore<TOleServices> : IDataObjectInternal where TOleServices : IOleServices
{
    private readonly Dictionary<string, DataStoreEntry> _mappedData = new(BackCompatibleStringComparer.Default);

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
            if (_mappedData.TryGetValue(format, out DataStoreEntry entry))
            {
                autoConvert |= entry.AutoConvert;

                if (entry.Data is T value)
                {
                    data = value;
                    return true;
                }
                else if (entry.Data is JsonData<T> jsonData)
                {
                    data = (T)jsonData.Deserialize();
                    return true;
                }
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
        ArgumentException.ThrowIfNullOrWhiteSpace(format, nameof(format));
        TOleServices.ValidateDataStoreData(ref format, autoConvert, data);
        _mappedData[format] = new DataStoreEntry(data, autoConvert);
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

        if (data is ISerializable && !_mappedData.ContainsKey(DataFormatNames.Serializable))
        {
            SetData(DataFormatNames.Serializable, data);
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
            return _mappedData.ContainsKey(format);
        }

        string[] formats = GetFormats(autoConvert);

        for (int i = 0; i < formats.Length; i++)
        {
            if (format.Equals(formats[i]))
            {
                return true;
            }
        }

        return false;
    }

    public bool GetDataPresent(string format) => GetDataPresent(format, autoConvert: true);

    public string[] GetFormats(bool autoConvert)
    {
        string[] definedFormats = new string[_mappedData.Keys.Count];
        _mappedData.Keys.CopyTo(definedFormats, 0);

        if (autoConvert)
        {
            // Since we are only adding elements to the HashSet, the order will be preserved.
            int definedCount = definedFormats.Length;
            HashSet<string> distinctFormats = new(definedCount);
            for (int i = 0; i < definedCount; i++)
            {
                string current = definedFormats[i];
                if (_mappedData[current].AutoConvert)
                {
                    distinctFormats.Add(current);
                    DataFormatNames.AddMappedFormats(current, distinctFormats);
                }
                else
                {
                    distinctFormats.Add(current);
                }
            }

            definedFormats = [.. distinctFormats];
        }

        return definedFormats;
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
