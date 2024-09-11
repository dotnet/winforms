// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Resources;

namespace System.ComponentModel.Design.Serialization;

public sealed partial class CodeDomComponentSerializationService
{
    private sealed partial class CodeDomSerializationStore
    {
        /// <summary>
        ///  LocalServices contains the services that we add to our serialization manager.
        ///  We do this, rather than implement interfaces directly on <see cref="CodeDomSerializationStore"/>
        ///  to prevent people from assuming what our implementation is
        ///  (CodeDomSerializationStore is returned publicly as <see cref="SerializationStore"/>).
        /// </summary>
        private class LocalServices : IServiceProvider, IResourceService
        {
            private readonly CodeDomSerializationStore _store;
            private readonly IServiceProvider? _provider;

            internal LocalServices(CodeDomSerializationStore store, IServiceProvider? provider)
            {
                _store = store;
                _provider = provider;
            }

            // IResourceService
            IResourceReader IResourceService.GetResourceReader(CultureInfo info) { return _store.Resources; }
            IResourceWriter IResourceService.GetResourceWriter(CultureInfo info) { return _store.Resources; }

            // IServiceProvider
            object? IServiceProvider.GetService(Type serviceType)
            {
                ArgumentNullException.ThrowIfNull(serviceType);

                if (serviceType == typeof(IResourceService))
                {
                    return this;
                }

                return _provider?.GetService(serviceType);
            }
        }
    }
}
