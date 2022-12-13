﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Runtime.Serialization;

namespace System.ComponentModel.Design
{
    public sealed class ExceptionCollection : Exception
    {
        private readonly ArrayList _exceptions;

        public ExceptionCollection(ArrayList exceptions)
        {
            _exceptions = exceptions;
        }

        public ArrayList Exceptions => (ArrayList)_exceptions?.Clone();

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new PlatformNotSupportedException();
        }
    }
}
