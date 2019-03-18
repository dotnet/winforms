// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Runtime.Serialization;
using System.Security;

namespace System.ComponentModel.Design
{
    [Serializable]
    public sealed class ExceptionCollection : Exception
    {
        readonly ArrayList _exceptions;

        public ExceptionCollection(ArrayList exceptions)
        {
            _exceptions = exceptions;
        }

        /// <summary>
        /// Need this constructor since Exception implements ISerializable. 
        /// </summary>
        private ExceptionCollection(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _exceptions = (ArrayList)info.GetValue("exceptions", typeof(ArrayList));
        }

        public ArrayList Exceptions
        {
            get
            {
                if (_exceptions != null)
                {
                    return (ArrayList)_exceptions.Clone();
                }
                return null;
            }
        }

        /// <summary>
        /// Need this since Exception implements ISerializable and we have fields to save out.
        /// </summary>
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("exceptions", _exceptions);
            base.GetObjectData(info, context);
        }
    }
}
