// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

/// <summary>
///  The extender provider service actually provides two services: IExtenderProviderService,
///  which allows other objects to add and remove extender providers, and IExtenderListService,
///  which is used by TypeDescriptor to discover the set of extender providers.
/// </summary>
internal sealed class ExtenderProviderService : IExtenderProviderService, IExtenderListService
{
    private List<IExtenderProvider>? _providers;

    internal ExtenderProviderService()
    {
    }

    /// <summary>
    ///  Gets the set of extender providers for the component.
    /// </summary>
    IExtenderProvider[] IExtenderListService.GetExtenderProviders() =>
        _providers is not null ? ([.. _providers]) : ([]);

    /// <summary>
    ///  Adds an extender provider.
    /// </summary>
    void IExtenderProviderService.AddExtenderProvider(IExtenderProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        _providers ??= new(4);

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
        ArgumentNullException.ThrowIfNull(provider);

        _providers?.Remove(provider);
    }
}
