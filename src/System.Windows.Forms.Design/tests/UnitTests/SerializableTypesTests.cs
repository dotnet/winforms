// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design.Serialization;
using Xunit;

namespace System.Windows.Forms.Design.Tests.Serialization
{
    public class SerializableTypesTests
    {
        [Fact(Skip ="AssemblyName is not serializable")]
        public void CodeDomSerializationStore_RoundTripInCore()
        {
            // For this class we don't know of real live scenarios that require 
            // exchange with the .NET framework, thus exchange is not enabled.
            var service = new CodeDomComponentSerializationService();
            var store = service.CreateStore();
            var button = new Button()
            {
                Name = "button1",
                Location = new Drawing.Point(11, 11),
                Size = new Drawing.Size(20, 20)
            };
            service.Serialize(store, button);
            store.Close();

            var blob = BinarySerialization.ToBase64String(store);
            var result = BinarySerialization.EnsureDeserialize<SerializationStore>(blob);

            var collection = service.Deserialize(result);
            Assert.NotNull(collection);
            Assert.Equal(1, collection.Count);
            object[] items = new object[1];
            collection.CopyTo(items, 0);
            var copy = items[0] as Button;
            Assert.NotNull(copy);
            Assert.Equal(button.Name, copy.Name);
            Assert.Equal(button.Location, copy.Location);
            Assert.Equal(button.Size, copy.Size);
        }
    }
}
