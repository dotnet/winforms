// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  This class provides a single entrypoint used by the Behaviors, KeySize and KeyMoves (in CommandSets) and SelectionService to update the StatusBar Information.
    /// </summary>
    internal class StatusCommandUI
    {
        private MenuCommand _statusRectCommand;
        private IMenuCommandService _menuService;
        private readonly IServiceProvider _serviceProvider;

        public StatusCommandUI(IServiceProvider provider)
        {
            _serviceProvider = provider;
        }

        /// <summary>
        ///  Retrieves the menu editor service, which we cache for speed.
        /// </summary>
        private IMenuCommandService MenuService
        {
            get
            {
                if (_menuService is null)
                {
                    _menuService = (IMenuCommandService)_serviceProvider.GetService(typeof(IMenuCommandService));
                }
                return _menuService;
            }
        }

        /// <summary>
        ///  Retrieves the actual StatusRectCommand, which we cache for speed.
        /// </summary>
        private MenuCommand StatusRectCommand
        {
            get
            {
                if (_statusRectCommand is null)
                {
                    if (MenuService != null)
                    {
                        _statusRectCommand = MenuService.FindCommand(MenuCommands.SetStatusRectangle);
                    }
                }
                return _statusRectCommand;
            }
        }

        /// <summary>
        ///  Actual Function which invokes the command.
        /// </summary>
        public void SetStatusInformation(Component selectedComponent, Point location)
        {
            if (selectedComponent is null)
            {
                return;
            }
            Rectangle bounds = Rectangle.Empty;
            if (selectedComponent is Control c)
            {
                bounds = c.Bounds;
            }
            else
            {
                PropertyDescriptor BoundsProp = TypeDescriptor.GetProperties(selectedComponent)["Bounds"];
                if (BoundsProp != null && typeof(Rectangle).IsAssignableFrom(BoundsProp.PropertyType))
                {
                    bounds = (Rectangle)BoundsProp.GetValue(selectedComponent);
                }
            }
            if (location != Point.Empty)
            {
                bounds.X = location.X;
                bounds.Y = location.Y;
            }
            if (StatusRectCommand != null)
            {
                StatusRectCommand.Invoke(bounds);
            }
        }

        /// <summary>
        ///  Actual Function which invokes the command.
        /// </summary>
        public void SetStatusInformation(Component selectedComponent)
        {
            if (selectedComponent is null)
            {
                return;
            }
            Rectangle bounds = Rectangle.Empty;
            if (selectedComponent is Control c)
            {
                bounds = c.Bounds;
            }
            else
            {
                PropertyDescriptor BoundsProp = TypeDescriptor.GetProperties(selectedComponent)["Bounds"];
                if (BoundsProp != null && typeof(Rectangle).IsAssignableFrom(BoundsProp.PropertyType))
                {
                    bounds = (Rectangle)BoundsProp.GetValue(selectedComponent);
                }
            }
            if (StatusRectCommand != null)
            {
                StatusRectCommand.Invoke(bounds);
            }
        }

        /// <summary>
        ///  Actual Function which invokes the command.
        /// </summary>
        public void SetStatusInformation(Rectangle bounds)
        {
            if (StatusRectCommand != null)
            {
                StatusRectCommand.Invoke(bounds);
            }
        }
    }
}
