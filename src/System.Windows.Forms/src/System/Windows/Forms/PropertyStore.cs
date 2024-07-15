// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  This is a small class that can efficiently store property values.
///  It tries to optimize for size first, "get" access second, and
///  "set" access third.
/// </summary>
internal partial class PropertyStore
{
    private static int s_currentKey;

    private readonly Dictionary<int, Value> _values = [];

    /// <summary>
    ///  Retrieves an integer value from our property list.
    ///  This will set value to zero and return false if the
    ///  list does not contain the given key.
    /// </summary>
    public bool ContainsInteger(int key)
    {
        GetInteger(key, out bool found);
        return found;
    }

    /// <summary>
    ///  Retrieves an integer value from our property list.
    ///  This will set value to zero and return false if the
    ///  list does not contain the given key.
    /// </summary>
    public bool ContainsObject(int key)
    {
        GetObject(key, out bool found);
        return found;
    }

    /// <summary>
    ///  Creates a new key for this property store. This is NOT
    ///  guarded by any thread safety so if you are calling it on
    ///  multiple threads you should guard. For our purposes,
    ///  we're fine because this is designed to be called in a class
    ///  initializer, and we never have the same class hierarchy
    ///  initializing on multiple threads at once.
    /// </summary>
    public static int CreateKey() => s_currentKey++;

    public Color GetColor(int key) => GetColor(key, out _);

    /// <summary>
    ///  A wrapper around GetObject designed to reduce the boxing hit
    /// </summary>
    public Color GetColor(int key, out bool found)
    {
        if (_values.TryGetValue(key, out Value value))
        {
            found = true;
            return value.GetValue<Color>();
        }

        found = false;
        return Color.Empty;
    }

    /// <summary>
    ///  A wrapper around GetObject designed to reduce the boxing hit.
    /// </summary>
    public Padding GetPadding(int key, out bool found)
    {
        object? storedObject = GetObject(key, out found);
        if (found)
        {
            if (storedObject is PaddingWrapper wrapper)
            {
                return wrapper.Padding;
            }

            Debug.Assert(storedObject is null, $"Have non-null object that isn't a padding wrapper stored in a padding entry!{Environment.NewLine}Did someone SetObject instead of SetPadding?");
        }

        found = false;
        return Padding.Empty;
    }

    /// <summary>
    ///  A wrapper around GetObject designed to reduce the boxing hit.
    /// </summary>
    public Size GetSize(int key, out bool found)
    {
        if (_values.TryGetValue(key, out Value value))
        {
            found = true;
            return value.GetValue<Size>();
        }

        found = false;
        return Size.Empty;
    }

    /// <summary>
    ///  A wrapper around GetObject designed to reduce the boxing hit.
    /// </summary>
    public Rectangle GetRectangle(int key, out bool found)
    {
        object? storedObject = GetObject(key, out found);
        if (found)
        {
            if (storedObject is RectangleWrapper wrapper)
            {
                return wrapper.Rectangle;
            }

            Debug.Assert(storedObject is null, $"Have non-null object that isn't a Rectangle wrapper stored in a Rectangle entry!{Environment.NewLine}Did someone SetObject instead of SetRectangle?");
        }

        found = false;
        return Rectangle.Empty;
    }

    /// <summary>
    ///  Retrieves an integer value from our property list.
    ///  This will set value to zero and return false if the
    ///  list does not contain the given key.
    /// </summary>
    public int GetInteger(int key) => GetInteger(key, out _);

    /// <summary>
    ///  Retrieves an integer value from our property list.
    ///  This will set value to zero and return false if the
    ///  list does not contain the given key.
    /// </summary>
    public int GetInteger(int key, out bool found)
    {
        found = _values.TryGetValue(key, out Value value);
        return found ? value.GetValue<int>() : default;
    }

    /// <summary>
    ///  Retrieves an object value from our property list.
    ///  This will set value to null and return false if the
    ///  list does not contain the given key.
    /// </summary>
    public object? GetObject(int key) => GetObject(key, out _);

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

    public bool ContainsObjectThatIsNotNull(int key)
    {
        object? entry = GetObject(key, out bool found);
        return found && entry is not null;
    }

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
    ///  Removes the given key from the array
    /// </summary>
    public void RemoveInteger(int key) => _values.Remove(key);

    /// <summary>
    ///  Removes the given key from the array
    /// </summary>
    public void RemoveObject(int key) => _values.Remove(key);

    public void SetColor(int key, Color value) => _values[key] = value;

    public void SetPadding(int key, Padding value)
    {
        object? storedObject = GetObject(key, out bool found);
        if (!found)
        {
            SetObject(key, new PaddingWrapper(value));
        }
        else
        {
            if (storedObject is PaddingWrapper wrapper)
            {
                // re-using the wrapper reduces the boxing hit.
                wrapper.Padding = value;
            }
            else
            {
                Debug.Assert(storedObject is null, "object should either be null or PaddingWrapper"); // could someone have SetObject to this key behind our backs?
                SetObject(key, new PaddingWrapper(value));
            }
        }
    }

    public void SetRectangle(int key, Rectangle value)
    {
        if (!_values.TryGetValue(key, out Value foundValue))
        {
            _values[key] = new(new RectangleWrapper(value));
        }
        else
        {
            foundValue.GetValue<RectangleWrapper>().Rectangle = value;
        }
    }

    public void SetSize(int key, Size value) => _values[key] = value;

    /// <summary>
    ///  Stores the given value in the key.
    /// </summary>
    public void SetInteger(int key, int value) => _values[key] = value;

    /// <summary>
    ///  Stores the given value in the key.
    /// </summary>
    public void SetObject(int key, object? value) => _values[key] = new(value);
}
