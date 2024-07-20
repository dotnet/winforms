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
    public static int CreateKey() => Interlocked.Increment(ref s_currentKey);

    // REMOVE
    /// <summary>
    ///  Retrieves an object value from our property list.
    ///  This will set value to null and return false if the
    ///  list does not contain the given key.
    /// </summary>
    public object? GetObject(int key) => GetObject(key, out _);

    // REMOVE
    public bool ContainsObjectThatIsNotNull(int key)
    {
        return TryGetValue(key, out object? entry) && entry is not null;
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
    public T? GetValueOrDefault<T>(int key)
    {
        if (_values.TryGetValue(key, out Value foundValue))
        {
            return foundValue.GetValue<T>();
        }

        return default;
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

    /// <summary>
    ///  Sets the given value or clears it from the store if the value is <see langword="null"/>.
    ///  Returns the previous value if it was set, or <see langword="null"/>.
    /// </summary>
    public T? AddOrRemoveValue<T>(int key, T? value) where T : class
    {
        TryGetValue(key, out T? previous);
        if (value is null)
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
