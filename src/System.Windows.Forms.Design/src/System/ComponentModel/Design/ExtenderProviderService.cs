// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  The extender provider service actually provides two services: IExtenderProviderService,
    ///  which allows other objects to add and remove extender providers, and IExtenderListService,
    ///  which is used by TypeDescriptor to discover the set of extender providers.
    /// </summary>
    internal sealed class ExtenderProviderService : IExtenderProviderService, IExtenderListService
    {
        private ArrayList _providers;

        internal ExtenderProviderService()
        {
        }

        /// <summary>
        ///  Gets the set of extender providers for the component.
        /// </summary>
        IExtenderProvider[] IExtenderListService.GetExtenderProviders()
        {
            if (_providers != null)
            {
                IExtenderProvider[] providers = new IExtenderProvider[_providers.Count];
                _providers.CopyTo(providers, 0);
                return providers;
            }
            return Array.Empty<IExtenderProvider>();
        }

        /// <summary>
        ///  Adds an extender provider.
        /// </summary>
        void IExtenderProviderService.AddExtenderProvider(IExtenderProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (_providers == null)
            {
                _providers = new ArrayList(4);
            }

            if (_providers.Contains(provider))
            {
                throw new ArgumentException(string.Format(SR.ExtenderProviderServiceDuplicateProvider, provider), nameof(provider));
            }

            _providers.Add(provider);
        }

        /// <summary>
        ///  Removes an extender provider.
        /// </summary>
        void IExtenderProviderService.RemoveExtenderProvider(IExtenderProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            _providers?.Remove(provider);
        }
    }
}
