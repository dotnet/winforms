﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

namespace System.Windows.Forms.Design
{
    public partial class ControlDesigner
    {
        // Custom code dom serializer for the DesignerControlCollection. We need this so we can filter out controls
        // that aren't sited in the host's container.
        internal class DesignerControlCollectionCodeDomSerializer : CollectionCodeDomSerializer
        {
            protected override object SerializeCollection(
                IDesignerSerializationManager manager,
                CodeExpression targetExpression,
                Type targetType,
                ICollection originalCollection,
                ICollection valuesToSerialize)
            {
                ArrayList subset = new ArrayList();
                if (valuesToSerialize != null && valuesToSerialize.Count > 0)
                {
                    foreach (object val in valuesToSerialize)
                    {
                        if (val is IComponent comp && comp.Site != null && !(comp.Site is INestedSite))
                        {
                            subset.Add(comp);
                        }
                    }
                }

                return base.SerializeCollection(manager, targetExpression, targetType, originalCollection, subset);
            }
        }
    }
}
