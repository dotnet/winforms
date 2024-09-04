// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

/// <summary>
///  This class mimics a clr enum that we can create at runtime. It associates an array of names with an array of
///  values and converts between them.
/// </summary>
/// <remarks>
///  <para>
///   A note here: we compare string values when looking for the value of an item. Typically these aren't large
///   lists and the perf is worth it. The reason stems from IPerPropertyBrowsing, which supplies a list of names
///   and a list of variants to mimic enum functionality. If the actual property value is a DWORD, which translates
///   to VT_UI4, and they specify their values as VT_I4 (which is a common mistake), they won't compare properly and
///   values can't be updated. By comparing strings, we avoid this problem and add flexibility to the system.
///  </para>
/// </remarks>
internal class Com2Enum
{
    /// <summary>
    ///  Our array of value string names.
    /// </summary>
    private string[] _names = [];

    /// <summary>
    ///  Our values.
    /// </summary>
    private object[] _values = [];

    /// <summary>
    ///  Our cached array of value.ToString()'s.
    /// </summary>
    private string?[] _stringValues = [];

    /// <summary>
    ///  Retrieve a copy of the value array.
    /// </summary>
    public virtual object[] Values => (object[])_values.Clone();

    /// <summary>
    ///  Retrieve a copy of the nme array.
    /// </summary>
    public virtual string[] Names => (string[])_names.Clone();

    /// <summary>
    ///  Associate a string to the appropriate value.
    /// </summary>
    public virtual object FromString(string value)
    {
        int bestMatch = -1;

        for (int i = 0; i < _stringValues.Length; i++)
        {
            if (string.Compare(_names[i], value, true, CultureInfo.InvariantCulture) == 0 ||
                string.Compare(_stringValues[i], value, true, CultureInfo.InvariantCulture) == 0)
            {
                return _values[i];
            }

            if (bestMatch == -1 && string.Compare(_names[i], value, true, CultureInfo.InvariantCulture) == 0)
            {
                bestMatch = i;
            }
        }

        return bestMatch != -1 ? _values[bestMatch] : value;
    }

    protected void PopulateArrays(string[] names, object[] values)
    {
        _names = names;
        _values = values;
        _stringValues = new string[names.Length];

        for (int i = 0; i < names.Length; i++)
        {
            _stringValues[i] = values[i]?.ToString();
        }
    }

    /// <summary>
    ///  Retrieves the string name of a given value.
    /// </summary>
    public virtual string ToString(object? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        // In case this is a real enum try to convert it.
        if (_values.Length > 0 && value.GetType() != _values[0].GetType())
        {
            try
            {
                value = Convert.ChangeType(value, _values[0].GetType(), CultureInfo.InvariantCulture);
            }
            catch
            {
            }
        }

        // We have to do this to compensate for small discrepancies in a lot of objects in COM2
        // (DWORD -> VT_IU4, value we get is VT_I4, which convert to Int32, UInt32 respectively)
        if (value?.ToString() is not string stringValue)
        {
            return string.Empty;
        }

        for (int i = 0; i < _values.Length; i++)
        {
            if (string.Compare(_stringValues[i], stringValue, true, CultureInfo.InvariantCulture) == 0)
            {
                return _names[i];
            }
        }

        return stringValue;
    }
}
