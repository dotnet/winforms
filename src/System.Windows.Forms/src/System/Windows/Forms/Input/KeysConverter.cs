// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace System.Windows.Forms;

/// <summary>
///  Provides a type converter to convert <see cref="Keys"/> objects to and from various
///  other representations.
/// </summary>
public class KeysConverter : TypeConverter, IComparer
{
    private Dictionary<CultureInfo, List<string>>? _cultureToDisplayOrder;
    private Dictionary<CultureInfo, Dictionary<string, Keys>>? _cultureToKeyName;
    private StandardValuesCollection? _values;

    [MemberNotNull(nameof(_cultureToDisplayOrder))]
    [MemberNotNull(nameof(_cultureToKeyName))]
    private void Initialize()
    {
        _cultureToDisplayOrder = [];
        _cultureToKeyName = [];
        AddLocalizedKeyNames(CultureInfo.InvariantCulture);
    }

    private void AddLocalizedKeyNames(CultureInfo cultureInfo)
    {
        if (CultureToDisplayOrder.ContainsKey(cultureInfo) && CultureToKeyName.ContainsKey(cultureInfo))
        {
            return;
        }

        List<string> localizedOrder = new(34);
        Dictionary<string, Keys> localizedNames = new(34);

        AddLocalizedKey(nameof(SR.toStringEnter), Keys.Return);
        AddKey("F12", Keys.F12);
        AddKey("F11", Keys.F11);
        AddKey("F10", Keys.F10);
        AddLocalizedKey(nameof(SR.toStringEnd), Keys.End);
        AddLocalizedKey(nameof(SR.toStringControl), Keys.Control);
        AddKey("F8", Keys.F8);
        AddKey("F9", Keys.F9);
        AddLocalizedKey(nameof(SR.toStringAlt), Keys.Alt);
        AddKey("F4", Keys.F4);
        AddKey("F5", Keys.F5);
        AddKey("F6", Keys.F6);
        AddKey("F7", Keys.F7);
        AddKey("F1", Keys.F1);
        AddKey("F2", Keys.F2);
        AddKey("F3", Keys.F3);
        AddLocalizedKey(nameof(SR.toStringPageDown), Keys.Next);
        AddLocalizedKey(nameof(SR.toStringInsert), Keys.Insert);
        AddLocalizedKey(nameof(SR.toStringHome), Keys.Home);
        AddLocalizedKey(nameof(SR.toStringDelete), Keys.Delete);
        AddLocalizedKey(nameof(SR.toStringShift), Keys.Shift);
        AddLocalizedKey(nameof(SR.toStringPageUp), Keys.Prior);
        AddLocalizedKey(nameof(SR.toStringBack), Keys.Back);
        AddLocalizedKey(nameof(SR.toStringNone), Keys.None);

        // new whidbey keys follow here...
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

        CultureToKeyName.Add(cultureInfo, localizedNames);
        CultureToDisplayOrder.Add(cultureInfo, localizedOrder);

        void AddKey(string key, Keys value)
        {
            localizedNames[key] = value;
            localizedOrder.Add(key);
        }

        void AddLocalizedKey(string keyName, Keys value)
        {
            string key = SR.ResourceManager.GetString(keyName, cultureInfo) ??
                throw new InvalidOperationException(string.Format(SR.ResourceValueNotFound, keyName));
            AddKey(key, value);
        }
    }

    /// <summary>
    ///  Access to a lookup table of name/value pairs for keys. These are localized
    ///  names.
    /// </summary>
    private Dictionary<CultureInfo, Dictionary<string, Keys>> CultureToKeyName
    {
        get
        {
            if (_cultureToKeyName is null)
            {
                Initialize();
            }

            return _cultureToKeyName;
        }
    }

    private Dictionary<CultureInfo, List<string>> CultureToDisplayOrder
    {
        get
        {
            if (_cultureToDisplayOrder is null)
            {
                Initialize();
            }

            return _cultureToDisplayOrder;
        }
    }

    /// <summary>
    ///  Determines if this converter can convert an object in the given source
    ///  type to the native type of the converter.
    /// </summary>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(string) || sourceType == typeof(Enum[]) || base.CanConvertFrom(context, sourceType);

    /// <summary>
    ///  Gets a value indicating whether this converter can
    ///  convert an object to the given destination type using the context.
    /// </summary>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        => destinationType == typeof(Enum[]) || base.CanConvertTo(context, destinationType);

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
        if (value is string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            Dictionary<string, Keys> keyNames = GetKeyNames(culture);

            // Parse an array of key tokens.
            string[] tokens = text.Split('+', StringSplitOptions.TrimEntries);

            // Now lookup each key token in our key hashtable.
            Keys key = 0;
            bool foundKeyCode = false;

            foreach (string token in tokens)
            {
                if (!keyNames.TryGetValue(token, out Keys currentKey))
                {
                    // Key was not found in our dictionary. See if it is a valid value in
                    // the Keys enum.
                    currentKey = (Keys)Enum.Parse(typeof(Keys), token);
                }

                if ((currentKey & Keys.KeyCode) != 0)
                {
                    // We found a match. If we have previously found a
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

            return key;
        }

        if (value is Enum[] valueAsEnumArray)
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
    ///  Converts the given object to another type. The most common types to convert
    ///  are to and from a string object. The default implementation will make a call
    ///  to ToString on the object if the object is valid and if the destination
    ///  type is string. If this cannot convert to the destination type, this will
    ///  throw a NotSupportedException.
    /// </summary>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (value is not Keys and not int)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }

        return destinationType == typeof(string)
            ? GetTermsString((Keys)value)
            : destinationType == typeof(Enum[])
                ? GetTermKeys((Keys)value)
                : base.ConvertTo(context, culture, value, destinationType);

        Enum[] GetTermKeys(Keys key)
        {
            List<Enum> termKeys = [];
            Keys modifiers = key & Keys.Modifiers;
            Dictionary<string, Keys> keyNames = GetKeyNames(culture);
            List<string> displayOrder = GetDisplayOrder(culture);

            if (key != Keys.None)
            {
                // First, iterate through and do the modifiers. These are
                // additive, so we support things like Ctrl + Alt
                foreach (string keyString in displayOrder)
                {
                    Keys keyValue = keyNames[keyString];
                    if (keyValue != Keys.None && modifiers.HasFlag(keyValue))
                    {
                        termKeys.Add(keyValue);
                    }
                }
            }

            // Now reset and do the key values. Here, we quit if
            // we find a match.
            Keys keyOnly = key & Keys.KeyCode;
            bool foundKey = false;

            foreach (string keyString in displayOrder)
            {
                Keys keyValue = keyNames[keyString];
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
            if (!foundKey && Enum.IsDefined(keyOnly))
            {
                termKeys.Add(keyOnly);
            }

            return [.. termKeys];
        }

        string GetTermsString(Keys key)
        {
            StringBuilder termStrings = new(32);
            Keys modifiers = key & Keys.Modifiers;
            Dictionary<string, Keys> keyNames = GetKeyNames(culture);
            List<string> displayOrder = GetDisplayOrder(culture);

            if (key != Keys.None)
            {
                // First, iterate through and do the modifiers. These are
                // additive, so we support things like Ctrl + Alt
                foreach (string keyString in displayOrder)
                {
                    Keys keyValue = keyNames[keyString];
                    if (keyValue != Keys.None && modifiers.HasFlag(keyValue))
                    {
                        termStrings.Append(keyString).Append('+');
                    }
                }
            }

            // Now reset and do the key values. Here, we quit if
            // we find a match.
            Keys keyOnly = key & Keys.KeyCode;
            bool foundKey = false;

            foreach (string keyString in displayOrder)
            {
                Keys keyValue = keyNames[keyString];
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
            if (!foundKey && Enum.IsDefined(keyOnly))
            {
                termStrings.Append(Enum.GetName(keyOnly));
            }

            return termStrings.ToString();
        }
    }

    private List<string> GetDisplayOrder(CultureInfo? culture)
    {
        // Use CurrentUICulture as default because we assume that this converter is primarily
        // used in user-facing applications (I.e. what key to press on the keyboard).
        culture ??= CultureInfo.CurrentUICulture;
        if (!CultureToDisplayOrder.ContainsKey(culture))
        {
            AddLocalizedKeyNames(culture);
        }

        return CultureToDisplayOrder[culture];
    }

    private Dictionary<string, Keys> GetKeyNames(CultureInfo? culture)
    {
        // Use CurrentUICulture as default because we assume that this converter is primarily
        // used in user-facing applications (I.e. what key to press on the keyboard).
        culture ??= CultureInfo.CurrentUICulture;
        if (!CultureToKeyName.ContainsKey(culture))
        {
            AddLocalizedKeyNames(culture);
        }

        return CultureToKeyName[culture];
    }

    /// <summary>
    ///  Retrieves a collection containing a set of standard values
    ///  for the data type this validator is designed for. This
    ///  will return null if the data type does not support a
    ///  standard set of values.
    /// </summary>
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
    {
        if (_values is null)
        {
            Keys[] values = [.. CultureToKeyName[CultureInfo.InvariantCulture].Values];
            Array.Sort(values, this);
            _values = new StandardValuesCollection(values);
        }

        return _values;
    }

    /// <summary>
    ///  Determines if the list of standard values returned from
    ///  GetStandardValues is an exclusive list. If the list
    ///  is exclusive, then no other values are valid, such as
    ///  in an enum data type. If the list is not exclusive,
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
