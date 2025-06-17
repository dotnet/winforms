// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DemoConsole;

internal sealed class DesignerSerializationService : IDesignerSerializationService
{
    private readonly IServiceProvider _serviceProvider;

    public DesignerSerializationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ICollection Deserialize(object serializationData)
    {
        SerializationStore serializationStore = serializationData as SerializationStore;
        if (serializationStore is not null)
        {
            ComponentSerializationService componentSerializationService = _serviceProvider.GetService(typeof(ComponentSerializationService)) as ComponentSerializationService;
            ICollection collection = componentSerializationService.Deserialize(serializationStore);
            return collection;
        }

        return Array.Empty<object>();
    }

    public object Serialize(ICollection objects)
    {
        ComponentSerializationService componentSerializationService = _serviceProvider.GetService(typeof(ComponentSerializationService)) as ComponentSerializationService;
        SerializationStore returnObject = null;
        using (SerializationStore serializationStore = componentSerializationService.CreateStore())
        {
            foreach (object obj in objects)
            {
                componentSerializationService.Serialize(serializationStore, obj);
            }

            returnObject = serializationStore;
        }

        return returnObject;
    }
}
