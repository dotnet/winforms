// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel.Design;

namespace System.ComponentModel.Design
{
    public class DesignerCommandSet
    {
        private protected const string VerbsCommand = "Verbs";
        private protected const string ActionListsCommand = "ActionLists";

        public virtual ICollection GetCommands(string name) => null;

        public DesignerVerbCollection Verbs => (DesignerVerbCollection)GetCommands(VerbsCommand);

        public DesignerActionListCollection ActionLists => (DesignerActionListCollection)GetCommands(ActionListsCommand);
    }
}
