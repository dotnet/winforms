// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design.Serialization
{
    internal static class DesignerSerializationManagerHelper
    {
        public static bool TryGetContext<T>(
            this IDesignerSerializationManager manager,
            [NotNullWhen(true)] out T? context)
        {
            if (manager.Context[typeof(T)] is T contextTemp)
            {
                context = contextTemp;
                return true;
            }

            context = default;
            return false;
        }

        public static T? GetContext<T>(this IDesignerSerializationManager manager) where T : class
        {
            return manager.Context[typeof(T)] as T;
        }

        public static T? GetSerializer<T>(this IDesignerSerializationManager manager, Type? objectType) where T : class
        {
            return manager.GetSerializer(objectType, typeof(T)) as T;
        }

        public static bool TryGetSerializer<T>(this IDesignerSerializationManager manager, Type? objectType, [NotNullWhen(true)] out T? serializer) where T : class
        {
            serializer = manager.GetSerializer(objectType, typeof(T)) as T;
            return serializer is not null;
        }
    }
}
