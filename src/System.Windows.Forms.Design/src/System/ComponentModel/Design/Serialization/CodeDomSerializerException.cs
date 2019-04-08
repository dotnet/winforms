// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  The exception that is thrown when the code dom serializer experiences an error.
    /// </summary>
    [Serializable]
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
    public class CodeDomSerializerException : SystemException
    {

        private readonly CodeLinePragma _linePragma;

        /// <summary>
        /// Initializes a new instance of the CodeDomSerializerException class.
        /// </summary>
        public CodeDomSerializerException(string message, CodeLinePragma linePragma) : base(message)
        {
            _linePragma = linePragma;
        }

        /// <summary>
        /// Initializes a new instance of the CodeDomSerializerException class.
        /// </summary>
        public CodeDomSerializerException(Exception ex, CodeLinePragma linePragma) : base(ex.Message, ex)
        {
            _linePragma = linePragma;
        }

        /// <summary>
        /// Initializes a new instance of the CodeDomSerializerException class.
        /// </summary>
        public CodeDomSerializerException(string message, IDesignerSerializationManager manager) : base(message)
        {
            FillLinePragmaFromContext(manager);
        }

        /// <summary>
        /// Initializes a new instance of the CodeDomSerializerException class.
        /// </summary>
        public CodeDomSerializerException(Exception ex, IDesignerSerializationManager manager) : base(ex.Message, ex)
        {
            FillLinePragmaFromContext(manager);
        }

        protected CodeDomSerializerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _linePragma = (CodeLinePragma)info.GetValue("linePragma", typeof(CodeLinePragma));
        }

        /// <summary>
        /// Gets the line pragma object that is related to this error.
        /// </summary>
        public CodeLinePragma LinePragma
        {
            get => _linePragma;
        }

        /// <summary>
        /// Sniffs around in the context looking for a code statement.  if it finds one, it will add the statement's line # information to the exception.
        /// </summary>
        private void FillLinePragmaFromContext(IDesignerSerializationManager manager)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            info.AddValue("linePragma", _linePragma);
            base.GetObjectData(info, context);
        }
    }
}
