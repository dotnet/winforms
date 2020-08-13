// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace System.Resources.Tests
{
    // NB: doesn't require thread affinity
    public class ResXResourceWriterTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void TestRoundTrip()
        {
            var key = "Some.Key.Name";
            var value = "Some.Key.Value";

            using (var stream = new MemoryStream())
            {
                using (var writer = new ResXResourceWriter(stream))
                {
                    writer.AddResource(key, value);
                }

                var buffer = stream.ToArray();
                using (var reader = new ResXResourceReader(new MemoryStream(buffer)))
                {
                    var dictionary = new Dictionary<object, object>();
                    IDictionaryEnumerator dictionaryEnumerator = reader.GetEnumerator();
                    while (dictionaryEnumerator.MoveNext())
                    {
                        dictionary.Add(dictionaryEnumerator.Key, dictionaryEnumerator.Value);
                    }

                    KeyValuePair<object, object> pair = Assert.Single(dictionary);
                    Assert.Equal(key, pair.Key);
                    Assert.Equal(value, pair.Value);
                }
            }
        }
    }
}
