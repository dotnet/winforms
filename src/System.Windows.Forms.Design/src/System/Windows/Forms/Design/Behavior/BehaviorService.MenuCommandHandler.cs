// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design.Behavior;

public sealed partial class BehaviorService
{
    private class MenuCommandHandler : IMenuCommandService
    {
        private readonly BehaviorService _owner;
        private readonly Stack<CommandID> _currentCommands = new();

        public MenuCommandHandler(BehaviorService owner, IMenuCommandService menuService)
        {
            _owner = owner;
            MenuService = menuService;
        }

        public IMenuCommandService MenuService { get; }

        void IMenuCommandService.AddCommand(MenuCommand command) => MenuService.AddCommand(command);

        void IMenuCommandService.RemoveVerb(DesignerVerb verb) => MenuService.RemoveVerb(verb);

        void IMenuCommandService.RemoveCommand(MenuCommand command) => MenuService.RemoveCommand(command);

        MenuCommand? IMenuCommandService.FindCommand(CommandID commandID)
        {
            try
            {
                if (_currentCommands.Contains(commandID))
                {
                    return null;
                }

                _currentCommands.Push(commandID);
                return _owner.FindCommand(commandID, MenuService);
            }
            finally
            {
                _currentCommands.Pop();
            }
        }

        bool IMenuCommandService.GlobalInvoke(CommandID commandID) => MenuService.GlobalInvoke(commandID);

        void IMenuCommandService.ShowContextMenu(CommandID menuID, int x, int y)
            => MenuService.ShowContextMenu(menuID, x, y);

        void IMenuCommandService.AddVerb(DesignerVerb verb) => MenuService.AddVerb(verb);

        DesignerVerbCollection IMenuCommandService.Verbs => MenuService.Verbs;
    }
}
