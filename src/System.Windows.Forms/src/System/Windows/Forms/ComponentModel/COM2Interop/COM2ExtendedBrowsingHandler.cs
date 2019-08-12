// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  This is the base class for handlers for COM2 extended browsing interface
    ///  such as IPerPropertyBrowsing, etc.
    ///
    ///  These handlers should be stateless.  That is, they should keep no refs to object
    ///  and should only work on a give object and dispid.  That way all objects that
    ///  support a give interface can share a handler.
    ///
    ///  See COM2Properties for the array of handler classes to interface classes
    ///  where handlers should be registered.
    /// </summary>
    internal abstract class Com2ExtendedBrowsingHandler
    {
        /// <summary>
        ///  The interface that this handler managers
        ///  such as IPerPropertyBrowsing, IProvidePropertyBuilder, etc.
        /// </summary>
        public abstract Type Interface
        {
            get;
        }

        /// <summary>
        ///  Called to setup the property handlers on a given property
        ///  In this method, the handler will add listeners to the events that
        ///  the COM2PropertyDescriptor surfaces that it cares about.
        /// </summary>
        public virtual void SetupPropertyHandlers(Com2PropertyDescriptor propDesc)
        {
            SetupPropertyHandlers(new Com2PropertyDescriptor[] { propDesc });
        }

        /// <summary>
        ///  Called to setup the property handlers on a given properties
        ///  In this method, the handler will add listeners to the events that
        ///  the Com2PropertyDescriptor surfaces that it cares about.
        /// </summary>
        public abstract void SetupPropertyHandlers(Com2PropertyDescriptor[] propDesc);
    }
}
