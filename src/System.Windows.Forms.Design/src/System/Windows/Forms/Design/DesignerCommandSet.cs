// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    public class DesignerCommandSet
    {
        public virtual ICollection GetCommands(string name)
        {
            return null;
        }

        public DesignerVerbCollection Verbs
        {
            get => (DesignerVerbCollection)GetCommands("Verbs");
        }

        public DesignerActionListCollection ActionLists
        {
            get => (DesignerActionListCollection)GetCommands("ActionLists");
        }
    }
}
