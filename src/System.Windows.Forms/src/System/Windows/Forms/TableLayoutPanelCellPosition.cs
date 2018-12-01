// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.ComponentModel.Design.Serialization;
    using System.Globalization;

    [TypeConverterAttribute(typeof(TableLayoutPanelCellPositionTypeConverter))]
    public struct TableLayoutPanelCellPosition {

        private int row;
        private int column;
        
        public TableLayoutPanelCellPosition(int column, int row) {
            if (row < -1) {
                throw new ArgumentOutOfRangeException(nameof(row), string.Format(SR.InvalidArgument, "row", (row).ToString(CultureInfo.CurrentCulture)));
            } 
            if (column < -1) {
                throw new ArgumentOutOfRangeException(nameof(column), string.Format(SR.InvalidArgument, "column", (column).ToString(CultureInfo.CurrentCulture)));
            }
            this.row = row;
            this.column = column;
        }

        public int Row {
            get {
                return row;
            }
            set {
                row = value;
            }
        }

        public int Column {
            get {
                return column;
            }
            set {
                column = value;
            }
        }
        public override bool Equals(object other) {
            if(other is TableLayoutPanelCellPosition) {
                TableLayoutPanelCellPosition dpeOther = (TableLayoutPanelCellPosition) other;

                return (dpeOther.row == row && 
                       dpeOther.column == column);
            }
            return false;
        }
        public static bool operator ==(TableLayoutPanelCellPosition p1, TableLayoutPanelCellPosition p2) {
            return p1.Row == p2.Row && p1.Column == p2.Column;
        }
        public static bool operator !=(TableLayoutPanelCellPosition p1, TableLayoutPanelCellPosition p2) {
            return !(p1 == p2);
        }
        
        
        public override string ToString() {
            return  Column.ToString(CultureInfo.CurrentCulture) + "," + Row.ToString(CultureInfo.CurrentCulture);
        }
        
        public override int GetHashCode() {
            // Structs should implement GetHashCode for perf
           return WindowsFormsUtils.GetCombinedHashCodes(
                                                        this.row,
                                                        this.column);
        }
    }
    
    internal class TableLayoutPanelCellPositionTypeConverter : TypeConverter {

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            if (destinationType == typeof(InstanceDescriptor)) {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            if (sourceType == typeof(string)) {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
        
            if (value is string) {
            
                string text = ((string)value).Trim();
            
                if (text.Length == 0) {
                    return null;
                }
                else {
                
                    // Parse 2 integer values.
                    //
                    if (culture == null) {
                        culture = CultureInfo.CurrentCulture;
                    }                    
                    char sep = culture.TextInfo.ListSeparator[0];
                    string[] tokens = text.Split(new char[] {sep});
                    int[] values = new int[tokens.Length];
                    TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));
                    for (int i = 0; i < values.Length; i++) {
                        // Note: ConvertFromString will raise exception if value cannot be converted.
                        values[i] = (int)intConverter.ConvertFromString(context, culture, tokens[i]);
                    }
                    
                    if (values.Length == 2) {
                        return new TableLayoutPanelCellPosition(values[0], values[1]);
                    }
                    else {
                        throw new ArgumentException(string.Format(SR.TextParseFailedFormat,
                                                                  text,
                                                                  "column, row"));
                    }
                }
            }
            
            return base.ConvertFrom(context, culture, value);
        }
        
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == null) {
                throw new ArgumentNullException(nameof(destinationType));
            }
        
            if (destinationType == typeof(InstanceDescriptor) && value is TableLayoutPanelCellPosition) {
                TableLayoutPanelCellPosition cellPosition = (TableLayoutPanelCellPosition) value;

             
                return new InstanceDescriptor(
                    typeof(TableLayoutPanelCellPosition).GetConstructor(new Type[] {typeof(int), typeof(int)}), 
                    new object[] {cellPosition.Column, cellPosition.Row});
            
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {

            return new TableLayoutPanelCellPosition(
                (int)propertyValues["Column"],
                (int)propertyValues["Row"]
             );
        
        }
        
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(TableLayoutPanelCellPosition), attributes);
            return props.Sort(new string[] {"Column","Row"});
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
            return true;
        }
    }

    
}
