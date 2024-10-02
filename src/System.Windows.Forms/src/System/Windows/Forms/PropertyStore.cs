// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms;

/// <summary>
///  Efficient property store that avoids boxing for common value types.
/// </summary>
/// <remarks>
///  <para>
///   This class discourages storing <see langword="null"/> values.
///  </para>
/// </remarks>
internal sealed class PropertyStore
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

    /// <summary>
    ///  Removes the given key from the store.
    /// </summary>
    public void RemoveValue(int key) => _values.Remove(key);

    /// <summary>
    ///  Gets the current value for the given key, or the <paramref name="defaultValue"/> if the key is not found.
    ///  Does not allow stored values of <see langword="null"/>.
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
    ///  Gets the current value for the given key, or the <paramref name="defaultValue"/> if the key is not found.
    ///  If the stored value is <see langword="null"/>, it will return <see langword="null"/>.
    /// </summary>
    public T? GetValueOrDefaultAllowNull<T>(int key, T? defaultValue = default) where T : class?
    {
        if (_values.TryGetValue(key, out Value foundValue))
        {
            return foundValue.Type is null ? null : foundValue.GetValue<T>();
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
    ///  Tries to get the value for the given key. Use <see cref="TryGetValueOrNull{T}(int, out T)"/> if
    ///  <see langword="null"/> values are allowed.
    /// </summary>
    /// <inheritdoc cref="TryGetValueOrNull{T}(int, out T)"/>
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
    ///  Tries to get the value for the given key, allowing explicitly set <see langword="null"/> values. Prefer
    ///  <see cref="TryGetValue{T}(int, out T)"/> if <see langword="null"/> values are not allowed.
    /// </summary>
    /// <param name="value">
    ///  <para>
    ///   The value if found, or <see langword="default"/> if not found.
    ///  </para>
    /// </param>
    /// <returns><see langword="true"/> if the value was found.</returns>
    public bool TryGetValueOrNull<T>(int key, out T? value) where T : class
    {
        if (_values.TryGetValue(key, out Value foundValue))
        {
            value = foundValue.Type is null ? null : foundValue.GetValue<T>();
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    ///  Setting <see langword="null"/> or <see cref="string.Empty"/> will clear the value.
    /// </summary>
    /// <returns>
    ///  <see langword="true"/> if the stored value was changed.
    /// </returns>
    public bool AddOrRemoveString(int key, string? value)
    {
        bool found = TryGetValue(key, out string? previous);
        bool changed = false;

        if (string.IsNullOrEmpty(value))
        {
            if (found)
            {
                _values.Remove(key);
                changed = true;
            }
        }
        else if (previous != value)
        {
            _values[key] = new(value);
            changed = true;
        }

        return changed;
    }

    /// <summary>
    ///  Sets the given value or clears it from the store if the value equals <paramref name="defaultValue"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Always explicitly set <paramref name="defaultValue"/> when using enums for clarity.
    ///  </para>
    /// </remarks>
    /// <returns>The previous value if it was set, or <paramref name="defaultValue"/>.</returns>
    public T? AddOrRemoveValue<T>(int key, T? value, T? defaultValue = default)
    {
        bool found = _values.TryGetValue(key, out Value foundValue);

        bool isDefault = (value is null && defaultValue is null)
            || (value is not null && value.Equals(defaultValue));

        // The previous should be whatever we found or what was specified as the default.
        T? previous = found
            ? foundValue.Type is null ? default : foundValue.GetValue<T>()
            : defaultValue;

        if (isDefault)
        {
            // Equivalent to default, remove if we found it.
            if (found)
            {
                _values.Remove(key);
            }
        }
        else if (!found || !ReferenceEquals(value, previous))
        {
            // If it wasn't found or it is the same instance we don't need set it.
            _values[key] = new(value);
        }

        return previous;
    }

    /// <summary>
    ///  Adds the given value to the store.
    /// </summary>
    [return: NotNullIfNotNull(nameof(value))]
    public T AddValue<T>(int key, T value)
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

        return value;
    }

    private unsafe void AddOrUpdate<T>(int key, T value) where T : unmanaged
    {
        // Should only call this from SetValue<T> for value types that are larger than 8 bytes.
        Debug.Assert(sizeof(T) > 8);

        if (_values.TryGetValue(key, out Value foundValue) && foundValue.Type == typeof(T))
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
