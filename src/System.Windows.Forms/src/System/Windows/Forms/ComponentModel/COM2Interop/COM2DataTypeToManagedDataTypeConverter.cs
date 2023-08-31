// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

/// <summary>
///  This base class maps an ole defined data type (OLE_COLOR, IFont, etc.),
/// </summary>
internal abstract class Com2DataTypeToManagedDataTypeConverter
{
    public virtual bool AllowExpand => false;

    /// <summary>
    ///  Returns the managed type that this editor maps the property type to.
    /// </summary>
    public abstract Type ManagedType { get; }

    /// <summary>
    ///  Converts the native value into a managed value.
    /// </summary>
    public abstract object? ConvertNativeToManaged(VARIANT nativeValue, Com2PropertyDescriptor property);

    /// <summary>
    ///  Converts the managed value into a native value.
    /// </summary>
    /// <param name="cancelSet">
    ///  If <see langword="true"/> do not set the returned value. This allows scenarios like setting back the
    ///  properties of an <see cref="IFont"/> without actually changing the <see cref="IFont"/> instance.
    /// </param>
    public abstract VARIANT ConvertManagedToNative(object? managedValue, Com2PropertyDescriptor property, ref bool cancelSet);
}
