// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design;

/// <summary>
///  This class provides a single entrypoint used by the Behaviors, KeySize and KeyMoves (in CommandSets) and
///  SelectionService to update the StatusBar Information.
/// </summary>
internal sealed class StatusCommandUI
{
    private MenuCommand? _statusRectCommand;
    private IMenuCommandService? _menuService;
    private readonly IServiceProvider _serviceProvider;

    public StatusCommandUI(IServiceProvider provider)
    {
        _serviceProvider = provider;
    }

    /// <summary>
    ///  Retrieves the menu editor service, which we cache for speed.
    /// </summary>
    private IMenuCommandService? MenuService => _menuService ??= _serviceProvider.GetService<IMenuCommandService>();

    /// <summary>
    ///  Retrieves the actual StatusRectCommand, which we cache for speed.
    /// </summary>
    private MenuCommand? StatusRectCommand => _statusRectCommand ??= MenuService?.FindCommand(MenuCommands.SetStatusRectangle);

    /// <summary>
    ///  Actual Function which invokes the command.
    /// </summary>
    public void SetStatusInformation(Component? selectedComponent, Point location)
    {
        if (selectedComponent is null)
        {
            return;
        }

        Rectangle bounds;
        if (selectedComponent is Control c)
        {
            bounds = c.Bounds;
        }
        else if (!TypeDescriptorHelper.TryGetPropertyValue(selectedComponent, "Bounds", out bounds))
        {
            bounds = Rectangle.Empty;
        }

        if (location != Point.Empty)
        {
            bounds.X = location.X;
            bounds.Y = location.Y;
        }

        StatusRectCommand?.Invoke(bounds);
    }

    /// <summary>
    ///  Actual Function which invokes the command.
    /// </summary>
    public void SetStatusInformation(Component? selectedComponent)
    {
        if (selectedComponent is null)
        {
            return;
        }

        Rectangle bounds;
        if (selectedComponent is Control c)
        {
            bounds = c.Bounds;
        }
        else if (!TypeDescriptorHelper.TryGetPropertyValue(selectedComponent, "Bounds", out bounds))
        {
            bounds = Rectangle.Empty;
        }

        StatusRectCommand?.Invoke(bounds);
    }

    /// <summary>
    ///  Actual Function which invokes the command.
    /// </summary>
    public void SetStatusInformation(Rectangle bounds)
    {
        StatusRectCommand?.Invoke(bounds);
    }
}
