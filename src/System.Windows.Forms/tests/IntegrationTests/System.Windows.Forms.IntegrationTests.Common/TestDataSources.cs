// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Windows.Forms.IntegrationTests.Common
{
    public static class TestDataSources
    {
        public static List<Person> GetPersons()
            => new List<Person>()
            {
                new Person(1, "Name 1"),
                new Person(2, "Name 2"),
                new Person(3, "Name 3"),
                new Person(4, "Name 4"),
                new Person(5, "Name 5"),
            };

        public const string PersonDisplayMember = "Name";
    }
}
