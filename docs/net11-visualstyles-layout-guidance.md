# .NET 11 VisualStyles layout guidance

`VisualStylesMode.Net11` modernizes framework-defined control chrome without adding a theming API. Colors and
metrics come from Windows state, including dark mode, accent color, High Contrast, DPI, focus metrics, and text
scale. High Contrast always selects classic, backward-compatible painting through
`EffectiveVisualStylesMode`.

Modern chrome can require more adornment space than classic chrome. When a naturally sized control opts in, the
control grows so its client area remains usable. This is intentional: glyphs and focus indicators designed for
96 DPI are not sufficient at modern display densities, and controls must also react to system text-scale changes.

## Choose an adaptive container

Use `TableLayoutPanel` or `FlowLayoutPanel` for controls whose preferred size can change. These containers already
handle localization, font changes, DPI changes, and AutoSize content, so they also handle modern visual-style
metrics.

Controls with intrinsic sizing, such as `Button`, single-line `TextBox`, `ComboBox`, and `Label`, update their
preferred size when their effective renderer changes. Controls with `AutoSize` enabled request layout
automatically.

### Fixed-bounds exceptions

`RichTextBox` and multiline `TextBox` do not have intrinsic single-line or `IntegralHeight` behavior. Switching
their visual style does not resize their bounds. If the modern non-client frame needs more room, their client area
shrinks slightly within the existing bounds.

That behavior is deliberate. Size and align fixed-bounds editors programmatically for the surrounding workflow
instead of relying on a global pixel-perfect height.

## Anchor when consuming size; Dock when pushing size

Use `Anchor` when a child consumes or aligns to space determined by its container. Use `Dock` when the child's
content should push the size of its container.

Inside an AutoSize `TableLayoutPanel`, anchor a control left and right when it should stretch to a column width
that the table computes:

```csharp
TextBox customerName = new()
{
    Anchor = AnchorStyles.Left | AnchorStyles.Right,
    AutoSize = true
};

tableLayoutPanel.Controls.Add(customerName, column: 1, row: 0);
```

Inside a constituent UserControl whose height should be driven by a single child, dock that child:

```csharp
public sealed class SearchEditor : UserControl
{
    private readonly TextBox _queryTextBox = new()
    {
        Dock = DockStyle.Top,
        AutoSize = true
    };

    public SearchEditor()
    {
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Controls.Add(_queryTextBox);
    }
}
```

Do not use `DockStyle.Fill` as a substitute for AutoSize. Fill consumes the bounds already assigned by the parent;
it does not tell an AutoSize parent what preferred height the child needs.

## Equal-width OK and Cancel buttons

An AutoSize table with two 50 percent columns guarantees equal button widths. Each button is AutoSize and anchored
left and right, so both columns take the width required by the wider preferred button.

```csharp
_btnOK = new Button();
_btnCancel = new Button();
_tlpDialogResultButtons = new TableLayoutPanel();
_tlpDialogResultButtons.SuspendLayout();
SuspendLayout();
//
// _btnOK
//
_btnOK.Anchor = AnchorStyles.Left | AnchorStyles.Right;
_btnOK.AutoSize = true;
_btnOK.AutoSizeMode = AutoSizeMode.GrowAndShrink;
_btnOK.DialogResult = DialogResult.OK;
_btnOK.Name = "_btnOK";
_btnOK.Padding = new Padding(14, 0, 14, 0);
_btnOK.TabIndex = 0;
_btnOK.Text = "OK";
_btnOK.UseVisualStyleBackColor = true;
//
// _btnCancel
//
_btnCancel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
_btnCancel.AutoSize = true;
_btnCancel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
_btnCancel.DialogResult = DialogResult.Cancel;
_btnCancel.Name = "_btnCancel";
_btnCancel.Padding = new Padding(14, 0, 14, 0);
_btnCancel.TabIndex = 1;
_btnCancel.Text = "Cancel";
_btnCancel.UseVisualStyleBackColor = true;
//
// _tlpDialogResultButtons
//
_tlpDialogResultButtons.AutoSize = true;
_tlpDialogResultButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
_tlpDialogResultButtons.ColumnCount = 2;
_tlpDialogResultButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
_tlpDialogResultButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
_tlpDialogResultButtons.Controls.Add(_btnCancel, 1, 0);
_tlpDialogResultButtons.Controls.Add(_btnOK, 0, 0);
_tlpDialogResultButtons.Name = "_tlpDialogResultButtons";
_tlpDialogResultButtons.RowCount = 1;
_tlpDialogResultButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
_tlpDialogResultButtons.TabIndex = 3;
```

The pattern remains stable when localization, DPI, text scale, or a VisualStylesMode transition changes either
button's preferred width or height.

## Align TextBox and ComboBox rows

Modern non-`Simple` ComboBox fields share their height metrics with an AutoSize, single-line modern TextBox using
`BorderStyle.Fixed3D`. ComboBox adds one logical pixel of internal style inset on top of its public Padding; apply
the same Padding to the TextBox when exact field-height parity is required. Put both in an AutoSize row, anchor
them left and right, and avoid hard-coded heights:

```csharp
TableLayoutPanel editorTable = new()
{
    AutoSize = true,
    AutoSizeMode = AutoSizeMode.GrowAndShrink,
    ColumnCount = 2,
    RowCount = 2
};
editorTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
editorTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
editorTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
editorTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));

Label nameLabel = new()
{
    Anchor = AnchorStyles.Left,
    AutoSize = true,
    Text = "Name"
};
TextBox nameEditor = new()
{
    Anchor = AnchorStyles.Left | AnchorStyles.Right,
    AutoSize = true,
    Padding = new Padding(1)
};

Label categoryLabel = new()
{
    Anchor = AnchorStyles.Left,
    AutoSize = true,
    Text = "Category"
};
ComboBox categoryEditor = new()
{
    Anchor = AnchorStyles.Left | AnchorStyles.Right,
    DropDownStyle = ComboBoxStyle.DropDownList
};

editorTable.Controls.Add(nameLabel, 0, 0);
editorTable.Controls.Add(nameEditor, 1, 0);
editorTable.Controls.Add(categoryLabel, 0, 1);
editorTable.Controls.Add(categoryEditor, 1, 1);
```

Do not derive a ComboBox height from `ItemHeight`. Item height controls list content; the modern selection field
uses TextBox-compatible padding and focus metrics. ComboBox Padding is designer-visible in .NET 11 and is
serialized only when it differs from `Padding.Empty`.

## Account for GroupBox DisplayRectangle

In modern `FlatStyle.Standard`, the GroupBox caption sits above a borderless rectangular surface. Its
`DisplayRectangle` starts lower than the classic etched-frame rectangle and reserves more content space above
than below. The caption aligns with the control Padding. Docked and anchored children are relaid out against the
new rectangle.

Use the GroupBox as a real layout container:

```csharp
GroupBox addressGroup = new()
{
    AutoSize = true,
    AutoSizeMode = AutoSizeMode.GrowAndShrink,
    Dock = DockStyle.Top,
    FlatStyle = FlatStyle.Standard,
    Text = "Address"
};

TableLayoutPanel addressTable = new()
{
    AutoSize = true,
    AutoSizeMode = AutoSizeMode.GrowAndShrink,
    Dock = DockStyle.Top
};

addressGroup.Controls.Add(addressTable);
```

Avoid positioning children with a fixed Y offset derived from `Font.Height`. The renderer owns caption metrics,
and `DisplayRectangle` is the supported content boundary.

The modern caption is derived from the ambient font at paint time. Standard and Popup enlarge the caption; Flat
keeps the ambient size. If the ambient font is regular and a matching installed Semibold family exists, WinForms
uses that real face. Otherwise the ambient weight is preserved; already styled fonts are never promoted.

## React to live text-scale changes

Place controls in AutoSize rows and let the layout engine react to the system event:

```csharp
TableLayoutPanel settingsTable = new()
{
    AutoSize = true,
    AutoSizeMode = AutoSizeMode.GrowAndShrink,
    ColumnCount = 1,
    RowCount = 2
};
settingsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
settingsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));

GroupBox optionsGroup = new()
{
    AutoSize = true,
    AutoSizeMode = AutoSizeMode.GrowAndShrink,
    Text = "Options"
};
ComboBox optionSelector = new()
{
    Anchor = AnchorStyles.Left | AnchorStyles.Right,
    DropDownStyle = ComboBoxStyle.DropDownList
};

settingsTable.Controls.Add(optionsGroup, 0, 0);
settingsTable.Controls.Add(optionSelector, 0, 1);
```

Do not recreate controls when text scale changes. WinForms clears preferred-size caches, updates native selection
field metrics where necessary, and requests layout while preserving handles and selection.

## Migration tooling

Automated migration tools can replace fixed pixel layouts with adaptive containers, identify fixed-bounds editor
exceptions, and review accessibility and text-scale behavior. Planned WinForms agent skills will cover High DPI
layout, accessibility, text-scale-adaptive layouts, and pixel-perfect-to-Fluent refactoring. Treat generated
changes as a starting point and verify each workflow at the application's supported DPI, language, and
accessibility settings.
