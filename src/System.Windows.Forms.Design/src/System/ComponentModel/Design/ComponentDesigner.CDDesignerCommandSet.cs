// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.ComponentModel.Design
{
    public partial class ComponentDesigner
    {
        /// <summary>
        ///  DesignerCommandSet to be used as a site specific service.
        /// </summary>
        private class CDDesignerCommandSet : DesignerCommandSet
        {
            private readonly ComponentDesigner _componentDesigner;

            public CDDesignerCommandSet(ComponentDesigner componentDesigner)
            {
                _componentDesigner = componentDesigner;
            }

            public override ICollection GetCommands(string name)
            {
                if (name == VerbsCommand)
                {
                    return _componentDesigner.Verbs;
                }
                else if (name == ActionListsCommand)
                {
                    return _componentDesigner.ActionLists;
                }
                else
                {
                    return base.GetCommands(name);
                }
            }
        }
    }
}
