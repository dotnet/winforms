// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal delegate void GetTypeConverterAndTypeEditorEventHandler(Com2PropertyDescriptor sender, GetTypeConverterAndTypeEditorEvent e);

internal class GetTypeConverterAndTypeEditorEvent : EventArgs
{
    public GetTypeConverterAndTypeEditorEvent(TypeConverter? typeConverter, object? typeEditor)
    {
        TypeEditor = typeEditor;
        TypeConverter = typeConverter;
    }

    public TypeConverter? TypeConverter { get; set; }

    public object? TypeEditor { get; set; }
}
