# Manual Testing Guide for TabControl Dark Mode Fix

## Issue
TabControl dark mode works for Top/Bottom alignment but NOT for Left/Right alignment in .NET 10.0.

## Fix Applied
Modified `ApplyDarkModeOnDemand()` in `TabControl.cs` to apply different themes based on alignment:
- **Vertical tabs (Left/Right)**: `DarkMode_Explorer` theme
- **Horizontal tabs (Top/Bottom)**: `DarkMode::FileExplorerBannerContainer` theme

## How to Test

### Prerequisites
1. Windows machine with Visual Studio 2022
2. .NET 10.0 SDK installed

### Steps

1. **Build the solution**:
   ```bash
   cd /path/to/winforms
   ./build.cmd
   ```

2. **Run the WinformsControlsTest application**:
   ```bash
   cd src/test/integration/WinformsControlsTest
   dotnet run
   ```

3. **Open the TabControl test form**:
   - From the main menu, you'll need to add a button to launch `TabControlTests` form
   - Alternatively, modify `Program.cs` to launch `TabControlTests` directly

4. **Enable Dark Mode**:
   - Modify `Program.cs` to add:
     ```csharp
     Application.SetColorMode(SystemColorMode.Dark);
     ```
   - This should be added before `Application.Run(...)`

5. **Verify the fix**:
   - The test form displays 4 TabControls with different alignments:
     - Top-left: Alignment = Top
     - Top-right: Alignment = Bottom
     - Bottom-left: Alignment = Left
     - Bottom-right: Alignment = Right
   
   - **Expected Result**: All 4 TabControls should render in dark mode with dark backgrounds on tabs
   - **Before Fix**: Only Top and Bottom alignment tabs would be dark; Left and Right would remain white
   - **After Fix**: All alignments should display properly in dark mode

### Alternative Quick Test

Create a minimal test app:

```csharp
using System.Windows.Forms;

Application.SetHighDpiMode(HighDpiMode.SystemAware);
Application.SetColorMode(SystemColorMode.Dark);

Form form = new Form
{
    Text = "TabControl Dark Mode Test",
    Width = 650,
    Height = 600
};

// Test all alignments
TabControl[] tabControls = new TabControl[4];
TabAlignment[] alignments = { TabAlignment.Top, TabAlignment.Bottom, TabAlignment.Left, TabAlignment.Right };
string[] labels = { "Top", "Bottom", "Left", "Right" };

for (int i = 0; i < 4; i++)
{
    int row = i / 2;
    int col = i % 2;
    
    Label label = new Label
    {
        Text = $"Alignment = {labels[i]}",
        Location = new Point(12 + col * 318, 12 + row * 280),
        AutoSize = true
    };
    form.Controls.Add(label);
    
    tabControls[i] = new TabControl
    {
        Location = new Point(12 + col * 318, 35 + row * 280),
        Size = new Size(300, 250),
        Alignment = alignments[i]
    };
    
    for (int j = 0; j < 3; j++)
    {
        tabControls[i].TabPages.Add(new TabPage { Text = $"Tab {j + 1}" });
    }
    
    form.Controls.Add(tabControls[i]);
}

Application.Run(form);
```

Save as `TestDarkMode.cs` and run with:
```bash
csc /r:System.Windows.Forms.dll TestDarkMode.cs
TestDarkMode.exe
```

## Expected Screenshots

### Before Fix
- Top and Bottom: Dark mode applied (dark tabs)
- Left and Right: Light mode (white tabs) - **BUG**

### After Fix
- All alignments: Dark mode applied consistently (dark tabs)

## Notes
- This fix addresses the issue where vertical tab controls (Left/Right alignment with TCS_VERTICAL style) were not receiving the correct dark mode theme
- The FileExplorerBannerContainer theme works for horizontal tabs but not vertical
- The Explorer theme is more appropriate for vertical tab layouts
