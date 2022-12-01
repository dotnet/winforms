﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides a type converter to convert <see cref="Keys"/> objects to and from various
    ///  other representations.
    /// </summary>
    public class KeysConverter : TypeConverter, IComparer
    {
        private IDictionary<string, Keys>? _keyNames;
        private List<string>? _displayOrder;
        private StandardValuesCollection? _values;

        [MemberNotNull(nameof(_keyNames))]
        [MemberNotNull(nameof(_displayOrder))]
        private void Initialize()
        {
            if (_keyNames is not null && _displayOrder is not null)
            {
                return;
            }

            Debug.Assert(_displayOrder is null);
            Debug.Assert(_keyNames is null);

            _keyNames = new Dictionary<string, Keys>(34);
            _displayOrder = new List<string>(34);

            AddKey(SR.toStringEnter, Keys.Return);
            AddKey("F12", Keys.F12);
            AddKey("F11", Keys.F11);
            AddKey("F10", Keys.F10);
            AddKey(SR.toStringEnd, Keys.End);
            AddKey(SR.toStringControl, Keys.Control);
            AddKey("F8", Keys.F8);
            AddKey("F9", Keys.F9);
            AddKey(SR.toStringAlt, Keys.Alt);
            AddKey("F4", Keys.F4);
            AddKey("F5", Keys.F5);
            AddKey("F6", Keys.F6);
            AddKey("F7", Keys.F7);
            AddKey("F1", Keys.F1);
            AddKey("F2", Keys.F2);
            AddKey("F3", Keys.F3);
            AddKey(SR.toStringPageDown, Keys.Next);
            AddKey(SR.toStringInsert, Keys.Insert);
            AddKey(SR.toStringHome, Keys.Home);
            AddKey(SR.toStringDelete, Keys.Delete);
            AddKey(SR.toStringShift, Keys.Shift);
            AddKey(SR.toStringPageUp, Keys.Prior);
            AddKey(SR.toStringBack, Keys.Back);

            //new whidbey keys follow here...
            // Add string mappings for these values (numbers 0-9) so that the keyboard shortcuts
            // will be displayed properly in menus.
            AddKey("0", Keys.D0);
            AddKey("1", Keys.D1);
            AddKey("2", Keys.D2);
            AddKey("3", Keys.D3);
            AddKey("4", Keys.D4);
            AddKey("5", Keys.D5);
            AddKey("6", Keys.D6);
            AddKey("7", Keys.D7);
            AddKey("8", Keys.D8);
            AddKey("9", Keys.D9);

            void AddKey(string key, Keys value)
            {
                _keyNames[key] = value;
                _displayOrder.Add(key);
            }
        }

        /// <summary>
        ///  Access to a lookup table of name/value pairs for keys.  These are localized
        ///  names.
        /// </summary>
        [MemberNotNull(nameof(_keyNames))]
        [MemberNotNull(nameof(_displayOrder))]
        private IDictionary<string, Keys> KeyNames
        {
            get
            {
                Initialize();
                return _keyNames;
            }
        }

        [MemberNotNull(nameof(_keyNames))]
        [MemberNotNull(nameof(_displayOrder))]
        private List<string> DisplayOrder
        {
            get
            {
                Initialize();
                return _displayOrder;
            }
        }

        /// <summary>
        ///  Determines if this converter can convert an object in the given source
        ///  type to the native type of the converter.
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || sourceType == typeof(Enum[]) ? true : base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        ///  Gets a value indicating whether this converter can
        ///  convert an object to the given destination type using the context.
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            return destinationType == typeof(Enum[]) ? true : base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        ///  Compares two key values for equivalence.
        /// </summary>
        public int Compare(object? a, object? b)
        {
            return string.Compare(ConvertToString(a), ConvertToString(b), false, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///  Converts the given object to the converter's native type.
        /// </summary>
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string valueAsString)
            {
                string text = valueAsString.Trim();

                if (text.Length == 0)
                {
                    return null;
                }
                else
                {
                    // Parse an array of key tokens.
                    string[] tokens = text.Split(new char[] { '+' });
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        tokens[i] = tokens[i].Trim();
                    }

                    // Now lookup each key token in our key hashtable.
                    Keys key = (Keys)0;
                    bool foundKeyCode = false;

                    for (int i = 0; i < tokens.Length; i++)
                    {
                        if (!KeyNames.TryGetValue(tokens[i], out Keys currentKey))
                        {
                            // Key was not found in our dictionary.  See if it is a valid value in
                            // the Keys enum.
                            currentKey = (Keys)Enum.Parse(typeof(Keys), tokens[i]);
                        }

                        if ((currentKey & Keys.KeyCode) != 0)
                        {
                            // We found a match.  If we have previously found a
                            // key code, then check to see that this guy
                            // isn't a key code (it is illegal to have, say,
                            // "A + B"
                            if (foundKeyCode)
                            {
                                throw new FormatException(SR.KeysConverterInvalidKeyCombination);
                            }

                            foundKeyCode = true;
                        }

                        // Now OR the key into our current key
                        key |= currentKey;
                    }

                    return (object)key;
                }
            }
            else if (value is Enum[] valueAsEnumArray)
            {
                long finalValue = 0;
                foreach (Enum e in valueAsEnumArray)
                {
                    finalValue |= Convert.ToInt64(e, CultureInfo.InvariantCulture);
                }

                return Enum.ToObject(typeof(Keys), finalValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        ///  Converts the given object to another type.  The most common types to convert
        ///  are to and from a string object.  The default implementation will make a call
        ///  to ToString on the object if the object is valid and if the destination
        ///  type is string.  If this cannot convert to the destination type, this will
        ///  throw a NotSupportedException.
        /// </summary>
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            ArgumentNullException.ThrowIfNull(destinationType);

            if (value is not Keys and not int)
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            bool asString = destinationType == typeof(string);
            bool asEnum = !asString && destinationType == typeof(Enum[]);
            if (asString || asEnum)
            {
                Keys key = (Keys)value;
                return asString ? GetTermsString(key) : GetTermKeys(key);
            }

            return base.ConvertTo(context, culture, value, destinationType);

            Enum[] GetTermKeys(Keys key)
            {
                List<Enum> termKeys = new();
                Keys modifiers = (key & Keys.Modifiers);

                // First, iterate through and do the modifiers. These are
                // additive, so we support things like Ctrl + Alt
                for (int i = 0; i < DisplayOrder.Count; i++)
                {
                    string keyString = DisplayOrder[i];
                    Keys keyValue = _keyNames[keyString];
                    if (keyValue.HasFlag(modifiers))
                    {
                        termKeys.Add(keyValue);
                    }
                }

                // Now reset and do the key values. Here, we quit if
                // we find a match.
                Keys keyOnly = key & Keys.KeyCode;
                bool foundKey = false;

                for (int i = 0; i < DisplayOrder.Count; i++)
                {
                    string keyString = DisplayOrder[i];
                    Keys keyValue = _keyNames[keyString];
                    if (keyValue.Equals(keyOnly))
                    {
                        termKeys.Add(keyValue);
                        foundKey = true;
                        break;
                    }
                }

                // Finally, if the key wasn't in our list, add it to
                // the end anyway. Here we just pull the key value out
                // of the enum.
                if (!foundKey && Enum.IsDefined(typeof(Keys), (int)keyOnly))
                {
                    termKeys.Add(keyOnly);
                }

                return termKeys.ToArray();
            }

            string GetTermsString(Keys key)
            {
                StringBuilder termStrings = new(32);
                Keys modifiers = (key & Keys.Modifiers);

                // First, iterate through and do the modifiers. These are
                // additive, so we support things like Ctrl + Alt
                for (int i = 0; i < DisplayOrder.Count; i++)
                {
                    string keyString = DisplayOrder[i];
                    Keys keyValue = _keyNames[keyString];
                    if (keyValue.HasFlag(modifiers))
                    {
                        termStrings.Append(keyString).Append('+');
                    }
                }

                // Now reset and do the key values. Here, we quit if
                // we find a match.
                Keys keyOnly = key & Keys.KeyCode;
                bool foundKey = false;

                for (int i = 0; i < DisplayOrder.Count; i++)
                {
                    string keyString = DisplayOrder[i];
                    Keys keyValue = _keyNames[keyString];
                    if (keyValue.Equals(keyOnly))
                    {
                        termStrings.Append(keyString);
                        foundKey = true;
                        break;
                    }
                }

                // Finally, if the key wasn't in our list, add it to
                // the end anyway. Here we just pull the key value out
                // of the enum.
                if (!foundKey && Enum.IsDefined(typeof(Keys), (int)keyOnly))
                {
                    termStrings.Append(keyOnly.ToString());
                }

                return termStrings.ToString();
            }
        }

        /// <summary>
        ///  Retrieves a collection containing a set of standard values
        ///  for the data type this validator is designed for.  This
        ///  will return null if the data type does not support a
        ///  standard set of values.
        /// </summary>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
        {
            if (_values is null)
            {
                Keys[] list = KeyNames.Values.ToArray();
                Array.Sort(list, this);
                _values = new StandardValuesCollection(list);
            }

            return _values;
        }

        /// <summary>
        ///  Determines if the list of standard values returned from
        ///  GetStandardValues is an exclusive list.  If the list
        ///  is exclusive, then no other values are valid, such as
        ///  in an enum data type.  If the list is not exclusive,
        ///  then there are other valid values besides the list of
        ///  standard values GetStandardValues provides.
        /// </summary>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context)
        {
            return false;
        }

        /// <summary>
        ///  Determines if this object supports a standard set of values
        ///  that can be picked from a list.
        /// </summary>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext? context)
        {
            return true;
        }
    }
}
