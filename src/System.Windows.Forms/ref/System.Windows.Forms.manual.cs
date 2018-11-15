// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Windows.Forms
{
    [System.ComponentModel.TypeConverterAttribute(typeof(DataGridViewCellConverter))]
    public partial class DataGridViewCell { }
    internal class DataGridViewCellConverter { }
    
    [System.ComponentModel.TypeConverterAttribute(typeof(DataGridViewColumnConverter))]
    public partial class DataGridViewColumn { }
    internal class DataGridViewColumnConverter { }
    
    [System.ComponentModel.TypeConverterAttribute(typeof(DataGridViewRowConverter))]
    public partial class DataGridViewRow { }
    internal class DataGridViewRowConverter { }
    
    [System.ComponentModel.TypeConverterAttribute(typeof(FlatButtonAppearanceConverter))]
    public partial class FlatButtonAppearance { }
    internal class FlatButtonAppearanceConverter { }
    
    [System.ComponentModel.TypeConverterAttribute(typeof(ImageListConverter))]
    public partial class ImageList { }
    internal class ImageListConverter { }

    [System.ComponentModel.TypeConverterAttribute(typeof(ListViewGroupConverter))]
    public partial class ListViewGroup { }
    internal class ListViewGroupConverter { }
    
    public partial class ListViewItem
    {
        [System.ComponentModel.TypeConverterAttribute(typeof(ListViewSubItemConverter))]
        public partial class ListViewSubItem { }
    }
    internal class ListViewSubItemConverter { }
    
    [System.ComponentModel.TypeConverterAttribute(typeof(TableLayoutPanelCellPositionTypeConverter))]
    public partial struct TableLayoutPanelCellPosition { }
    internal class TableLayoutPanelCellPositionTypeConverter { }
    
    [System.ComponentModel.TypeConverterAttribute(typeof(TableLayoutSettings.StyleConverter))]
    public partial class TableLayoutStyle { }
    public partial class TableLayoutSettings
    {
        internal class StyleConverter { }
    }
}
