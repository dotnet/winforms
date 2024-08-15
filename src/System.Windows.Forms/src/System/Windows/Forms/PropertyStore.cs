// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms;

/// <summary>
///  Efficient property store that avoids boxing for common value types.
/// </summary>
internal class PropertyStore
{
    private static int s_currentKey;

    private readonly Dictionary<int, Value> _values = [];

    /// <summary>
    ///  Returns <see langword="true"/> if the current key has a value in the <see cref="PropertyStore"/>.
    /// </summary>
    public bool ContainsKey(int key) => _values.ContainsKey(key);

    /// <summary>
    ///  Creates a new key for this property store.
    /// </summary>
    public static int CreateKey() => s_currentKey++;

    // REMOVE
    /// <summary>
    ///  Retrieves an object value from our property list.
    ///  This will set value to null and return false if the
    ///  list does not contain the given key.
    /// </summary>
    public object? GetObject(int key) => GetObject(key, out _);

    // REMOVE
    /// <summary>
    ///  Retrieves an object value from our property list.
    ///  This will set value to null and return false if the
    ///  list does not contain the given key.
    /// </summary>
    /// <typeparam name="T">The type of object to retrieve.</typeparam>
    /// <param name="key">The key corresponding to the object in the property list.</param>
    /// <param name="value">Output parameter where the object will be set if found.
    ///  Will be set to null if the key is not present.</param>
    /// <remarks><para>If a null value is set for a given key
    ///  it will return true and a null value.</para></remarks>
    /// <returns>True if an object (including null) is found for the given key; otherwise, false.</returns>
    public bool TryGetObject<T>(int key, out T? value)
    {
        object? entry = GetObject(key, out bool found);
        Debug.Assert(!found || entry is null || entry is T, $"Entry is not of type {typeof(T)}, but of type {entry?.GetType()}");
        if (typeof(T).IsValueType || typeof(T).IsEnum || typeof(T).IsPrimitive)
        {
            value = found && entry is not null ? (T?)entry : default;
            return found;
        }

        value = found ? (T?)entry : default;
        return found;
    }

    // REMOVE
    public bool ContainsObjectThatIsNotNull(int key)
    {
        object? entry = GetObject(key, out bool found);
        return found && entry is not null;
    }

    // REMOVE
    /// <summary>
    ///  Retrieves an object value from our property list.
    ///  This will set value to null and return false if the
    ///  list does not contain the given key.
    /// </summary>
    public object? GetObject(int key, out bool found)
    {
        found = _values.TryGetValue(key, out Value value);
        return found ? value.GetValue<object?>() : null;
    }

    /// <summary>
    ///  Removes the given key from the store.
    /// </summary>
    public void RemoveValue(int key) => _values.Remove(key);

    // REMOVE
    /// <summary>
    ///  Stores the given value in the key.
    /// </summary>
    public void SetObject(int key, object? value) => _values[key] = new(value);

    /// <summary>
    ///  Gets the current value for the given key, or the default value for the type if the key is not found.
    /// </summary>
    public T? GetValueOrDefault<T>(int key, T? defaultValue = default)
    {
        if (_values.TryGetValue(key, out Value foundValue))
        {
            return foundValue.GetValue<T>();
        }

        return defaultValue;
    }

    /// <summary>
    ///  Gets the current string value for the given key, or <see cref="string.Empty"/> if the key is not found.
    /// </summary>
    public string GetStringOrEmptyString(int key)
    {
        if (_values.TryGetValue(key, out Value foundValue))
        {
            return foundValue.GetValue<string>();
        }

        return string.Empty;
    }

    /// <summary>
    ///  Tries to get the value for the given key. Returns <see langword="true"/> if the value was found and was
    ///  not <see langword="null"/>.
    /// </summary>
    public bool TryGetValue<T>(int key, [NotNullWhen(true)] out T? value)
    {
        if (_values.TryGetValue(key, out Value foundValue))
        {
            value = foundValue.GetValue<T>();
            return value is not null;
        }

        value = default;
        return false;
    }

    // Ideally we can get rid of this one and just clear values when they are null.

    /// <summary>
    ///  Tries to get the value for the given key, allowing explicitly set <see langword="null"/> values.
    ///  Returns <see langword="true"/> if the value was found.
    /// </summary>
    public bool TryGetValueOrNull<T>(int key, out T? value) where T : class
    {
        if (_values.TryGetValue(key, out Value foundValue))
        {
            value = foundValue.GetValue<T>();
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    ///  Previous will be `string.Empty` if not set. Setting `string.Empty` will clear the value.
    /// </summary>
    public string AddOrRemoveString(int key, string? value)
    {
        TryGetValue(key, out string? previous);
        if (string.IsNullOrEmpty(value))
        {
            _values.Remove(key);
        }
        else
        {
            _values[key] = new(value);
        }

        return previous ?? string.Empty;
    }

    /// <summary>
    ///  Sets the given value or clears it from the store if the value is <see langword="null"/>.
    ///  Returns the previous value if it was set, or <see langword="null"/>.
    /// </summary>
    public T? AddOrRemoveValue<T>(int key, T? value)
    {
        TryGetValue(key, out T? previous);
        if (value is null || EqualityComparer<T>.Default.Equals(value, default))
        {
            _values.Remove(key);
        }
        else
        {
            _values[key] = new(value);
        }

        return previous;
    }

    /// <summary>
    ///  Adds the given value to the store.
    /// </summary>
    public void AddValue<T>(int key, T value)
    {
        // For value types that are larger than 8 bytes, we attempt to update the existing value
        // to avoid another boxing allocation.

        if (typeof(T) == typeof(Padding))
        {
            AddOrUpdate(key, Unsafe.As<T, Padding>(ref value));
        }
        else if (typeof(T) == typeof(Rectangle))
        {
            AddOrUpdate(key, Unsafe.As<T, Rectangle>(ref value));
        }
        else
        {
            _values[key] = Value.Create(value);
        }
    }

    private unsafe void AddOrUpdate<T>(int key, T value) where T : unmanaged
    {
        // Should only call this from SetValue<T> for value types that are larger than 8 bytes.
        Debug.Assert(sizeof(T) > 8);

        if (_values.TryGetValue(key, out Value foundValue))
        {
            object storedValue = foundValue.GetValue<object>();
            ref T unboxed = ref Unsafe.Unbox<T>(storedValue);
            unboxed = value;
        }
        else
        {
            _values[key] = Value.Create(value);
        }
    }
}
