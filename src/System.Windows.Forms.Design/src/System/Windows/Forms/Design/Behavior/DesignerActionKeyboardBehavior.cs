// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using System.Diagnostics;

namespace System.Windows.Forms.Design.Behavior
{
    internal sealed class DesignerActionKeyboardBehavior : Behavior
    {
        private readonly DesignerActionPanel _panel;
        private readonly IMenuCommandService _menuService;
        private readonly DesignerActionUIService _daUISvc;
        private static readonly Guid s_vSStandardCommandSet97 = new Guid("{5efc7975-14bc-11cf-9b2b-00aa00573819}");

        public DesignerActionKeyboardBehavior(DesignerActionPanel panel, IServiceProvider serviceProvider, BehaviorService behaviorService) :
            base(true, behaviorService)
        {
            _panel = panel;
            if (serviceProvider != null)
            {
                _menuService = serviceProvider.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
                Debug.Assert(_menuService != null, "we should have found a menu service here...");
                _daUISvc = serviceProvider.GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
            }
        }
        // THIS should not stay here, creation of a custom command or of the real thing should be handled in the designeractionpanel itself
        public override MenuCommand FindCommand(CommandID commandId)
        {
            if (_panel != null && _menuService != null)
            {
                // if the command we're looking for is handled by the panel, just tell VS that this command is disabled. otherwise let it through as usual...
                foreach (CommandID candidateCommandId in _panel.FilteredCommandIDs)
                {
                    // VisualStudio shell implements a mutable derived class from the base CommandID. The mutable class compares overridden properties instead of the read-only backing fields when testing equality of command IDs. Thus Equals method is asymmetrical derived class's override that compares properties is the accurate one.
                    if (commandId.Equals(candidateCommandId))
                    {
                        MenuCommand dummyMC = new MenuCommand(delegate
                        { }, commandId)
                        {
                            Enabled = false
                        };
                        return dummyMC;
                    }
                }
                // in case of a ctrl-tab we need to close the DAP
                if (_daUISvc != null && commandId.Guid == DesignerActionKeyboardBehavior.s_vSStandardCommandSet97 && commandId.ID == 1124)
                {
                    _daUISvc.HideUI(null);
                }
            }
            return base.FindCommand(commandId); // this will route the request to the parent behavior
        }
    }
}
