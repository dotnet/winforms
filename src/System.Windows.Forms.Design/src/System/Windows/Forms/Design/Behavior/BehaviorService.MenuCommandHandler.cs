// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design.Behavior
{
    public sealed partial class BehaviorService
    {
        private class MenuCommandHandler : IMenuCommandService
        {
            private readonly BehaviorService _owner;            // ptr back to the behavior service
            private readonly IMenuCommandService _menuService;  // core service used for most implementations of the IMCS interface
            private readonly Stack<CommandID> _currentCommands = new Stack<CommandID>();

            public MenuCommandHandler(BehaviorService owner, IMenuCommandService menuService)
            {
                _owner = owner;
                _menuService = menuService;
            }

            public IMenuCommandService MenuService
            {
                get => _menuService;
            }

            void IMenuCommandService.AddCommand(MenuCommand command)
            {
                _menuService.AddCommand(command);
            }

            void IMenuCommandService.RemoveVerb(DesignerVerb verb)
            {
                _menuService.RemoveVerb(verb);
            }

            void IMenuCommandService.RemoveCommand(MenuCommand command)
            {
                _menuService.RemoveCommand(command);
            }

            MenuCommand IMenuCommandService.FindCommand(CommandID commandID)
            {
                try
                {
                    if (_currentCommands.Contains(commandID))
                    {
                        return null;
                    }
                    _currentCommands.Push(commandID);
                    return _owner.FindCommand(commandID, _menuService);
                }
                finally
                {
                    _currentCommands.Pop();
                }
            }

            bool IMenuCommandService.GlobalInvoke(CommandID commandID)
            {
                return _menuService.GlobalInvoke(commandID);
            }

            void IMenuCommandService.ShowContextMenu(CommandID menuID, int x, int y)
            {
                _menuService.ShowContextMenu(menuID, x, y);
            }

            void IMenuCommandService.AddVerb(DesignerVerb verb)
            {
                _menuService.AddVerb(verb);
            }

            DesignerVerbCollection IMenuCommandService.Verbs
            {
                get => _menuService.Verbs;
            }
        }
    }
}
