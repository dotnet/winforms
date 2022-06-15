// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    internal partial class OleDragDropHandler
    {
        // just so we can recognize the ones we create
        protected class ComponentDataObjectWrapper : DataObject
        {
            readonly ComponentDataObject innerData;

            public ComponentDataObjectWrapper(ComponentDataObject dataObject) : base(dataObject)
            {
                innerData = dataObject;
            }

            public ComponentDataObject InnerData
            {
                get
                {
                    return innerData;
                }
            }
        }
    }
}
