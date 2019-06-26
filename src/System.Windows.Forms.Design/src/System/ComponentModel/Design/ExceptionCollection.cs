// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace System.ComponentModel.Design
{
    // [Serializable] necessary for round trip binary serialization
    [Serializable]
    public sealed class ExceptionCollection : Exception
    {
        private const string SerializationKey = "exceptions";

        public ExceptionCollection(List<Exception> exceptions)
        {
            Exceptions = exceptions ?? throw new ArgumentNullException(nameof(exceptions));
        }

        private ExceptionCollection(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Exceptions = info.GetValue(SerializationKey, typeof(List<Exception>)) as List<Exception> ?? new List<Exception>();
        }

        public IReadOnlyList<Exception> Exceptions { get; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(SerializationKey, Exceptions);
            base.GetObjectData(info, context);
        }
    }
}
