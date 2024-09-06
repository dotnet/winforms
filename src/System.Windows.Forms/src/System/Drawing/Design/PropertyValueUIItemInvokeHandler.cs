// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Drawing.Design;

/// <summary>
///  Represents a method that will handle the event raised when an icon in the properties window associated with a
///  <see cref="PropertyValueUIItem"/> is double-clicked.
/// </summary>
/// <param name="context">
///  The <see cref="ITypeDescriptorContext" /> for the property associated with the icon that was double-clicked.
/// </param>
/// <param name="descriptor">The property associated with the icon that was double-clicked. </param>
/// <param name="invokedItem">
///  The <see cref="PropertyValueUIItem" /> associated with the icon that was double-clicked.
/// </param>
public delegate void PropertyValueUIItemInvokeHandler(ITypeDescriptorContext context, PropertyDescriptor descriptor, PropertyValueUIItem invokedItem);
