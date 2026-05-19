---
name: control-api-tests
description: >-
  Instructions for writing unit tests for new public APIs on WinForms controls
  and components. Covers test project structure, naming conventions, property
  tests, event tests, OnXxx method tests, SubControl patterns, data attributes,
  and handle-state verification.
metadata:
  author: dotnet-winforms
  version: "1.0"
---

# Writing Unit Tests for WinForms Control APIs

These rules apply when **writing unit tests for new public properties, methods,
events, and virtual methods** on WinForms controls or components. For the API
implementation itself, see the `new-control-api` skill.

> **Golden rule:** Every new public API member needs tests that verify default
> values, get/set round-trips, event firing, event idempotency, and behavior
> both with and without a native window handle.

---

## 1. Test Project Structure

### 1.1 Test location

Control tests live under:

```
src\test\unit\System.Windows.Forms\System\Windows\Forms\
```

The test project file is:

```
src\test\unit\System.Windows.Forms\System.Windows.Forms.Tests.csproj
```

### 1.2 Test file naming

Each control has its own test file (or set of partial files):

| Control | Test file(s) |
|---|---|
| `Button` | `ButtonTests.cs` |
| `ButtonBase` | `ButtonBaseTests.cs` |
| `Control` | `ControlTests.cs`, `ControlTests.Handlers.cs` |
| `Form` | `FormTests.cs` |
| `TextBox` | `TextBoxTests.cs` |

When adding new API tests, add them to the **existing test file** for that
control. If the file is already very large, use a new partial file named
`{Control}Tests.{Feature}.cs`.

### 1.3 Test framework

The project uses **xUnit** with **FluentAssertions**. Key attributes:

| Attribute | Purpose |
|---|---|
| `[WinFormsFact]` | Single test case (STA-thread-aware `[Fact]`) |
| `[WinFormsTheory]` | Parameterized test (STA-thread-aware `[Theory]`) |

These are custom xUnit attributes that ensure tests run on an STA thread,
which WinForms requires for COM interop and UI operations.

---

## 2. Test Method Naming

Follow the pattern:

```
{ControlName}_{MemberName}_{Scenario}
```

Examples:

```csharp
Button_DialogResult_Set_GetReturnsExpected
Control_OnAutoSizeChanged_Invoke_CallsAutoSizeChanged
ButtonBase_Command_SetWithHandler_CallsCommandChanged
Control_DataContext_AmbientBehaviorTest
```

---

## 3. Control Creation and Disposal

**Always** use `using` declarations to ensure controls are properly disposed,
releasing native window handles and GDI resources:

```csharp
[WinFormsFact]
public void Button_MyProperty_DefaultValue()
{
    using Button control = new();
    Assert.Equal(expectedDefault, control.MyProperty);
}
```

---

## 4. The SubControl Pattern

Protected members (`OnXxx` methods, protected properties) cannot be called
directly in tests. Create a **private nested subclass** inside the test class
that exposes them using `new`:

```csharp
private class SubButton : Button
{
    // Expose protected virtual methods for direct invocation
    public new void OnMyPropertyChanged(EventArgs e)
        => base.OnMyPropertyChanged(e);

    // Expose protected properties
    public new bool CanEnableIme => base.CanEnableIme;
}
```

**Rules:**

* The subclass is `private` and nested inside the test class.
* Use `public new` to re-expose `protected` base members.
* Name it `Sub{ControlName}` (e.g., `SubButton`, `SubControl`).
* For access to `private` members, use `TestAccessor`:
  `this.TestAccessor.Dynamic.PrivateMethod()`.

---

## 5. Test Data Attributes

Use built-in test data attributes to avoid hand-coding value sets:

| Attribute | Generates |
|---|---|
| `[BoolData]` | `true`, `false` |
| `[EnumData<TEnum>]` | All values of the enum |
| `[NewAndDefaultData<EventArgs>]` | `new EventArgs()`, `EventArgs.Empty` |
| `[InlineData(...)]` | Explicit inline values |
| `[MemberData(nameof(...))]` | Values from a static property/method |

```csharp
[WinFormsTheory]
[EnumData<DialogResult>]
public void Button_DialogResult_Set_GetReturnsExpected(DialogResult value)
{
    using Button control = new() { DialogResult = value };
    Assert.Equal(value, control.DialogResult);
}
```

---

## 6. Required Test Categories

For every new public property, provide tests in these categories:

### 6.1 Default value test

Verify the property returns its expected default immediately after
construction — before any handle is created:

```csharp
[WinFormsFact]
public void MyControl_MyProperty_DefaultValue()
{
    using MyControl control = new();
    Assert.Equal(expectedDefault, control.MyProperty);
    Assert.False(control.IsHandleCreated);
}
```

### 6.2 Property set/get round-trip — without handle

```csharp
[WinFormsTheory]
[InlineData(1)]
[InlineData(42)]
public void MyControl_MyProperty_Set_GetReturnsExpected(int value)
{
    using MyControl control = new() { MyProperty = value };
    Assert.Equal(value, control.MyProperty);
    Assert.False(control.IsHandleCreated);

    // Set same value again — must be idempotent.
    control.MyProperty = value;
    Assert.Equal(value, control.MyProperty);
    Assert.False(control.IsHandleCreated);
}
```

### 6.3 Property set/get round-trip — with handle

Force handle creation and verify no unexpected side-effect events:

```csharp
[WinFormsTheory]
[InlineData(1)]
[InlineData(42)]
public void MyControl_MyProperty_SetWithHandle_GetReturnsExpected(int value)
{
    using MyControl control = new();
    Assert.NotEqual(IntPtr.Zero, control.Handle);

    int invalidatedCallCount = 0;
    control.Invalidated += (sender, e) => invalidatedCallCount++;
    int styleChangedCallCount = 0;
    control.StyleChanged += (sender, e) => styleChangedCallCount++;
    int createdCallCount = 0;
    control.HandleCreated += (sender, e) => createdCallCount++;

    control.MyProperty = value;
    Assert.Equal(value, control.MyProperty);
    Assert.True(control.IsHandleCreated);
    Assert.Equal(0, invalidatedCallCount);
    Assert.Equal(0, styleChangedCallCount);
    Assert.Equal(0, createdCallCount);
}
```

### 6.4 Event firing test

Verify the `[Property]Changed` event fires when the value changes, does
**not** fire when the same value is set, and does not fire after the handler
is removed:

```csharp
[WinFormsFact]
public void MyControl_MyProperty_SetWithHandler_CallsMyPropertyChanged()
{
    using MyControl control = new();
    int callCount = 0;
    EventHandler handler = (sender, e) =>
    {
        Assert.Same(control, sender);
        Assert.Same(EventArgs.Empty, e);
        callCount++;
    };

    control.MyPropertyChanged += handler;

    // Set different value — event fires.
    control.MyProperty = newValue1;
    Assert.Equal(newValue1, control.MyProperty);
    Assert.Equal(1, callCount);

    // Set same value — event does NOT fire.
    control.MyProperty = newValue1;
    Assert.Equal(1, callCount);

    // Set another different value — event fires again.
    control.MyProperty = newValue2;
    Assert.Equal(2, callCount);

    // Remove handler — event no longer fires.
    control.MyPropertyChanged -= handler;
    control.MyProperty = newValue1;
    Assert.Equal(2, callCount);
}
```

### 6.5 OnXxx virtual method test

Test the `On[Property]Changed` method directly via the SubControl, verifying
it raises the event and can be unsubscribed:

```csharp
[WinFormsTheory]
[NewAndDefaultData<EventArgs>]
public void MyControl_OnMyPropertyChanged_Invoke_CallsMyPropertyChanged(EventArgs eventArgs)
{
    using SubMyControl control = new();
    int callCount = 0;
    EventHandler handler = (sender, e) =>
    {
        Assert.Same(control, sender);
        Assert.Same(eventArgs, e);
        callCount++;
    };

    // Call with handler subscribed.
    control.MyPropertyChanged += handler;
    control.OnMyPropertyChanged(eventArgs);
    Assert.Equal(1, callCount);

    // Remove handler — still callable, but handler not invoked.
    control.MyPropertyChanged -= handler;
    control.OnMyPropertyChanged(eventArgs);
    Assert.Equal(1, callCount);
}
```

### 6.6 OnXxx virtual method test — with handle

If the `On` method triggers visual changes (invalidation, style changes),
test with a handle:

```csharp
[WinFormsTheory]
[NewAndDefaultData<EventArgs>]
public void MyControl_OnMyPropertyChanged_InvokeWithHandle_CallsMyPropertyChanged(EventArgs eventArgs)
{
    using SubMyControl control = new();
    Assert.NotEqual(IntPtr.Zero, control.Handle);

    int invalidatedCallCount = 0;
    control.Invalidated += (sender, e) => invalidatedCallCount++;

    int callCount = 0;
    EventHandler handler = (sender, e) =>
    {
        Assert.Same(control, sender);
        Assert.Same(eventArgs, e);
        callCount++;
    };

    control.MyPropertyChanged += handler;
    control.OnMyPropertyChanged(eventArgs);
    Assert.Equal(1, callCount);
    Assert.True(control.IsHandleCreated);
    // Adjust expected counts based on whether the property triggers Invalidate().
}
```

---

## 7. Testing Custom EventArgs and Delegates

When the API introduces a dedicated `EventArgs` subclass and delegate:

```csharp
[WinFormsFact]
public void MyControl_OnMyAction_Invoke_CallsMyAction()
{
    using SubMyControl control = new();
    MyActionEventArgs expectedArgs = new("test detail");
    int callCount = 0;

    MyActionEventHandler handler = (sender, e) =>
    {
        Assert.Same(control, sender);
        Assert.Same(expectedArgs, e);
        Assert.Equal("test detail", e.Detail);
        callCount++;
    };

    control.MyAction += handler;
    control.OnMyAction(expectedArgs);
    Assert.Equal(1, callCount);
}
```

---

## 8. Testing Command Binding (ICommand)

If the new API involves `ICommand` binding, test the full lifecycle:

```csharp
[WinFormsFact]
public void MyControl_BasicCommandBinding()
{
    using SubMyControl control = new();
    CommandViewModel viewModel = new() { TestCommandExecutionAbility = true };

    int callCount = 0;
    EventHandler handler = (sender, e) =>
    {
        Assert.Same(control, sender);
        Assert.Same(EventArgs.Empty, e);
        callCount++;
    };

    // Bind command.
    control.CommandChanged += handler;
    control.Command = viewModel.TestCommand;
    Assert.Equal(1, callCount);

    // Set parameter.
    control.CommandParameterChanged += handler;
    control.CommandParameter = "TestParam";
    Assert.Equal(2, callCount);

    // Execute.
    control.OnClick(EventArgs.Empty);
    Assert.Equal("TestParam", viewModel.CommandExecuteResult);

    // Disable command.
    viewModel.TestCommandExecutionAbility = false;
    Assert.False(control.Enabled);
}
```

---

## 9. Checklist for New API Tests

* [ ] Default value test (constructor, no handle)
* [ ] Property set/get round-trip (without handle)
* [ ] Property set/get round-trip (with handle, verify side-effect events)
* [ ] Event firing on value change
* [ ] Event idempotency (same value → no event)
* [ ] Event handler removal (unsubscribe → no event)
* [ ] OnXxx virtual method direct invocation (via SubControl)
* [ ] OnXxx with handle (if it triggers invalidation)
* [ ] Custom EventArgs property assertions (if applicable)
* [ ] `using` declarations for all control instances
* [ ] `Assert.False(control.IsHandleCreated)` where handle should not be forced
