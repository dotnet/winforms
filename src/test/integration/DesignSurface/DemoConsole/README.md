# DesignSurface (DemoConsole)

- [Debugging Native Code in This Project](#debugging-native-code-in-this-project)
  - [1. Enable Native Code Debugging](#1.-enable-native-code-debugging)
  - [2. Configure Symbol Server for Native Debugging (Symweb)](<#2.-configure-symbol-server-for-native-debugging-(symweb)>)
- [Running the Project Against .NET Framework](#running-the-project-against-.net-framework)

## Debugging Native Code in This Project

To fully enable debugging in this project (including native code and symbol resolution), perform the following two steps:

### 1. Enable Native Code Debugging

This allows Visual Studio to step into native code

**Option A – Using Visual Studio GUI:**

1. Right-click the project in Solution Explorer and select **Properties**.
2. Go to the **Debug** then **Open debug launch profiles UI**.
3. Check the box **Enable native code debugging**.
4. Save and rebuild the project.

> This setting is saved in a user-specific `.csproj.user` file.

**Option B – Editing `launchSettings.json`:**

1. Open the file `Properties\launchSettings.json`.
2. Under the `"profiles"` section, locate the profile matching your project name.
3. Add or update the property `"nativeDebugging": true`.

Example:

```json
{
  "profiles": {
    "YourProjectName": {
      "commandName": "Project",
      "nativeDebugging": true
    }
  }
}
```

---

### 2. Configure Symbol Server for Native Debugging (Symweb)

To resolve native symbols when stepping through native code, configure the Symweb server:

1. Go to **Debug > Options** from the Visual Studio toolbar.
2. In the left pane, select **Debugging > Symbols**.
3. Under **Symbol file (.pdb) locations**, click the **➕** (plus) button.
4. Enter the following URL:

```
https://symweb
```

5. Press **Enter** or click **OK** to save.

---

After completing both steps above, Visual Studio will be able to debug both managed and native parts of the application, using symbols downloaded from Symweb as needed.

## Running the Project Against .NET Framework

This project targets multiple frameworks, including .NET Framework 4.8.1. To run or debug it against the .NET Framework instead of .NET (e.g., net10.0-windows), follow the steps below:

1. In Visual Studio, locate the Debug dropdown on the top toolbar (next to the green **Start** button).
2. Click the small downward arrow next to the active configuration. You will see the currently selected framework (typically something like `Framework (net10.0-windows)`).
3. Click the displayed framework name to open a submenu showing all target frameworks for this project.
4. Select **net481** from the list.
5. Build and run the project using any of the usual commands:
   - **Build**: `Ctrl + Shift + B`
   - **Start Without Debugging**: `Ctrl + F5`
   - **Start With Debugging**: `F5`

This configuration change ensures that Visual Studio builds and runs the project using the .NET Framework target.

