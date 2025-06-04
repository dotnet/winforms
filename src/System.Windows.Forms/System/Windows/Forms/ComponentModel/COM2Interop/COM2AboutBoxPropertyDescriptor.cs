// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

[RequiresUnreferencedCode(ComNativeDescriptor.ComTypeDescriptorsMessage + " Uses Com2PropertyDescriptor which is not trim-compatible.")]
internal sealed partial class Com2AboutBoxPropertyDescriptor : Com2PropertyDescriptor
{
    private TypeConverter? _converter;
    private UITypeEditor? _editor;

    public Com2AboutBoxPropertyDescriptor()
        : base(
              PInvokeCore.DISPID_ABOUTBOX,
              "About",
              [
                  new DispIdAttribute(PInvokeCore.DISPID_ABOUTBOX),
                  DesignerSerializationVisibilityAttribute.Hidden,
                  new DescriptionAttribute(SR.AboutBoxDesc),
                  new ParenthesizePropertyNameAttribute(true)
              ],
              readOnly: true,
              typeof(string),
              typeData: null,
              hrHidden: false)
    {
    }

    public override TypeConverter Converter
    {
        [RequiresUnreferencedCode(TrimmingConstants.PropertyDescriptorPropertyTypeMessage)]
        get => _converter ??= new TypeConverter();
    }

    public override bool IsReadOnly => true;

    public override Type PropertyType => typeof(string);

    public override bool CanResetValue(object component) => false;

    [RequiresUnreferencedCode($"{TrimmingConstants.EditorRequiresUnreferencedCode} {TrimmingConstants.PropertyDescriptorPropertyTypeMessage}")]
    public override object? GetEditor(Type editorBaseType)
    {
        if (editorBaseType == typeof(UITypeEditor))
        {
            _editor ??= new AboutBoxUITypeEditor();
        }

        return _editor;
    }

    public override object? GetValue(object? component) => string.Empty;

    public override void ResetValue(object? component)
    {
    }

    public override void SetValue(object? component, object? value) => throw new ArgumentException(null);

    public override bool ShouldSerializeValue(object component) => false;
}
