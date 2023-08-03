// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal delegate void GetAttributesEventHandler(Com2PropertyDescriptor sender, GetAttributesEvent e);

internal class GetAttributesEvent : EventArgs
{
    private readonly List<Attribute> _attributeList;

    public GetAttributesEvent(List<Attribute> attributeList) => _attributeList = attributeList;

    public void Add(Attribute attribute) => _attributeList.Add(attribute);
}
