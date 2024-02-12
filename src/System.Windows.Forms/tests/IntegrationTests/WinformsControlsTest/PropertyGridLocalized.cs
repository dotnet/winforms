// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;

namespace WinformsControlsTest;

public partial class PropertyGridLocalized : Form
{
    private CultureInfo _originalCulture;

    // https://github.com/dotnet/runtime/issues/94365
    public PropertyGridLocalized()
    {
        InitializeComponent();
        _originalCulture = CultureInfo.CurrentUICulture;
        propertyGrid1.SelectedObject = new MyObjectClass();
        cultureComboBox.DisplayMember = "EnglishName";
        cultureComboBox.ValueMember = "Name";
        LoadCultures();
        cultureComboBox.SelectedItem = CultureInfo.CurrentUICulture;
        cultureComboBox.SelectedIndexChanged += CultureComboBox_SelectedIndexChanged;
    }

    private void CultureComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cultureComboBox.SelectedItem is CultureInfo selectedCulture && selectedCulture != CultureInfo.CurrentUICulture)
        {
            CultureInfo.CurrentUICulture = selectedCulture;
            CultureInfo.CurrentCulture = selectedCulture;
            propertyGrid1.SelectedObject = null;
            propertyGrid1.SelectedObject = new MyObjectClass();
            propertyGrid1.Refresh();
        }
    }

    private void LoadCultures()
    {
        foreach (InputLanguage language in InputLanguage.InstalledInputLanguages)
        {
            cultureComboBox.Items.Add(language.Culture);
        }
    }

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components is not null))
        {
            components.Dispose();
        }

        // reset the culture back to what is was when form opened.
        CultureInfo.CurrentUICulture = _originalCulture;
        CultureInfo.CurrentCulture = _originalCulture;

        base.Dispose(disposing);
    }

    private class MyObjectClass : UserControl
    {
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string MiscValue { get; set; }
    }
}
