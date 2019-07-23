// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design.Serialization;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  This serializer serializes images.
    /// </summary>
    public class ImageListCodeDomSerializer : CodeDomSerializer
    {
        /// <summary>
        ///  This method takes a CodeDomObject and deserializes into a real object.
        ///  We don't do anything here.
        /// </summary>
        public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Serializes the given object into a CodeDom object.
        /// </summary>
        public override object Serialize(IDesignerSerializationManager manager, object value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}
