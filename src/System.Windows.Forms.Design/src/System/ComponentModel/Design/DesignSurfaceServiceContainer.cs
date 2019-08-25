// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  A service container that supports "fixed" services.  Fixed  services cannot be removed.
    /// </summary>
    internal sealed class DesignSurfaceServiceContainer : ServiceContainer
    {
        private readonly Hashtable _fixedServices = new Hashtable();

        /// <summary>
        ///  We always add ourselves as a service.
        /// </summary>
        internal DesignSurfaceServiceContainer(IServiceProvider parentProvider) : base(parentProvider)
        {
            AddFixedService(typeof(DesignSurfaceServiceContainer), this);
        }

        /// <summary>
        ///  Removes the given service type from the service container.
        /// </summary>
        internal void AddFixedService(Type serviceType, object serviceInstance)
        {
            AddService(serviceType, serviceInstance);
            _fixedServices[serviceType] = serviceType;
        }

        /// <summary>
        ///  Removes a previously added fixed service.
        /// </summary>
        internal void RemoveFixedService(Type serviceType)
        {
            _fixedServices.Remove(serviceType);
            RemoveService(serviceType);
        }

        /// <summary>
        ///  Removes the given service type from the service container.  Throws an exception if the service is fixed.
        /// </summary>
        public override void RemoveService(Type serviceType, bool promote)
        {
            if (serviceType != null && _fixedServices.ContainsKey(serviceType))
            {
                throw new InvalidOperationException(string.Format(SR.DesignSurfaceServiceIsFixed, serviceType.Name));
            }

            base.RemoveService(serviceType, promote);
        }
    }
}
