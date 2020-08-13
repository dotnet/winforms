// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This is a small class that can efficiently store property values.
    ///  It tries to optimize for size first, "get" access second, and
    ///  "set" access third.
    /// </summary>
    internal class PropertyStore
    {
        private static int s_currentKey;

        private IntegerEntry[]? _intEntries;
        private ObjectEntry[]? _objEntries;

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
            object? storedObject = GetObject(key, out found);
            if (found)
            {
                if (storedObject is ColorWrapper wrapper)
                {
                    return wrapper.Color;
                }

                Debug.Assert(storedObject is null, $"Have non-null object that isnt a color wrapper stored in a color entry!{Environment.NewLine}Did someone SetObject instead of SetColor?");
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

                Debug.Assert(storedObject is null, $"Have non-null object that isnt a padding wrapper stored in a padding entry!{Environment.NewLine}Did someone SetObject instead of SetPadding?");
            }

            found = false;
            return Padding.Empty;
        }

        /// <summary>
        ///  A wrapper around GetObject designed to reduce the boxing hit.
        /// </summary>
        public Size GetSize(int key, out bool found)
        {
            object? storedObject = GetObject(key, out found);
            if (found)
            {
                if (storedObject is SizeWrapper wrapper)
                {
                    return wrapper.Size;
                }

                Debug.Assert(storedObject is null, $"Have non-null object that isnt a padding wrapper stored in a padding entry!{Environment.NewLine}Did someone SetObject instead of SetPadding?");
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

                Debug.Assert(storedObject is null, $"Have non-null object that isnt a Rectangle wrapper stored in a Rectangle entry!{Environment.NewLine}Did someone SetObject instead of SetRectangle?");
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
            short keyIndex = SplitKey(key, out short element);
            if (!LocateIntegerEntry(keyIndex, out int index))
            {
                found = false;
                return default;
            }

            // We have found the relevant entry. See if
            // the bitmask indicates the value is used.
            if (((1 << element) & _intEntries![index].Mask) == 0)
            {
                found = false;
                return default;
            }

            found = true;
            switch (element)
            {
                case 0:
                    return _intEntries[index].Value1;
                case 1:
                    return _intEntries[index].Value2;
                case 2:
                    return _intEntries[index].Value3;
                case 3:
                    return _intEntries[index].Value4;
                default:
                    Debug.Fail("Invalid element obtained from LocateIntegerEntry");
                    return default;
            }
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
        public object? GetObject(int key, out bool found)
        {
            short keyIndex = SplitKey(key, out short element);
            if (!LocateObjectEntry(keyIndex, out int index))
            {
                found = false;
                return null;
            }

            // We have found the relevant entry. See if
            // the bitmask indicates the value is used.
            if (((1 << element) & _objEntries![index].Mask) == 0)
            {
                found = false;
                return null;
            }

            found = true;
            switch (element)
            {
                case 0:
                    return _objEntries[index].Value1;
                case 1:
                    return _objEntries[index].Value2;
                case 2:
                    return _objEntries[index].Value3;
                case 3:
                    return _objEntries[index].Value4;
                default:
                    Debug.Fail("Invalid element obtained from LocateObjectEntry");
                    return null;
            }
        }

        /// <summary>
        ///  Locates the requested entry in our array if entries. This does
        ///  not do the mask check to see if the entry is currently being used,
        ///  but it does locate the entry. If the entry is found, this returns
        ///  true and fills in index and element. If the entry is not found,
        ///  this returns false. If the entry is not found, index will contain
        ///  the insert point at which one would add a new element.
        /// </summary>
        [MemberNotNullWhen(true, nameof(_intEntries))]
        private bool LocateIntegerEntry(short entryKey, out int index)
        {
            if (_intEntries is null)
            {
                index = 0;
                return false;
            }

            int length = _intEntries.Length;
            if (length <= 16)
            {
                // If the array is small enough, we unroll the binary search to be more efficient.
                // Usually the performance gain is around 10% to 20%
                // DON'T change this code unless you are very confident!
                index = 0;
                int midPoint = length / 2;
                if (_intEntries[midPoint].Key <= entryKey)
                {
                    index = midPoint;
                }

                // We don't move this inside the previous if branch since this catches both
                // the case index == 0 and index = midPoint
                if (_intEntries[index].Key == entryKey)
                {
                    return true;
                }

                midPoint = (length + 1) / 4;
                if (_intEntries[index + midPoint].Key <= entryKey)
                {
                    index += midPoint;
                    if (_intEntries[index].Key == entryKey)
                    {
                        return true;
                    }
                }

                midPoint = (length + 3) / 8;
                if (_intEntries[index + midPoint].Key <= entryKey)
                {
                    index += midPoint;
                    if (_intEntries[index].Key == entryKey)
                    {
                        return true;
                    }
                }

                midPoint = (length + 7) / 16;
                if (_intEntries[index + midPoint].Key <= entryKey)
                {
                    index += midPoint;
                    if (_intEntries[index].Key == entryKey)
                    {
                        return true;
                    }
                }

                Debug.Assert(index < length);
                if (entryKey > _intEntries[index].Key)
                {
                    index++;
                }

                Debug_VerifyLocateIntegerEntry(index, entryKey, length);
                return false;
            }
            else
            {
                // Entries are stored in numerical order by key index so we can
                // do a binary search on them.
                int max = length - 1;
                int min = 0;
                int idx = 0;

                do
                {
                    idx = (max + min) / 2;
                    short currentKeyIndex = _intEntries[idx].Key;

                    if (currentKeyIndex == entryKey)
                    {
                        index = idx;
                        return true;
                    }
                    else if (entryKey < currentKeyIndex)
                    {
                        max = idx - 1;
                    }
                    else
                    {
                        min = idx + 1;
                    }
                }
                while (max >= min);

                // Didn't find the index. Setup our output appropriately
                index = idx;
                if (entryKey > _intEntries[idx].Key)
                {
                    index++;
                }

                return false;
            }
        }

        /// <summary>
        ///  Locates the requested entry in our array if entries. This does
        ///  not do the mask check to see if the entry is currently being used,
        ///  but it does locate the entry. If the entry is found, this returns
        ///  true and fills in index and element. If the entry is not found,
        ///  this returns false. If the entry is not found, index will contain
        ///  the insert point at which one would add a new element.
        /// </summary>
        [MemberNotNullWhen(true, nameof(_objEntries))]
        private bool LocateObjectEntry(short entryKey, out int index)
        {
            if (_objEntries != null)
            {
                int length = _objEntries.Length;
                Debug.Assert(length > 0);
                if (length <= 16)
                {
                    // If the array is small enough, we unroll the binary search to be more efficient.
                    // Usually the performance gain is around 10% to 20%
                    // DON'T change this code unless you are very confident!
                    index = 0;
                    int midPoint = length / 2;
                    if (_objEntries[midPoint].Key <= entryKey)
                    {
                        index = midPoint;
                    }

                    // We don't move this inside the previous if branch since this catches
                    // both the case index == 0 and index = midPoint
                    if (_objEntries[index].Key == entryKey)
                    {
                        return true;
                    }

                    midPoint = (length + 1) / 4;
                    if (_objEntries[index + midPoint].Key <= entryKey)
                    {
                        index += midPoint;
                        if (_objEntries[index].Key == entryKey)
                        {
                            return true;
                        }
                    }

                    midPoint = (length + 3) / 8;
                    if (_objEntries[index + midPoint].Key <= entryKey)
                    {
                        index += midPoint;
                        if (_objEntries[index].Key == entryKey)
                        {
                            return true;
                        }
                    }

                    midPoint = (length + 7) / 16;
                    if (_objEntries[index + midPoint].Key <= entryKey)
                    {
                        index += midPoint;
                        if (_objEntries[index].Key == entryKey)
                        {
                            return true;
                        }
                    }

                    Debug.Assert(index < length);
                    if (entryKey > _objEntries[index].Key)
                    {
                        index++;
                    }

                    Debug_VerifyLocateObjectEntry(index, entryKey, length);
                    return false;
                }
                else
                {
                    // Entries are stored in numerical order by key index so we can
                    // do a binary search on them.
                    int max = length - 1;
                    int min = 0;
                    int idx = 0;

                    do
                    {
                        idx = (max + min) / 2;
                        short currentKeyIndex = _objEntries[idx].Key;

                        if (currentKeyIndex == entryKey)
                        {
                            index = idx;
                            return true;
                        }
                        else if (entryKey < currentKeyIndex)
                        {
                            max = idx - 1;
                        }
                        else
                        {
                            min = idx + 1;
                        }
                    }
                    while (max >= min);

                    // Didn't find the index. Setup our output appropriately
                    index = idx;
                    if (entryKey > _objEntries[idx].Key)
                    {
                        index++;
                    }

                    return false;
                }
            }
            else
            {
                index = 0;
                return false;
            }
        }

        /// <summary>
        ///  Removes the given key from the array
        /// </summary>
        public void RemoveInteger(int key)
        {
            short entryKey = SplitKey(key, out short element);
            if (!LocateIntegerEntry(entryKey, out int index))
            {
                return;
            }

            if (((1 << element) & _intEntries![index].Mask) == 0)
            {
                // this element is not being used - return right away
                return;
            }

            // declare that the element is no longer used
            _intEntries[index].Mask &= (short)(~((short)(1 << element)));

            if (_intEntries[index].Mask == 0)
            {
                // This object entry is no longer in use - let's remove it all together
                // not great for perf but very simple and we don't expect to remove much
                IntegerEntry[] newEntries = new IntegerEntry[_intEntries.Length - 1];
                if (index > 0)
                {
                    Array.Copy(_intEntries, 0, newEntries, 0, index);
                }
                if (index < newEntries.Length)
                {
                    Debug.Assert(_intEntries.Length - index - 1 > 0);
                    Array.Copy(_intEntries, index + 1, newEntries, index, _intEntries.Length - index - 1);
                }

                _intEntries = newEntries;
            }
            else
            {
                // This object entry is still in use - let's just clean up the deleted element
                switch (element)
                {
                    case 0:
                        _intEntries[index].Value1 = 0;
                        break;

                    case 1:
                        _intEntries[index].Value2 = 0;
                        break;

                    case 2:
                        _intEntries[index].Value3 = 0;
                        break;

                    case 3:
                        _intEntries[index].Value4 = 0;
                        break;

                    default:
                        Debug.Fail("Invalid element obtained from LocateIntegerEntry");
                        break;
                }
            }
        }

        /// <summary>
        ///  Removes the given key from the array
        /// </summary>
        public void RemoveObject(int key)
        {
            short entryKey = SplitKey(key, out short element);
            if (!LocateObjectEntry(entryKey, out int index))
            {
                return;
            }

            if (((1 << element) & _objEntries![index].Mask) == 0)
            {
                // This element is not being used - return right away
                return;
            }

            // Declare that the element is no longer used
            _objEntries[index].Mask &= (short)(~((short)(1 << element)));

            if (_objEntries[index].Mask == 0)
            {
                // This object entry is no longer in use - let's remove it all together
                // not great for perf but very simple and we don't expect to remove much
                if (_objEntries.Length == 1)
                {
                    // Instead of allocating an array of length 0, we simply reset the array to null.
                    _objEntries = null;
                }
                else
                {
                    ObjectEntry[] newEntries = new ObjectEntry[_objEntries.Length - 1];
                    if (index > 0)
                    {
                        Array.Copy(_objEntries, 0, newEntries, 0, index);
                    }
                    if (index < newEntries.Length)
                    {
                        Debug.Assert(_objEntries.Length - index - 1 > 0);
                        Array.Copy(_objEntries, index + 1, newEntries, index, _objEntries.Length - index - 1);
                    }
                    _objEntries = newEntries;
                }
            }
            else
            {
                // This object entry is still in use - let's just clean up the deleted element
                switch (element)
                {
                    case 0:
                        _objEntries[index].Value1 = null;
                        break;

                    case 1:
                        _objEntries[index].Value2 = null;
                        break;

                    case 2:
                        _objEntries[index].Value3 = null;
                        break;

                    case 3:
                        _objEntries[index].Value4 = null;
                        break;

                    default:
                        Debug.Fail("Invalid element obtained from LocateObjectEntry");
                        break;
                }
            }
        }

        public void SetColor(int key, Color value)
        {
            object? storedObject = GetObject(key, out bool found);
            if (!found)
            {
                SetObject(key, new ColorWrapper(value));
            }
            else
            {
                if (storedObject is ColorWrapper wrapper)
                {
                    // re-using the wrapper reduces the boxing hit.
                    wrapper.Color = value;
                }
                else
                {
                    Debug.Assert(storedObject is null, "object should either be null or ColorWrapper"); // could someone have SetObject to this key behind our backs?
                    SetObject(key, new ColorWrapper(value));
                }
            }
        }

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
            object? storedObject = GetObject(key, out bool found);
            if (!found)
            {
                SetObject(key, new RectangleWrapper(value));
            }
            else
            {
                if (storedObject is RectangleWrapper wrapper)
                {
                    // re-using the wrapper reduces the boxing hit.
                    wrapper.Rectangle = value;
                }
                else
                {
                    Debug.Assert(storedObject is null, "object should either be null or RectangleWrapper"); // could someone have SetObject to this key behind our backs?
                    SetObject(key, new RectangleWrapper(value));
                }
            }
        }

        public void SetSize(int key, Size value)
        {
            object? storedObject = GetObject(key, out bool found);
            if (!found)
            {
                SetObject(key, new SizeWrapper(value));
            }
            else
            {
                if (storedObject is SizeWrapper wrapper)
                {
                    // re-using the wrapper reduces the boxing hit.
                    wrapper.Size = value;
                }
                else
                {
                    Debug.Assert(storedObject is null, "object should either be null or SizeWrapper"); // could someone have SetObject to this key behind our backs?
                    SetObject(key, new SizeWrapper(value));
                }
            }
        }

        /// <summary>
        ///  Stores the given value in the key.
        /// </summary>
        public void SetInteger(int key, int value)
        {
            short entryKey = SplitKey(key, out short element);
            if (!LocateIntegerEntry(entryKey, out int index))
            {
                // We must allocate a new entry.
                if (_intEntries != null)
                {
                    IntegerEntry[] newEntries = new IntegerEntry[_intEntries.Length + 1];

                    if (index > 0)
                    {
                        Array.Copy(_intEntries, 0, newEntries, 0, index);
                    }

                    if (_intEntries.Length - index > 0)
                    {
                        Array.Copy(_intEntries, index, newEntries, index + 1, _intEntries.Length - index);
                    }

                    _intEntries = newEntries;
                }
                else
                {
                    _intEntries = new IntegerEntry[1];
                    Debug.Assert(index == 0, "LocateIntegerEntry should have given us a zero index.");
                }

                _intEntries[index].Key = entryKey;
            }

            // Now determine which value to set.
            switch (element)
            {
                case 0:
                    _intEntries![index].Value1 = value;
                    break;

                case 1:
                    _intEntries![index].Value2 = value;
                    break;

                case 2:
                    _intEntries![index].Value3 = value;
                    break;

                case 3:
                    _intEntries![index].Value4 = value;
                    break;

                default:
                    Debug.Fail("Invalid element obtained from LocateIntegerEntry");
                    break;
            }

            _intEntries[index].Mask = (short)((1 << element) | (ushort)(_intEntries[index].Mask));
        }

        /// <summary>
        ///  Stores the given value in the key.
        /// </summary>
        public void SetObject(int key, object? value)
        {
            short entryKey = SplitKey(key, out short element);
            if (!LocateObjectEntry(entryKey, out int index))
            {
                // We must allocate a new entry.
                if (_objEntries != null)
                {
                    ObjectEntry[] newEntries = new ObjectEntry[_objEntries.Length + 1];

                    if (index > 0)
                    {
                        Array.Copy(_objEntries, 0, newEntries, 0, index);
                    }

                    if (_objEntries.Length - index > 0)
                    {
                        Array.Copy(_objEntries, index, newEntries, index + 1, _objEntries.Length - index);
                    }

                    _objEntries = newEntries;
                }
                else
                {
                    _objEntries = new ObjectEntry[1];
                    Debug.Assert(index == 0, "LocateObjectEntry should have given us a zero index.");
                }

                _objEntries[index].Key = entryKey;
            }

            // Now determine which value to set.
            switch (element)
            {
                case 0:
                    _objEntries![index].Value1 = value;
                    break;

                case 1:
                    _objEntries![index].Value2 = value;
                    break;

                case 2:
                    _objEntries![index].Value3 = value;
                    break;

                case 3:
                    _objEntries![index].Value4 = value;
                    break;

                default:
                    Debug.Fail("Invalid element obtained from LocateObjectEntry");
                    break;
            }

            _objEntries[index].Mask = (short)((ushort)(_objEntries[index].Mask) | (1 << element));
        }

        /// <summary>
        ///  Takes the given key and splits it into an index and an element.
        /// </summary>
        private short SplitKey(int key, out short element)
        {
            element = (short)(key & 0x00000003);
            return (short)(key & 0xFFFFFFFC);
        }

        [Conditional("DEBUG_PROPERTYSTORE")]
        private void Debug_VerifyLocateIntegerEntry(int index, short entryKey, int length)
        {
            int max = length - 1;
            int min = 0;
            int idx = 0;

            do
            {
                idx = (max + min) / 2;
                short currentKeyIndex = _intEntries![idx].Key;

                if (currentKeyIndex == entryKey)
                {
                    Debug.Assert(index == idx, "GetIntegerEntry in property store broken. index is " + index + " while it should be " + idx + "length of the array is " + length);
                }
                else if (entryKey < currentKeyIndex)
                {
                    max = idx - 1;
                }
                else
                {
                    min = idx + 1;
                }
            }
            while (max >= min);

            // shouldn't find the index if we run this debug code
            if (entryKey > _intEntries[idx].Key)
            {
                idx++;
            }
            Debug.Assert(index == idx, "GetIntegerEntry in property store broken. index is " + index + " while it should be " + idx + "length of the array is " + length);
        }

        [Conditional("DEBUG_PROPERTYSTORE")]
        private void Debug_VerifyLocateObjectEntry(int index, short entryKey, int length)
        {
            int max = length - 1;
            int min = 0;
            int idx = 0;

            do
            {
                idx = (max + min) / 2;
                short currentKeyIndex = _objEntries![idx].Key;

                if (currentKeyIndex == entryKey)
                {
                    Debug.Assert(index == idx, "GetObjEntry in property store broken. index is " + index + " while is should be " + idx + "length of the array is " + length);
                }
                else if (entryKey < currentKeyIndex)
                {
                    max = idx - 1;
                }
                else
                {
                    min = idx + 1;
                }
            }
            while (max >= min);

            if (entryKey > _objEntries[idx].Key)
            {
                idx++;
            }
            Debug.Assert(index == idx, "GetObjEntry in property store broken. index is " + index + " while is should be " + idx + "length of the array is " + length);
        }

        /// <summary>
        ///  Stores the relationship between a key and a value.
        ///  We do not want to be so inefficient that we require
        ///  four bytes for each four byte property, so use an algorithm
        ///  that uses the bottom two bits of the key to identify
        ///  one of four elements in an entry.
        /// </summary>
        private struct IntegerEntry
        {
            public short Key;
            public short Mask;  // only lower four bits are used; mask of used values.
            public int Value1;
            public int Value2;
            public int Value3;
            public int Value4;
        }

        /// <summary>
        ///  Stores the relationship between a key and a value.
        ///  We do not want to be so inefficient that we require
        ///  four bytes for each four byte property, so use an algorithm
        ///  that uses the bottom two bits of the key to identify
        ///  one of four elements in an entry.
        /// </summary>
        private struct ObjectEntry
        {
            public short Key;
            public short Mask;  // only lower four bits are used; mask of used values.
            public object? Value1;
            public object? Value2;
            public object? Value3;
            public object? Value4;
        }

        private sealed class ColorWrapper
        {
            public Color Color;

            public ColorWrapper(Color color)
            {
                Color = color;
            }
        }

        private sealed class PaddingWrapper
        {
            public Padding Padding;

            public PaddingWrapper(Padding padding)
            {
                Padding = padding;
            }
        }

        private sealed class RectangleWrapper
        {
            public Rectangle Rectangle;

            public RectangleWrapper(Rectangle rectangle)
            {
                Rectangle = rectangle;
            }
        }

        private sealed class SizeWrapper
        {
            public Size Size;

            public SizeWrapper(Size size)
            {
                Size = size;
            }
        }
    }
}
