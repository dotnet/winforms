// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Text;
    using Microsoft.Win32;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Diagnostics.CodeAnalysis;

    /// <include file='doc\KeysConverter.uex' path='docs/doc[@for="KeysConverter"]/*' />
    /// <devdoc>
    /// <para>Provides a type converter to convert <see cref='System.Windows.Forms.Keys'/> objects to and from various 
    ///    other representations.</para>
    /// </devdoc>
    public class KeysConverter : TypeConverter, IComparer {
        private IDictionary keyNames;
        private List<string> displayOrder;
        private StandardValuesCollection values;
    
        private const Keys FirstDigit = System.Windows.Forms.Keys.D0;
        private const Keys LastDigit = System.Windows.Forms.Keys.D9;
        private const Keys FirstAscii = System.Windows.Forms.Keys.A;
        private const Keys LastAscii = System.Windows.Forms.Keys.Z;
        private const Keys FirstNumpadDigit = System.Windows.Forms.Keys.NumPad0;
        private const Keys LastNumpadDigit = System.Windows.Forms.Keys.NumPad9;

        private void AddKey(string key, Keys value ) {
            keyNames[key] = value;
            displayOrder.Add(key);
        }

        private void Initialize() {
            keyNames = new Hashtable(34);
            displayOrder = new List<string>(34);

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
        }

        /// <include file='doc\KeysConverter.uex' path='docs/doc[@for="KeysConverter.KeyNames"]/*' />
        /// <devdoc>
        ///  Access to a lookup table of name/value pairs for keys.  These are localized
        ///  names.
        /// </devdoc>
        private IDictionary KeyNames {
            get {
                if (keyNames == null) {
                    Debug.Assert(displayOrder == null);
                    Initialize();
                }
                return keyNames;
            }
        }

        private List<string> DisplayOrder {
            get {
                if (displayOrder == null) {
                    Debug.Assert(keyNames == null);
                    Initialize();
                }
                return displayOrder;
            }
        }
    
        /// <include file='doc\KeysConverter.uex' path='docs/doc[@for="KeysConverter.CanConvertFrom"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Determines if this converter can convert an object in the given source
        ///    type to the native type of the converter.
        /// </devdoc>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            if (sourceType == typeof(string) || sourceType == typeof(Enum[])) {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
        
        /// <include file='doc\EnumConverter.uex' path='docs/doc[@for="EnumConverter.CanConvertTo"]/*' />
        /// <devdoc>
        ///    <para>Gets a value indicating whether this converter can
        ///       convert an object to the given destination type using the context.</para>
        /// </devdoc>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            if (destinationType == typeof(Enum[])) {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        /// <include file='doc\KeysConverter.uex' path='docs/doc[@for="KeysConverter.Compare"]/*' />
        /// <devdoc>
        ///    <para>Compares two key values for equivalence.</para>
        /// </devdoc>
        public int Compare(object a, object b) {
            return String.Compare(ConvertToString(a), ConvertToString(b), false, CultureInfo.InvariantCulture);
        }

        /// <include file='doc\KeysConverter.uex' path='docs/doc[@for="KeysConverter.ConvertFrom"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Converts the given object to the converter's native type.
        /// </devdoc>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
        
            if (value is string) {
            
                string text = ((string)value).Trim();
            
                if (text.Length == 0) {
                    return null;
                }
                else {
                
                    // Parse an array of key tokens.
                    //
                    string[] tokens = text.Split(new char[] {'+'});
                    for (int i = 0; i < tokens.Length; i++) {
                        tokens[i] = tokens[i].Trim();
                    }
                    
                    // Now lookup each key token in our key hashtable.
                    //
                    Keys key = (Keys)0;
                    bool foundKeyCode = false;
                    
                    for (int i = 0; i < tokens.Length; i++) {
                        object obj = KeyNames[tokens[i]];
                        
                        if (obj == null) {
                        
                            // Key was not found in our table.  See if it is a valid value in
                            // the Keys enum.
                            //
                            obj = Enum.Parse(typeof(Keys), tokens[i]);
                        }
                        
                        if (obj != null) {
                            Keys currentKey = (Keys)obj;
                            
                            if ((currentKey & Keys.KeyCode) != 0) {
                            
                                // We found a match.  If we have previously found a
                                // key code, then check to see that this guy
                                // isn't a key code (it is illegal to have, say,
                                // "A + B"
                                //
                                if (foundKeyCode) {
                                    throw new FormatException(SR.KeysConverterInvalidKeyCombination);
                                }
                                foundKeyCode = true;
                            }
                            
                            // Now OR the key into our current key
                            //
                            key |= currentKey;
                        }
                        else {
                        
                            // We did not match this key.  Report this as an error too.
                            //
                            throw new FormatException(string.Format(SR.KeysConverterInvalidKeyName, tokens[i]));
                            
                        }
                    }
                    
                    return (object)key;
                }
            }
            else if (value is Enum[]) {
                long finalValue = 0;
                foreach(Enum e in (Enum[])value) {
                    finalValue |= Convert.ToInt64(e, CultureInfo.InvariantCulture);
                }
                return Enum.ToObject(typeof(Keys), finalValue);
            }
            
            return base.ConvertFrom(context, culture, value);
        }

        /// <include file='doc\KeysConverter.uex' path='docs/doc[@for="KeysConverter.ConvertTo"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Converts the given object to another type.  The most common types to convert
        ///    are to and from a string object.  The default implementation will make a call
        ///    to ToString on the object if the object is valid and if the destination
        ///    type is string.  If this cannot convert to the desitnation type, this will
        ///    throw a NotSupportedException.
        /// </devdoc>
        [SuppressMessage("Microsoft.Performance", "CA1803:AvoidCostlyCallsWherePossible")]            
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == null) {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (value is Keys || value is int) {
                bool asString = destinationType == typeof(string);
                bool asEnum = false;
                if (!asString) asEnum = destinationType == typeof(Enum[]);
                if (asString || asEnum) {
                    Keys key = (Keys)value;
                    bool added = false;
                    ArrayList terms = new ArrayList();
                    Keys modifiers = (key & Keys.Modifiers);

                    // First, iterate through and do the modifiers. These are
                    // additive, so we support things like Ctrl + Alt
                    //
                    for (int i = 0; i < DisplayOrder.Count; i++) {
                        string keyString = (string)DisplayOrder[i];
                        Keys keyValue = (Keys)keyNames[keyString];
                        if (((int)(keyValue) & (int)modifiers) != 0) {

                            if (asString) {
                                if (added) {
                                    terms.Add("+");
                                }

                                terms.Add((string)keyString);
                            }
                            else {
                                terms.Add(keyValue);
                            }
                            
                            added = true;
                        }
                    }
        
                    // Now reset and do the key values.  Here, we quit if
                    // we find a match.
                    //
                    Keys keyOnly = (key & Keys.KeyCode);
                    bool foundKey = false;
                    
                    if (added && asString) {
                        terms.Add("+");
                    }

                    for (int i = 0; i < DisplayOrder.Count; i++) {
                        string keyString = (string)DisplayOrder[i];
                        Keys keyValue = (Keys)keyNames[keyString];
                        if (keyValue.Equals(keyOnly)) {

                            if (asString) {
                                terms.Add((string)keyString);
                            }
                            else {
                                terms.Add(keyValue);
                            }
                            added = true;
                            foundKey = true;
                            break;
                        }
                    }
                    
                    // Finally, if the key wasn't in our list, add it to 
                    // the end anyway.  Here we just pull the key value out
                    // of the enum.
                    //
                    if (!foundKey && Enum.IsDefined(typeof(Keys), (int)keyOnly)) {
                        if (asString) {
                            terms.Add(((Enum)keyOnly).ToString());
                        }
                        else {
                            terms.Add((Enum)keyOnly);
                        }
                    }

                    if (asString) {
                        StringBuilder b = new StringBuilder(32);
                        foreach(string t in terms) {
                            b.Append(t);
                        }
                        return b.ToString();
                    }
                    else {
                        return (Enum[])terms.ToArray(typeof(Enum));
                    }
                }
            }
            
            return base.ConvertTo(context, culture, value, destinationType);
        }
        
        /// <include file='doc\KeysConverter.uex' path='docs/doc[@for="KeysConverter.GetStandardValues"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Retrieves a collection containing a set of standard values
        ///    for the data type this validator is designed for.  This
        ///    will return null if the data type does not support a
        ///    standard set of values.
        /// </devdoc>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            if (values == null) {
                ArrayList list = new ArrayList();
                
                ICollection keys = KeyNames.Values;
                
                foreach (object o in keys) {
                    list.Add(o);
                }
                
                list.Sort(this);
                
                values = new StandardValuesCollection(list.ToArray());
            }
            return values;
        }
    
        /// <include file='doc\KeysConverter.uex' path='docs/doc[@for="KeysConverter.GetStandardValuesExclusive"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Determines if the list of standard values returned from
        ///    GetStandardValues is an exclusive list.  If the list
        ///    is exclusive, then no other values are valid, such as
        ///    in an enum data type.  If the list is not exclusive,
        ///    then there are other valid values besides the list of
        ///    standard values GetStandardValues provides.
        /// </devdoc>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
            return false;
        }
        
        /// <include file='doc\KeysConverter.uex' path='docs/doc[@for="KeysConverter.GetStandardValuesSupported"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Determines if this object supports a standard set of values
        ///    that can be picked from a list.
        /// </devdoc>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            return true;
        }
    }
}

