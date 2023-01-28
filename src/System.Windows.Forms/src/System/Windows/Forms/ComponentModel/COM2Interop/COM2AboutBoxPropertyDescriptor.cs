﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal partial class Com2AboutBoxPropertyDescriptor : Com2PropertyDescriptor
{
    private TypeConverter? _converter;
    private UITypeEditor? _editor;

    public Com2AboutBoxPropertyDescriptor()
        : base(
              PInvoke.DISPID_ABOUTBOX,
              "About",
              new Attribute[]
              {
                  new DispIdAttribute(PInvoke.DISPID_ABOUTBOX),
                  DesignerSerializationVisibilityAttribute.Hidden,
                  new DescriptionAttribute(SR.AboutBoxDesc),
                  new ParenthesizePropertyNameAttribute(true)
              },
              readOnly: true,
              typeof(string),
              typeData: null,
              hrHidden: false)
    {
    }

    public override Type ComponentType => typeof(IDispatch.Interface);

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

    public override void SetValue(object? component, object? value) => throw new ArgumentException();

    public override bool ShouldSerializeValue(object component) => false;
}
