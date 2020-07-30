// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms.Design.Behavior;
using Microsoft.Win32;

namespace System.Windows.Forms.Design
{
    internal sealed class ToolStripDesignerUtils
    {
        private static readonly Type s_toolStripItemType = typeof(ToolStripItem);
        [ThreadStatic]
        private static Dictionary<Type, ToolboxItem> s_cachedToolboxItems;
        [ThreadStatic]
        private static int s_customToolStripItemCount = 0;
        private const int TOOLSTRIPCHARCOUNT = 9;
        // used to cache in the selection. This is used when the selection is changing and we need to invalidate the original selection Especially, when the selection is changed through NON UI designer action like through propertyGrid or through Doc Outline.
        public static ArrayList originalSelComps;

        [ThreadStatic]
        private static Dictionary<Type, Bitmap> s_cachedWinformsImages;
        private static readonly string s_systemWindowsFormsNamespace = typeof(ToolStripItem).Namespace;

        private ToolStripDesignerUtils()
        {
        }

        #region NewItemTypeLists
        // This section controls the ordering of standard item types in all the various pieces of toolstrip designer UI.
        // ToolStrip - Default item is determined by being first in the list
        private static readonly Type[] s_newItemTypesForToolStrip =
            new Type[] { typeof(ToolStripButton), typeof(ToolStripLabel), typeof(ToolStripSplitButton), typeof(ToolStripDropDownButton), typeof(ToolStripSeparator), typeof(ToolStripComboBox), typeof(ToolStripTextBox), typeof(ToolStripProgressBar) };
        // StatusStrip - Default item is determined by being first in the list
        private static readonly Type[] s_newItemTypesForStatusStrip =
            new Type[] { typeof(ToolStripStatusLabel), typeof(ToolStripProgressBar), typeof(ToolStripDropDownButton), typeof(ToolStripSplitButton) };
        // MenuStrip - Default item is determined by being first in the list
        private static readonly Type[] s_newItemTypesForMenuStrip =
            new Type[] { typeof(ToolStripMenuItem), typeof(ToolStripComboBox), typeof(ToolStripTextBox) };
        // ToolStripDropDown - roughly same as menu strip.
        private static readonly Type[] s_newItemTypesForToolStripDropDownMenu =
            new Type[] { typeof(ToolStripMenuItem), typeof(ToolStripComboBox), typeof(ToolStripSeparator), typeof(ToolStripTextBox) };
        #endregion

        // Get the Correct bounds for painting...
        public static void GetAdjustedBounds(ToolStripItem item, ref Rectangle r)
        {
            // adjust bounds as per item
            if (!(item is ToolStripControlHost && item.IsOnDropDown))
            {
                //Deflate the SelectionGlyph for the MenuItems...
                if (item is ToolStripMenuItem && item.IsOnDropDown)
                {
                    r.Inflate(-3, -2);
                    r.Width++;
                }
                else if (item is ToolStripControlHost && !item.IsOnDropDown)
                {
                    r.Inflate(0, -2);
                }
                else if (item is ToolStripMenuItem && !item.IsOnDropDown)
                {
                    r.Inflate(-3, -3);
                }
                else
                {
                    r.Inflate(-1, -1);
                }
            }
        }

        /// <summary>
        ///  If IComponent is ToolStrip return ToolStrip
        ///  If IComponent is ToolStripItem return the Owner ToolStrip
        ///  If IComponent is ToolStripDropDownItem return the child DropDown ToolStrip
        /// </summary>
        private static ToolStrip GetToolStripFromComponent(IComponent component)
        {
            ToolStrip parent;
            if (component is ToolStripItem stripItem)
            {
                if (!(stripItem is ToolStripDropDownItem))
                {
                    parent = stripItem.Owner;
                }
                else
                {
                    parent = ((ToolStripDropDownItem)stripItem).DropDown;
                }
            }
            else
            {
                parent = component as ToolStrip;
            }
            return parent;
        }

        private static ToolboxItem GetCachedToolboxItem(Type itemType)
        {
            ToolboxItem tbxItem = null;
            if (s_cachedToolboxItems is null)
            {
                s_cachedToolboxItems = new Dictionary<Type, ToolboxItem>();
            }
            else if (s_cachedToolboxItems.ContainsKey(itemType))
            {
                tbxItem = s_cachedToolboxItems[itemType];
                return tbxItem;
            }

            // no cache hit - load the item.
            if (tbxItem is null)
            {
                // create a toolbox item to match
                tbxItem = new ToolboxItem(itemType);
            }

            s_cachedToolboxItems[itemType] = tbxItem;
            if (s_customToolStripItemCount > 0 && (s_customToolStripItemCount * 2 < s_cachedToolboxItems.Count))
            {
                // time to clear the toolbox item cache - we've got twice the number of toolbox items than actual custom item types.
                s_cachedToolboxItems.Clear();
            }
            return tbxItem;
        }

        // only call this for well known items.
        private static Bitmap GetKnownToolboxBitmap(Type itemType)
        {
            if (s_cachedWinformsImages is null)
            {
                s_cachedWinformsImages = new Dictionary<Type, Bitmap>();
            }
            if (!s_cachedWinformsImages.ContainsKey(itemType))
            {
                Bitmap knownImage = ToolboxBitmapAttribute.GetImageFromResource(itemType, null, false) as Bitmap;
                s_cachedWinformsImages[itemType] = knownImage;
                return knownImage;
            }
            return s_cachedWinformsImages[itemType];
        }

        public static Bitmap GetToolboxBitmap(Type itemType)
        {
            // if and only if the namespace of the type is System.Windows.Forms. try to pull the image out of the manifest.
            if (itemType.Namespace == s_systemWindowsFormsNamespace)
            {
                return GetKnownToolboxBitmap(itemType);
            }

            // check to see if we've got a toolbox item, and use it.
            ToolboxItem tbxItem = GetCachedToolboxItem(itemType);
            if (tbxItem != null)
            {
                return tbxItem.Bitmap;
            }
            // if all else fails, throw up a default image.
            return GetKnownToolboxBitmap(typeof(Component));
        }

        /// <summary>
        ///  Fishes out the display name attribute from the Toolbox item  if not present, uses Type.Name
        /// </summary>
        public static string GetToolboxDescription(Type itemType)
        {
            string currentName = null;
            ToolboxItem tbxItem = GetCachedToolboxItem(itemType);
            if (tbxItem != null)
            {
                currentName = tbxItem.DisplayName;
            }
            if (currentName is null)
            {
                currentName = itemType.Name;
            }
            if (currentName.StartsWith("ToolStrip"))
            {
                return currentName.Substring(TOOLSTRIPCHARCOUNT);
            }
            return currentName;
        }

        /// <summary>
        ///  The first item returned should be the DefaultItem to create on the ToolStrip
        /// </summary>
        public static Type[] GetStandardItemTypes(IComponent component)
        {
            ToolStrip toolStrip = GetToolStripFromComponent(component);
            if (toolStrip is MenuStrip)
            {
                return s_newItemTypesForMenuStrip;
            }
            else if (toolStrip is ToolStripDropDownMenu)
            {
                // ToolStripDropDown gets default items.
                return s_newItemTypesForToolStripDropDownMenu;
            }
            else if (toolStrip is StatusStrip)
            {
                return s_newItemTypesForStatusStrip;
            }
            Debug.Assert(toolStrip != null, "why werent we handed a toolstrip here? returning default list");
            return s_newItemTypesForToolStrip;
        }

        private static ToolStripItemDesignerAvailability GetDesignerVisibility(ToolStrip toolStrip)
        {
            ToolStripItemDesignerAvailability visiblity;
            if (toolStrip is StatusStrip)
            {
                visiblity = ToolStripItemDesignerAvailability.StatusStrip;
            }
            else if (toolStrip is MenuStrip)
            {
                visiblity = ToolStripItemDesignerAvailability.MenuStrip;
            }
            else if (toolStrip is ToolStripDropDownMenu)
            {
                visiblity = ToolStripItemDesignerAvailability.ContextMenuStrip;
            }
            else
            {
                visiblity = ToolStripItemDesignerAvailability.ToolStrip;
            }
            return visiblity;
        }

        public static Type[] GetCustomItemTypes(IComponent component, IServiceProvider serviceProvider)
        {
            ITypeDiscoveryService discoveryService = null;
            if (serviceProvider != null)
            {
                discoveryService = serviceProvider.GetService(typeof(ITypeDiscoveryService)) as ITypeDiscoveryService;
            }
            return GetCustomItemTypes(component, discoveryService);
        }
        public static Type[] GetCustomItemTypes(IComponent component, ITypeDiscoveryService discoveryService)
        {
            if (discoveryService != null)
            {
                // fish out all types which derive from toolstrip item
                ICollection itemTypes = discoveryService.GetTypes(s_toolStripItemType, false /*excludeGlobalTypes*/);
                ToolStrip toolStrip = GetToolStripFromComponent(component);
                // determine the value of the visibility attribute which matches the current toolstrip type.
                ToolStripItemDesignerAvailability currentToolStripVisibility = GetDesignerVisibility(toolStrip);
                Debug.Assert(currentToolStripVisibility != ToolStripItemDesignerAvailability.None, "Why is GetDesignerVisibility returning None?");
                // fish out the ones we've already listed.
                Type[] stockItemTypeList = GetStandardItemTypes(component);
                if (currentToolStripVisibility != ToolStripItemDesignerAvailability.None)
                {
                    ArrayList createableTypes = new ArrayList(itemTypes.Count);
                    foreach (Type t in itemTypes)
                    {
                        if (t.IsAbstract)
                        {
                            continue;
                        }
                        if (!t.IsPublic && !t.IsNestedPublic)
                        {
                            continue;
                        }
                        if (t.ContainsGenericParameters)
                        {
                            continue;
                        }
                        // Check if we have public constructor...
                        ConstructorInfo ctor = t.GetConstructor(Array.Empty<Type>());
                        if (ctor is null)
                        {
                            continue;
                        }

                        // if the visibility matches the current toolstrip type,  add it to the list of possible types to create.
                        ToolStripItemDesignerAvailabilityAttribute visiblityAttribute = (ToolStripItemDesignerAvailabilityAttribute)TypeDescriptor.GetAttributes(t)[typeof(ToolStripItemDesignerAvailabilityAttribute)];
                        if (visiblityAttribute != null && ((visiblityAttribute.ItemAdditionVisibility & currentToolStripVisibility) == currentToolStripVisibility))
                        {
                            bool isStockType = false;
                            // PERF: consider a dictionary - but this list will usually be 3-7 items.
                            foreach (Type stockType in stockItemTypeList)
                            {
                                if (stockType == t)
                                {
                                    isStockType = true;
                                    break;
                                }
                            }
                            if (!isStockType)
                            {
                                createableTypes.Add(t);
                            }
                        }
                    }

                    if (createableTypes.Count > 0)
                    {
                        Type[] createableTypesArray = new Type[createableTypes.Count];
                        createableTypes.CopyTo(createableTypesArray, 0);
                        s_customToolStripItemCount = createableTypes.Count;
                        return createableTypesArray;
                    }
                }
            }
            s_customToolStripItemCount = 0;
            return Array.Empty<Type>();
        }

        /// <summary>
        ///  wraps the result of GetStandardItemTypes in ItemTypeToolStripMenuItems.
        /// </summary>
        public static ToolStripItem[] GetStandardItemMenuItems(IComponent component, EventHandler onClick, bool convertTo)
        {
            Type[] standardTypes = GetStandardItemTypes(component);
            ToolStripItem[] items = new ToolStripItem[standardTypes.Length];
            for (int i = 0; i < standardTypes.Length; i++)
            {
                ItemTypeToolStripMenuItem item = new ItemTypeToolStripMenuItem(standardTypes[i])
                {
                    ConvertTo = convertTo
                };
                if (onClick != null)
                {
                    item.Click += onClick;
                }
                items[i] = item;
            }
            return items;
        }

        /// <summary>
        ///  wraps the result of GetCustomItemTypes in ItemTypeToolStripMenuItems.
        /// </summary>
        public static ToolStripItem[] GetCustomItemMenuItems(IComponent component, EventHandler onClick, bool convertTo, IServiceProvider serviceProvider)
        {
            Type[] customTypes = GetCustomItemTypes(component, serviceProvider);
            ToolStripItem[] items = new ToolStripItem[customTypes.Length];
            for (int i = 0; i < customTypes.Length; i++)
            {
                ItemTypeToolStripMenuItem item = new ItemTypeToolStripMenuItem(customTypes[i])
                {
                    ConvertTo = convertTo
                };
                if (onClick != null)
                {
                    item.Click += onClick;
                }
                items[i] = item;
            }
            return items;
        }

        /// <summary>
        ///  build up a list of standard items separated by the custom items
        /// </summary>
        public static NewItemsContextMenuStrip GetNewItemDropDown(IComponent component, ToolStripItem currentItem, EventHandler onClick, bool convertTo, IServiceProvider serviceProvider, bool populateCustom)
        {
            NewItemsContextMenuStrip contextMenu = new NewItemsContextMenuStrip(component, currentItem, onClick, convertTo, serviceProvider);
            contextMenu.GroupOrdering.Add("StandardList");
            contextMenu.GroupOrdering.Add("CustomList");
            // plumb through the standard and custom items.
            foreach (ToolStripItem item in GetStandardItemMenuItems(component, onClick, convertTo))
            {
                contextMenu.Groups["StandardList"].Items.Add(item);
                if (convertTo)
                {
                    if (item is ItemTypeToolStripMenuItem toolItem && currentItem != null && toolItem.ItemType == currentItem.GetType())
                    {
                        toolItem.Enabled = false;
                    }
                }
            }
            if (populateCustom)
            {
                GetCustomNewItemDropDown(contextMenu, component, currentItem, onClick, convertTo, serviceProvider);
            }
            if (serviceProvider.GetService(typeof(IUIService)) is IUIService uis)
            {
                contextMenu.Renderer = (ToolStripProfessionalRenderer)uis.Styles["VsRenderer"];
                contextMenu.Font = (Font)uis.Styles["DialogFont"];
                if (uis.Styles["VsColorPanelText"] is Color)
                {
                    contextMenu.ForeColor = (Color)uis.Styles["VsColorPanelText"];
                }
            }
            contextMenu.Populate();
            return contextMenu;
        }

        public static void GetCustomNewItemDropDown(NewItemsContextMenuStrip contextMenu, IComponent component, ToolStripItem currentItem, EventHandler onClick, bool convertTo, IServiceProvider serviceProvider)
        {
            foreach (ToolStripItem item in GetCustomItemMenuItems(component, onClick, convertTo, serviceProvider))
            {
                contextMenu.Groups["CustomList"].Items.Add(item);
                if (convertTo)
                {
                    if (item is ItemTypeToolStripMenuItem toolItem && currentItem != null && toolItem.ItemType == currentItem.GetType())
                    {
                        toolItem.Enabled = false;
                    }
                }
            }
            contextMenu.Populate();
        }

        public static void InvalidateSelection(ArrayList originalSelComps, ToolStripItem nextSelection, IServiceProvider provider, bool shiftPressed)
        {
            // if we are not selecting a ToolStripItem then return (dont invalidate).
            if (nextSelection is null || provider is null)
            {
                return;
            }
            //InvalidateOriginal SelectedComponents.
            Region invalidateRegion = null;
            Region itemRegion = null;
            int GLYPHBORDER = 1;
            int GLYPHINSET = 2;
            ToolStripItemDesigner designer = null;
            bool templateNodeSelected = false;

            try
            {
                Rectangle invalidateBounds = Rectangle.Empty;
                IDesignerHost designerHost = (IDesignerHost)provider.GetService(typeof(IDesignerHost));

                if (designerHost != null)
                {
                    foreach (Component comp in originalSelComps)
                    {
                        if (comp is ToolStripItem selItem)
                        {
                            if ((originalSelComps.Count > 1) ||
                               (originalSelComps.Count == 1 && selItem.GetCurrentParent() != nextSelection.GetCurrentParent()) ||
                               selItem is ToolStripSeparator || selItem is ToolStripControlHost || !selItem.IsOnDropDown || selItem.IsOnOverflow)
                            {
                                // finally Invalidate the selection rect ...
                                designer = designerHost.GetDesigner(selItem) as ToolStripItemDesigner;
                                if (designer != null)
                                {
                                    invalidateBounds = designer.GetGlyphBounds();
                                    GetAdjustedBounds(selItem, ref invalidateBounds);
                                    invalidateBounds.Inflate(GLYPHBORDER, GLYPHBORDER);

                                    if (invalidateRegion is null)
                                    {
                                        invalidateRegion = new Region(invalidateBounds);
                                        invalidateBounds.Inflate(-GLYPHINSET, -GLYPHINSET);
                                        invalidateRegion.Exclude(invalidateBounds);
                                    }
                                    else
                                    {
                                        itemRegion = new Region(invalidateBounds);
                                        invalidateBounds.Inflate(-GLYPHINSET, -GLYPHINSET);
                                        itemRegion.Exclude(invalidateBounds);
                                        invalidateRegion.Union(itemRegion);
                                    }
                                }
                            }
                        }
                    }
                }

                if (invalidateRegion != null || templateNodeSelected || shiftPressed)
                {
                    BehaviorService behaviorService = (BehaviorService)provider.GetService(typeof(BehaviorService));
                    if (behaviorService != null)
                    {
                        if (invalidateRegion != null)
                        {
                            behaviorService.Invalidate(invalidateRegion);
                        }

                        // When a ToolStripItem is PrimarySelection, the glyph bounds are not invalidated  through the SelectionManager so we have to do this.
                        designer = designerHost.GetDesigner(nextSelection) as ToolStripItemDesigner;
                        if (designer != null)
                        {
                            invalidateBounds = designer.GetGlyphBounds();
                            GetAdjustedBounds(nextSelection, ref invalidateBounds);
                            invalidateBounds.Inflate(GLYPHBORDER, GLYPHBORDER);
                            invalidateRegion = new Region(invalidateBounds);

                            invalidateBounds.Inflate(-GLYPHINSET, -GLYPHINSET);
                            invalidateRegion.Exclude(invalidateBounds);
                            behaviorService.Invalidate(invalidateRegion);
                        }
                    }
                }
            }
            finally
            {
                if (invalidateRegion != null)
                {
                    invalidateRegion.Dispose();
                }

                if (itemRegion != null)
                {
                    itemRegion.Dispose();
                }
            }
        }

        /// <summary> represents cached information about the display</summary>
        internal static class DisplayInformation
        {
            private static bool s_highContrast;  //whether we are under hight contrast mode
            private static bool s_lowRes;  //whether we are under low resolution mode
            private static bool s_isTerminalServerSession;   //whether this application is run on a terminal server (remote desktop)
            private static bool s_highContrastSettingValid;   //indicates whether the high contrast setting is correct
            private static bool s_lowResSettingValid;  //indicates whether the low resolution setting is correct
            private static bool s_terminalSettingValid; //indicates whether the terminal server setting is correct
            private static short s_bitsPerPixel;
            private static bool s_dropShadowSettingValid;
            private static bool s_dropShadowEnabled;

            static DisplayInformation()
            {
                SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(UserPreferenceChanged);
                SystemEvents.DisplaySettingsChanged += new EventHandler(DisplaySettingChanged);
            }

            public static short BitsPerPixel
            {
                get
                {
                    if (s_bitsPerPixel == 0)
                    {
                        foreach (Screen s in Screen.AllScreens)
                        {
                            if (s_bitsPerPixel == 0)
                            {
                                s_bitsPerPixel = (short)s.BitsPerPixel;
                            }
                            else
                            {
                                s_bitsPerPixel = (short)Math.Min(s.BitsPerPixel, s_bitsPerPixel);
                            }
                        }
                    }
                    return s_bitsPerPixel;
                }
            }

            /// <summary>
            ///  Tests to see if the monitor is in low resolution mode (8-bit color depth or less).
            /// </summary>
            public static bool LowResolution
            {
                get
                {
                    if (s_lowResSettingValid)
                    {
                        return s_lowRes;
                    }
                    s_lowRes = BitsPerPixel <= 8;
                    s_lowResSettingValid = true;
                    return s_lowRes;
                }
            }

            /// <summary>
            ///  Tests to see if we are under high contrast mode
            /// </summary>
            public static bool HighContrast
            {
                get
                {
                    if (s_highContrastSettingValid)
                    {
                        return s_highContrast;
                    }
                    s_highContrast = SystemInformation.HighContrast;
                    s_highContrastSettingValid = true;
                    return s_highContrast;
                }
            }
            public static bool IsDropShadowEnabled
            {
                get
                {
                    if (s_dropShadowSettingValid)
                    {
                        return s_dropShadowEnabled;
                    }
                    s_dropShadowEnabled = SystemInformation.IsDropShadowEnabled;
                    s_dropShadowSettingValid = true;
                    return s_dropShadowEnabled;
                }
            }

            /// <summary>
            ///test to see if we are under terminal server mode
            /// </summary>
            public static bool TerminalServer
            {
                get
                {
                    if (s_terminalSettingValid)
                    {
                        return s_isTerminalServerSession;
                    }
                    s_isTerminalServerSession = SystemInformation.TerminalServerSession;
                    s_terminalSettingValid = true;
                    return s_isTerminalServerSession;
                }
            }

            /// <summary>
            ///event handler for change in display setting
            /// </summary>
            private static void DisplaySettingChanged(object obj, EventArgs ea)
            {
                s_highContrastSettingValid = false;
                s_lowResSettingValid = false;
                s_terminalSettingValid = false;
                s_dropShadowSettingValid = false;
            }

            /// <summary>
            ///event handler for change in user preference
            /// </summary>
            private static void UserPreferenceChanged(object obj, UserPreferenceChangedEventArgs ea)
            {
                s_highContrastSettingValid = false;
                s_lowResSettingValid = false;
                s_terminalSettingValid = false;
                s_dropShadowSettingValid = false;
                s_bitsPerPixel = 0;
            }
        }
    }
}
