// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

namespace System.Windows.Forms.Design;

/// <summary>
///  This serializer serializes images.
/// </summary>
public class ImageListCodeDomSerializer : CodeDomSerializer
{
    /// <summary>
    ///  This method takes a CodeDomObject and deserializes into a real object.
    ///  We don't do anything here.
    /// </summary>
    public override object? Deserialize(IDesignerSerializationManager manager, object codeObject)
    {
        // REVIEW: Please look at this carefully - This is just copied from ControlCodeDomSerializer
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(codeObject);

        // Find our base class's serializer.
        if (!manager.TryGetSerializer(typeof(Component), out CodeDomSerializer? serializer))
        {
            Debug.Fail("Unable to find a CodeDom serializer for 'Component'. Has someone tampered with the serialization providers?");

            return null;
        }

        return serializer.Deserialize(manager, codeObject);
    }

    /// <summary>
    ///  Serializes the given object into a CodeDom object.
    /// </summary>
    public override object? Serialize(IDesignerSerializationManager manager, object value)
    {
        CodeDomSerializer baseSerializer = manager.GetSerializer<CodeDomSerializer>(typeof(ImageList).BaseType)!;
        object? codeObject = baseSerializer.Serialize(manager, value);

        if (value is ImageList imageList)
        {
            if (codeObject is CodeStatementCollection codeStatementCollection)
            {
                CodeExpression? imageListObject = GetExpression(manager, value);

                if (imageListObject is not null)
                {
                    CodeExpression imageListImagesProperty = new CodePropertyReferenceExpression(imageListObject, "Images");

                    StringCollection imageKeys = imageList.Images.Keys;

                    for (int i = 0; i < imageKeys.Count; i++)
                    {
                        if (imageKeys[i] is { Length: not 0 } imageKey)
                        {
                            CodeMethodInvokeExpression setNameMethodCall
                                = new(imageListImagesProperty, "SetKeyName",
                                    [
                                        new CodePrimitiveExpression(i),         // SetKeyName(int,
                                        new CodePrimitiveExpression(imageKey)        // string);
                                    ]);

                            codeStatementCollection.Add(setNameMethodCall);
                        }
                    }
                }
            }
        }

        return codeObject;
    }
}
