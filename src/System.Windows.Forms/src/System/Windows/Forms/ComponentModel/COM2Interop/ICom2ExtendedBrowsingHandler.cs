// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.ComponentModel.Com2Interop;

/// <summary>
///  This is the interface for handlers for COM2 extended browsing interface such as IPerPropertyBrowsing, etc.
/// </summary>
/// <remarks>
///  <para>
///   These handlers should be stateless. That is, they should keep no refernces to objects and should only work
///   on a given object and dispid. That way all objects that support a given interface can share a handler.
///  </para>
///  <para>
///   See <see cref="Com2Properties"/> for the array of handler classes to interface classes where handlers should
///   be registered.
///  </para>
/// </remarks>
internal unsafe interface ICom2ExtendedBrowsingHandler
{
    /// <summary>
    ///  Returns <see langword="true"/> if the given object is supported by this type.
    /// </summary>
    bool ObjectSupportsInterface(object @object);

    /// <summary>
    ///  Called to setup the property handlers on a given property. In this method, the handler will add listeners
    ///  to the events that the <see cref="Com2PropertyDescriptor"/> surfaces that it cares about.
    /// </summary>
    void RegisterEvents(Com2PropertyDescriptor[]? properties);
}
