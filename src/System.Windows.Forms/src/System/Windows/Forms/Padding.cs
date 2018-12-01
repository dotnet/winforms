// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.ComponentModel.Design.Serialization;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Globalization;

    /// <include file='doc\Forms.uex' path='docs/doc[@for="Forms.Padding"]/*' />
    [TypeConverterAttribute(typeof(PaddingConverter))]
    [Serializable]
    public struct Padding {
        private bool _all;
        private int _top;
        private int _left;
        private int _right;
        private int _bottom;

        /// <include file='doc\Padding.uex' path='docs/doc[@for="Padding.Empty"]/*' />
        public static readonly Padding Empty = new Padding(0);

        /// <include file='doc\Padding.uex' path='docs/doc[@for="Padding.Padding"]/*' />
        public Padding(int all) {
            _all = true;
            _top = _left = _right = _bottom = all;
            Debug_SanityCheck();
        }

        /// <include file='doc\Padding.uex' path='docs/doc[@for="Padding.Padding"]/*' />
        public Padding(int left, int top, int right, int bottom) {
            _top = top;
            _left = left;
            _right = right;
            _bottom = bottom;
            _all = _top == _left && _top == _right && _top == _bottom;
            Debug_SanityCheck();
        }

        /// <include file='doc\Padding.uex' path='docs/doc[@for="Padding.All"]/*' />
        [RefreshProperties(RefreshProperties.All)]
        public int All {
            get {
                return _all ? _top : -1;
            }
            set {
                if (_all != true || _top != value) {
                    _all = true;
                    _top = _left = _right = _bottom = value;
                }
                Debug_SanityCheck();
            }
        }

        /// <include file='doc\Padding.uex' path='docs/doc[@for="Padding.Bottom"]/*' />
        [RefreshProperties(RefreshProperties.All)]
        public int Bottom {
            get {
                if (_all) {
                    return _top;
                }
                return _bottom;
            }
            set {
                if (_all || _bottom != value) {
                    _all = false;
                    _bottom = value;
                }
                Debug_SanityCheck();
            }
        }

        /// <include file='doc\Padding.uex' path='docs/doc[@for="Padding.Left"]/*' />
        [RefreshProperties(RefreshProperties.All)]
        public int Left {
            get {
                if (_all) {
                    return _top;
                }
                return _left;
            }
            set {
                if (_all || _left != value) {
                    _all = false;
                    _left = value;
                }
                Debug_SanityCheck();
            }
        }

        /// <include file='doc\Padding.uex' path='docs/doc[@for="Padding.Right"]/*' />
        [RefreshProperties(RefreshProperties.All)]
        public int Right {
            get {
                if (_all) {
                    return _top;
                }
                return _right;
            }
            set {
                if (_all || _right != value) {
                    _all = false;
                    _right = value;
                }
                Debug_SanityCheck();
            }
        }

        /// <include file='doc\Padding.uex' path='docs/doc[@for="Padding.Top"]/*' />
        [RefreshProperties(RefreshProperties.All)]
        public int Top {
            get {
                return _top;
            }
            set {
                if (_all || _top != value) {
                    _all = false;
                    _top = value;
                }
                Debug_SanityCheck();
            }
        }

        /// <include file='doc\Padding.uex' path='docs/doc[@for="Padding.Horizontal"]/*' />
        [Browsable(false)]
        public int Horizontal {
            get {
                return Left + Right;
            }
        }

        /// <include file='doc\Padding.uex' path='docs/doc[@for="Padding.Vertical"]/*' />
        [Browsable(false)]
        public int Vertical {
            get {
                return Top + Bottom;
            }
        }

        /// <include file='doc\Padding.uex' path='docs/doc[@for="Padding.Size"]/*' />
        [Browsable(false)]
        public Size Size {
            get {
                return new Size(Horizontal, Vertical);
            }
        }

        public static Padding Add(Padding p1, Padding p2) {
            // added for FXCop rule: Provide a friendly-name version of the Addition operator
            return p1 + p2;
        }

        public static Padding Subtract(Padding p1, Padding p2) {
            // added for FXCop rule: Provide a friendly-name version of the Subtraction operator
            return p1 - p2;
        }

        /// <include file='doc\Padding.uex' path='docs/doc[@for="Padding.Equals"]/*' />
        public override bool Equals(object other) {
            if(other is Padding) {            
                return ((Padding)other) == this;                
            }
            return false;
        }

      

        /// <include file='doc\Size.uex' path='docs/doc[@for="Padding.operator+"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Performs vector addition of two <see cref='System.Windows.Forms.Padding'/> objects.
        ///    </para>
        /// </devdoc>
        public static Padding operator +(Padding p1, Padding p2) {
            return new Padding(p1.Left + p2.Left, p1.Top + p2.Top, p1.Right + p2.Right, p1.Bottom + p2.Bottom );
        
        }

        /// <include file='doc\Size.uex' path='docs/doc[@for="Padding.operator-"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Contracts a <see cref='System.Drawing.Size'/> by another <see cref='System.Drawing.Size'/>
        ///       .
        ///    </para>
        /// </devdoc>
        public static Padding operator -(Padding p1, Padding p2) {
            return new Padding(p1.Left - p2.Left, p1.Top - p2.Top, p1.Right - p2.Right, p1.Bottom - p2.Bottom );
        }

        /// <include file='doc\Size.uex' path='docs/doc[@for="Padding.operator=="]/*' />
        /// <devdoc>
        ///    Tests whether two <see cref='System.Windows.Forms.Padding'/> objects
        ///    are identical.
        /// </devdoc>
        public static bool operator ==(Padding p1, Padding p2) {
            return p1.Left == p2.Left && p1.Top == p2.Top && p1.Right == p2.Right && p1.Bottom == p2.Bottom; 
        }
        
        /// <include file='doc\Size.uex' path='docs/doc[@for="Padding.operator!="]/*' />
        /// <devdoc>
        ///    <para>
        ///       Tests whether two <see cref='System.Windows.Forms.Padding'/> objects are different.
        ///    </para>
        /// </devdoc>
        public static bool operator !=(Padding p1, Padding p2) {
            return !(p1 == p2);
        }
        
        /// <include file='doc\Padding.uex' path='docs/doc[@for="Padding.GetHashCode"]/*' />
        public override int GetHashCode() {
            // Padding class should implement GetHashCode for perf
            return Left
                ^ WindowsFormsUtils.RotateLeft(Top, 8)
                ^ WindowsFormsUtils.RotateLeft(Right, 16)
                ^ WindowsFormsUtils.RotateLeft(Bottom, 24);
        }

        /// <include file='doc\Padding.uex' path='docs/doc[@for="Padding.ToString"]/*' />
        public override string ToString() {
            return "{Left=" + Left.ToString(CultureInfo.CurrentCulture) + ",Top=" + Top.ToString(CultureInfo.CurrentCulture) + ",Right=" + Right.ToString(CultureInfo.CurrentCulture) + ",Bottom=" + Bottom.ToString(CultureInfo.CurrentCulture) + "}";
        }

        private void ResetAll() {
            All = 0;
        }

        private void ResetBottom() {
            Bottom = 0;
        }

        private void ResetLeft() {
            Left = 0;
        }

        private void ResetRight() {
            Right = 0;
        }

        private void ResetTop() {
            Top = 0;
        }

        internal void Scale(float dx, float dy) {
            _top = (int)((float)_top * dy);
            _left = (int)((float)_left * dx);
            _right = (int)((float)_right * dx);
            _bottom = (int)((float)_bottom * dy);
        }

        internal bool ShouldSerializeAll() {
            return _all;
        }

        [Conditional("DEBUG")]
        private void Debug_SanityCheck() {
            if(_all) {
                Debug.Assert(ShouldSerializeAll(), "_all is true, but ShouldSerializeAll() is false.");
                Debug.Assert(All == Left && Left == Top && Top == Right && Right == Bottom, "_all is true, but All/Left/Top/Right/Bottom inconsistent.");
            } else {
                Debug.Assert(All == -1, "_all is false, but All != -1.");
                Debug.Assert(!ShouldSerializeAll(), "ShouldSerializeAll() should not be true when all flag is not set.");

                // The below assert is not true with the current implementation of DockPaddingEdges.
                // Debug.Assert(Left != Top || Top != Right || Right != Bottom, "_all is not set, but Left/Top/Right/Bottom are all the same");
            }
        }
    }

    public class PaddingConverter : TypeConverter {

        /// <devdoc>
        ///      Determines if this converter can convert an object in the given source
        ///      type to the native type of the converter.
        /// </devdoc>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor)) {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        /// <devdoc>
        ///      Converts the given object to the converter's native type.
        /// </devdoc>
        [
            SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes") // ConvertFromString returns an object
        ]
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string valueStr = value as string;
            if (valueStr != null) {
                valueStr = valueStr.Trim();

                if (valueStr.Length == 0) {
                    return null;
                }
                else {
                    // Parse 4 integer values.
                    if (culture == null) {
                        culture = CultureInfo.CurrentCulture;
                    }
                    char sep = culture.TextInfo.ListSeparator[0];
                    string[] tokens = valueStr.Split(new char[] { sep });
                    int[] values = new int[tokens.Length];
                    TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));
                    for (int i = 0; i < values.Length; i++) {
                        // Note: ConvertFromString will raise exception if value cannot be converted.
                        values[i] = (int)intConverter.ConvertFromString(context, culture, tokens[i]);
                    }
                    if (values.Length == 4) {
                        return new Padding(values[0], values[1], values[2], values[3]);
                    }
                    else {
                        throw new ArgumentException(string.Format(SR.TextParseFailedFormat,
                                                                  "value",
                                                                  valueStr,
                                                                  "left, top, right, bottom"));
                    }
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
        
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == null) {
                throw new ArgumentNullException(nameof(destinationType));
            }
        
            if (value is Padding) {
                if (destinationType == typeof(string)) {
                    Padding padding = (Padding)value;
                    
                    if (culture == null) {
                        culture = CultureInfo.CurrentCulture;
                    }                                        
                    string sep = culture.TextInfo.ListSeparator + " ";
                    TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));
                    string[] args = new string[4];
                    int nArg = 0;
                    
                    // Note: ConvertToString will raise exception if value cannot be converted.
                    args[nArg++] = intConverter.ConvertToString(context, culture, padding.Left);
                    args[nArg++] = intConverter.ConvertToString(context, culture, padding.Top);
                    args[nArg++] = intConverter.ConvertToString(context, culture, padding.Right);
                    args[nArg++] = intConverter.ConvertToString(context, culture, padding.Bottom);
                    
                    return string.Join(sep, args);
                }
                else if (destinationType == typeof(InstanceDescriptor)) {
                    Padding padding = (Padding) value;

                    if(padding.ShouldSerializeAll()) {
                        return new InstanceDescriptor(
                            typeof(Padding).GetConstructor(new Type[] {typeof(int)}), 
                            new object[] {padding.All});
                    }
                    else {
                        return new InstanceDescriptor(
                            typeof(Padding).GetConstructor(new Type[] {typeof(int), typeof(int), typeof(int), typeof(int)}), 
                            new object[] {padding.Left, padding.Top, padding.Right, padding.Bottom});
                    }
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }
            if (propertyValues == null) {
                throw new ArgumentNullException(nameof(propertyValues));
            }

            Padding original = (Padding) context.PropertyDescriptor.GetValue(context.Instance);

            int all = (int)propertyValues["All"];
            if(original.All != all) {
                return new Padding(all);
            }
            else {
                return new Padding(
                    (int)propertyValues["Left"],
                    (int)propertyValues["Top"],
                    (int)propertyValues["Right"],
                    (int)propertyValues["Bottom"]);
            }
        }
        
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(Padding), attributes);
            return props.Sort(new string[] {"All", "Left", "Top", "Right", "Bottom"});
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
            return true;
        }
    }
}
