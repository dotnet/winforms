// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop {
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Collections;
    using Microsoft.Win32;
    using System.Globalization;

    /// <include file='doc\COM2Enum.uex' path='docs/doc[@for="Com2Enum"]/*' />
    /// <devdoc>
    /// This class mimics a clr enum that we can create at runtime.
    /// It associates an array of names with an array of values and converts
    /// between them.
    ///
    /// A note here: we compare string values when looking for the value of an item.
    /// Typically these aren't large lists and the perf is worth it.  The reason stems
    /// from IPerPropertyBrowsing, which supplies a list of names and a list of
    /// variants to mimic enum functionality.  If the actual property value is a DWORD,
    /// which translates to VT_UI4, and they specify their values as VT_I4 (which is a common
    /// mistake), they won't compare properly and values can't be updated.
    /// By comparing strings, we avoid this problem and add flexiblity to the system.
    /// </devdoc>
    internal class Com2Enum {

        /// <include file='doc\COM2Enum.uex' path='docs/doc[@for="Com2Enum.names"]/*' />
        /// <devdoc>
        /// Our array of value string names
        /// </devdoc>
        private string[] names;


        /// <include file='doc\COM2Enum.uex' path='docs/doc[@for="Com2Enum.values"]/*' />
        /// <devdoc>
        /// Our values
        /// </devdoc>
        private object[] values;



        /// <include file='doc\COM2Enum.uex' path='docs/doc[@for="Com2Enum.stringValues"]/*' />
        /// <devdoc>
        /// Our cached array of value.ToString()'s
        /// </devdoc>
        private string[] stringValues;

        /// <include file='doc\COM2Enum.uex' path='docs/doc[@for="Com2Enum.allowUnknownValues"]/*' />
        /// <devdoc>
        /// Should we allow values besides what's in the listbox?
        /// </devdoc>
        private bool    allowUnknownValues;

        /// <include file='doc\COM2Enum.uex' path='docs/doc[@for="Com2Enum.Com2Enum1"]/*' />
        /// <devdoc>
        /// Our one and only ctor
        /// </devdoc>
        public Com2Enum(string[] names, object[] values, bool allowUnknownValues) {

            this.allowUnknownValues = allowUnknownValues;

            // these have to be null and the same length
            if (names == null ||
                values == null ||
                names.Length != values.Length) {
                throw new ArgumentException(SR.COM2NamesAndValuesNotEqual);
            }

            PopulateArrays(names, values);
        }

        /// <include file='doc\COM2Enum.uex' path='docs/doc[@for="Com2Enum.IsStrictEnum"]/*' />
        /// <devdoc>
        /// Can this enum be values other than the strict enum?
        /// </devdoc>
        public bool IsStrictEnum {
            get {
                return !this.allowUnknownValues;
            }
        }

        /// <include file='doc\COM2Enum.uex' path='docs/doc[@for="Com2Enum.Values"]/*' />
        /// <devdoc>
        /// Retrieve a copy of the value array
        /// </devdoc>
        public virtual object[] Values {
            get {
                return(object[])this.values.Clone();
            }
        }

        /// <include file='doc\COM2Enum.uex' path='docs/doc[@for="Com2Enum.Names"]/*' />
        /// <devdoc>
        /// Retrieve a copy of the nme array.
        /// </devdoc>
        public virtual string[] Names {
            get {
                return(string[])this.names.Clone();
            }
        }

        /// <include file='doc\COM2Enum.uex' path='docs/doc[@for="Com2Enum.FromString"]/*' />
        /// <devdoc>
        /// Associate a string to the appropriate value.
        /// </devdoc>
        public virtual object FromString(string s) {
            int bestMatch = -1;
        
            for (int i = 0; i < stringValues.Length; i++) {
                if (String.Compare(names[i], s, true, CultureInfo.InvariantCulture) == 0 ||
                    String.Compare(stringValues[i], s, true, CultureInfo.InvariantCulture) == 0) {
                    return values[i];
                }
                
                if (bestMatch == -1 && 0 == String.Compare(names[i], s, true, CultureInfo.InvariantCulture)) {
                    bestMatch = i;
                }
            }
            
            if (bestMatch != -1) {
                return values[bestMatch];    
            }
            
            return allowUnknownValues ? s : null;
        }

        protected virtual void PopulateArrays(string[] names, object[] values) {
            // setup our values...since we have to walk through
            // them anyway to do the ToString, we just copy them here.
            this.names = new string[names.Length];
            this.stringValues = new string[names.Length];
            this.values = new object[names.Length];
            for (int i = 0; i < names.Length; i++) {
                //Debug.WriteLine(names[i] + ": item " + i.ToString() + ",type=" + values[i].GetType().Name + ", value=" + values[i].ToString());
                this.names[i] = names[i];
                this.values[i] = values[i];
                if (values[i] != null) {
                    this.stringValues[i] = values[i].ToString();
                }
            }
        }

        /// <include file='doc\COM2Enum.uex' path='docs/doc[@for="Com2Enum.ToString"]/*' />
        /// <devdoc>
        /// Retrieves the string name of a given value.
        /// </devdoc>
        public virtual string ToString(object v) {
            if (v != null) {

                // in case this is a real enum...try to convert it.
                //
                if (values.Length > 0 && v.GetType() != values[0].GetType()) {
                    try {
                        v = Convert.ChangeType(v, values[0].GetType(), CultureInfo.InvariantCulture);
                    }
                    catch{
                    }
                }

                // we have to do this do compensate for small discrpencies
                // in a lot of objects in COM2 (DWORD -> VT_IU4, value we get is VT_I4, which
                // convert to Int32, UInt32 respectively
                string strVal = v.ToString();
                for (int i = 0; i < values.Length; i++) {
                    if (String.Compare(stringValues[i], strVal, true, CultureInfo.InvariantCulture) == 0) {
                        return names[i];
                    }
                }
                if (allowUnknownValues) {
                    return strVal;
                }
            }
            return "";
        }
    }
}
