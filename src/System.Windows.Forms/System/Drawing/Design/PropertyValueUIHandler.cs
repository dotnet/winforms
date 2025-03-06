// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Drawing.Design;

/// <summary>Represents a delegate to be added to <see cref="IPropertyValueUIService"/>.</summary>
/// <param name="context">
///  An <see cref="ITypeDescriptorContext" /> that can be used to obtain context information.
/// </param>
/// <param name="propDesc">
///  A <see cref="PropertyDescriptor" /> that represents the property being queried.
/// </param>
/// <param name="valueUIItemList">
///  An <see cref="ArrayList" /> of <see cref="PropertyValueUIItem" /> objects containing the UI items
///  associated with the property.
/// </param>
public delegate void PropertyValueUIHandler(ITypeDescriptorContext context, PropertyDescriptor propDesc, ArrayList valueUIItemList);
