// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;

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
            // REVIEW: Please look at this carefully - This is just copied from ControlCodeDomSerializer
            if (manager == null || codeObject == null)
            {
                throw new ArgumentNullException(manager == null ? "manager" : "codeObject");
            }

            // Find our base class's serializer.  
            CodeDomSerializer serializer = (CodeDomSerializer)manager.GetSerializer(typeof(Component), typeof(CodeDomSerializer));
            
            if (serializer == null)
            {
                Debug.Fail("Unable to find a CodeDom serializer for 'Component'.  Has someone tampered with the serialization providers?");

                return null;
            }

            return serializer.Deserialize(manager, codeObject);
        }

        /// <summary>
        ///  Serializes the given object into a CodeDom object.
        /// </summary>
        public override object Serialize(IDesignerSerializationManager manager, object value)
        {
            CodeDomSerializer baseSerializer = (CodeDomSerializer)manager.GetSerializer(typeof(ImageList).BaseType, typeof(CodeDomSerializer));
            object codeObject = baseSerializer.Serialize(manager, value);
            ImageList imageList = value as ImageList;
            
            if (imageList != null)
            {
                StringCollection imageKeys = imageList.Images.Keys;

                if (codeObject is CodeStatementCollection)
                {
                    CodeExpression imageListObject = GetExpression(manager, value);
                    
                    if (imageListObject != null)
                    {
                        CodeExpression imageListImagesProperty = new CodePropertyReferenceExpression(imageListObject, "Images");

                        if (imageListImagesProperty != null)
                        {
                            for (int i = 0; i < imageKeys.Count; i++)
                            {
                                if ((imageKeys[i] != null) || (imageKeys[i].Length != 0))
                                {
                                    CodeMethodInvokeExpression setNameMethodCall = new CodeMethodInvokeExpression(imageListImagesProperty, "SetKeyName",
                                                                                   new CodeExpression[] {
                                                                                            new CodePrimitiveExpression(i),         // SetKeyName(int,
                                                                                            new CodePrimitiveExpression(imageKeys[i])        // string);
                                                                                            });

                                    ((CodeStatementCollection)codeObject).Add(setNameMethodCall);
                                }
                            }
                        }
                    }
                }
            }

            return codeObject;
        }
    }
}
