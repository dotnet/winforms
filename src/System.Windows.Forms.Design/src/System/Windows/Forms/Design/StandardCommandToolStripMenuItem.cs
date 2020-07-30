// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Associates standard command with ToolStripMenuItem.
    /// </summary>
    internal class StandardCommandToolStripMenuItem : ToolStripMenuItem
    {
        private bool _cachedImage;
        private Image _image;
        private readonly CommandID _menuID;
        private IMenuCommandService _menuCommandService;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _name;
        private readonly MenuCommand _menuCommand;

        // Ok to call MenuService.FindComand to find the menuCommand mapping to the appropriated menuID.
        public StandardCommandToolStripMenuItem(CommandID menuID, string text, string imageName, IServiceProvider serviceProvider)
        {
            _menuID = menuID;
            _serviceProvider = serviceProvider;
            // Findcommand can throw; so we need to catch and disable the command.
            try
            {
                _menuCommand = MenuService.FindCommand(menuID);
            }
            catch
            {
                Enabled = false;
            }

            Text = text;
            _name = imageName;

            RefreshItem();
        }

        public void RefreshItem()
        {
            if (_menuCommand != null)
            {
                Visible = _menuCommand.Visible;
                Enabled = _menuCommand.Enabled;
                Checked = _menuCommand.Checked;
            }
        }

        /// <summary>
        ///  Retrieves the menu editor service, which we cache for speed.
        /// </summary>
        public IMenuCommandService MenuService
        {
            get
            {
                if (_menuCommandService is null)
                {
                    _menuCommandService = (IMenuCommandService)_serviceProvider.GetService(typeof(IMenuCommandService));
                }
                return _menuCommandService;
            }
        }

        public override Image Image
        {
            // Standard 'catch all - rethrow critical' exception pattern
            get
            {
                // Defer loading the image until we're sure we need it
                if (!_cachedImage)
                {
                    _cachedImage = true;
                    try
                    {
                        if (_name != null)
                        {
                            _image = new Icon(typeof(ToolStripMenuItem), _name).ToBitmap();
                        }
                        ImageTransparentColor = Color.Magenta;
                    }
                    catch (Exception ex)
                    {
                        if (ClientUtils.IsCriticalException(ex))
                        {
                            throw;
                        }
                    }
                }
                return _image;
            }
            set
            {
                _image = value;
                _cachedImage = true;
            }
        }

        protected override void OnClick(System.EventArgs e)
        {
            if (_menuCommand != null)
            {
                _menuCommand.Invoke();
            }
            else if (MenuService != null)
            {
                if (MenuService.GlobalInvoke(_menuID))
                {
                    return;
                }
            }
        }
    }
}
