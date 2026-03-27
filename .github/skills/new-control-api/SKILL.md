---
name: new-control-api
description: >-
  Instructions for adding new public APIs (properties, methods, events,
  delegates) to existing WinForms controls or components. Covers API issue
  tracking, PublicAPI file maintenance, property/event conventions, CodeDOM
  serialization, design-time attributes, and XML documentation.
metadata:
  author: dotnet-winforms
  version: "1.0"
---

# Adding New APIs to an Existing WinForms Control or Component

These rules apply when **adding new public or protected members** — properties,
methods, events, delegates, enums, or interfaces — to an existing WinForms
control or component. For creating entirely new controls, or for general coding
standards, see the `coding-standards` skill instead.

> **Golden rule:** Every new public API must be tracked, reviewed, documented,
> and serialization-safe before it ships.

---

## 1. API Issue Tracking

Every new public API surface change requires a corresponding **API proposal
issue** in the `api-suggestion` format, which will be reviewed by the .NET API
review board.

### 1.1 Check for an existing issue

Before creating anything, search the **upstream** repository
(`dotnet/winforms`) for an existing issue with the `api-suggestion` label that
covers the planned API change.

### 1.2 Environment-aware issue management

* **Copilot CLI / Copilot Web-UI (CCA):** You have permission to interact with
  the GitHub API. Follow the rules below to check, create, or update issues.
* **VS Code / Visual Studio:** Do **not** attempt to create or modify issues
  yourself — the necessary permissions may not be available. Instead, remind
  the user to verify and manage the API issue manually.

### 1.3 Creating or updating the API issue

> ⚠️ **NEVER create a new issue in the upstream repo** (`dotnet/winforms`).

1. **If the issue exists upstream and you can modify it** (you are acting on
   behalf of a WinForms team member with sufficient permissions), update it
   in-place.
2. **If you cannot modify the upstream issue**, check whether a corresponding
   issue exists in the **origin** fork. If yes, update it there. If no, create
   it there.
3. **If no issue exists at all**, create it in the **origin** fork using the
   format described below.

### 1.4 Required issue content

The API issue **must** contain all of the following sections. If any information
is missing, **stop and ask the user** before proceeding with implementation.

**Section 1 — Background and motivation:**
Why is this API needed? What scenario does it enable?

**Section 2 — API Proposal** (C# code block with full public signature, no
method bodies):

```csharp
namespace System.Windows.Forms;

public partial class ExistingControl
{
    public Color NewProperty { get; set; }
    public event EventHandler? NewPropertyChanged;
    protected virtual void OnNewPropertyChanged(EventArgs e);
}
```

**Section 3 — API Usage** (C# code block showing consumption):

```csharp
var control = new ExistingControl();
control.NewProperty = Color.Red;
control.NewPropertyChanged += (s, e) => Console.WriteLine("Changed!");
```

**Section 4 — Alternative Designs:** Other approaches considered.

**Section 5 — Risks:** Breaking changes, perf regressions, etc.

**Section 6 — Will this feature affect UI controls?** Designer support,
accessibility impact, localization needs.

---

## 2. PublicAPI File Tracking

All new **public** and **protected** members must be recorded in the
PublicAPI text files so the Roslyn analyzer can enforce API compatibility.

### 2.1 During development — `PublicAPI.Unshipped.txt`

Add entries for every new API surface to the **Unshipped** file of the
project that contains the new code. For `System.Windows.Forms` controls this
is:

```
src\System.Windows.Forms\PublicAPI.Unshipped.txt
```

### 2.2 Before final release — `PublicAPI.Shipped.txt`

Before snapping for a release, entries are moved from `Unshipped` to
`Shipped`. During development, only touch `Unshipped`.

### 2.3 Entry format

Entries use the **Roslyn PublicAPI format** — one line per accessor, fully
qualified, sorted alphabetically. Key patterns:

```text
# Property (getter + setter on separate lines)
System.Windows.Forms.Control.DataContext.get -> object?
System.Windows.Forms.Control.DataContext.set -> void

# Virtual / override / abstract modifiers prefix the line
virtual System.Windows.Forms.Control.DataContext.get -> object?
virtual System.Windows.Forms.Control.DataContext.set -> void

# Events — handler type as return
System.Windows.Forms.Control.DataContextChanged -> System.EventHandler?

# Protected virtual On-methods
virtual System.Windows.Forms.Control.OnDataContextChanged(System.EventArgs! e) -> void

# Methods with parameters
System.Windows.Forms.Control.SomeMethod(int count, string! name) -> bool

# Constructors
System.Windows.Forms.MyComponent.MyComponent() -> void

# Enum members
System.Windows.Forms.MyEnum.Value1 = 0 -> System.Windows.Forms.MyEnum
System.Windows.Forms.MyEnum.Value2 = 1 -> System.Windows.Forms.MyEnum
```

**Nullable annotations:** `?` = nullable reference, `!` = non-nullable
reference. Value types do not carry these markers unless `Nullable<T>`.

### 2.4 Publicly accessible interfaces

If a new **public or protected interface** is introduced (or an existing one
gains new members), every member that is publicly accessible must also appear
in the PublicAPI file and be part of the API review scope.

---

## 3. New Property Conventions

### 3.1 Property backing storage — PropertyStore (mandatory)

WinForms controls **do not use regular backing fields** for public properties.
Instead, all property values are stored in the control's `Properties`
collection (a `PropertyStore` instance). This is critical for two reasons:

1. **Memory efficiency** — only properties that are explicitly set consume
   space. A form with hundreds of controls does not multiply per-property
   field overhead.
2. **Constructor ordering** — in an inherited control, the base class
   constructor runs first, and virtual methods like `CreateParams` are called
   **before** the derived class's constructor body executes. If a property
   getter relied on a backing field initialized in the derived constructor,
   it would read an uninitialized value. The PropertyStore avoids this because
   `GetValueOrDefault` safely returns a default when no value has been stored.

**Declare a static key** for each new property (one per property, shared
across all instances):

```csharp
private static readonly int s_myPropertyProperty = PropertyStore.CreateKey();
```

**Value-type property** (int, bool, Color, enum, struct):

```csharp
public Color MyProperty
{
    get => Properties.GetValueOrDefault(s_myPropertyProperty, Color.Empty);
    set
    {
        if (Properties.GetValueOrDefault(s_myPropertyProperty, Color.Empty) != value)
        {
            Properties.AddOrRemoveValue(s_myPropertyProperty, value, defaultValue: Color.Empty);
            OnMyPropertyChanged(EventArgs.Empty);
        }
    }
}
```

`AddOrRemoveValue` automatically removes the entry when the value equals the
default, keeping the store lean.

**Reference-type property** (object, string, Image):

```csharp
public Image? MyImage
{
    get => Properties.GetValueOrDefault<Image>(s_myImageProperty);
    set
    {
        if (Properties.GetValueOrDefault<Image>(s_myImageProperty) != value)
        {
            Properties.AddOrRemoveValue(s_myImageProperty, value);
            OnMyImageChanged(EventArgs.Empty);
        }
    }
}
```

### 3.2 CodeDOM serialization strategy — mandatory

Every new public property **must** have a serialization strategy so the
WinForms Designer can persist it correctly. Use **one** of these approaches:

| Approach | When to use |
|---|---|
| `[DefaultValue(...)]` | Simple value-type properties with a constant default |
| `[DesignerSerializationVisibility(Hidden)]` | Properties that must not be serialized (e.g., runtime-only, bound) |
| `ShouldSerialize` + `Reset` methods | Complex defaults, reference-type defaults, or ambient properties |

**ShouldSerialize / Reset pattern** (uses PropertyStore):

```csharp
// These methods MUST be private — the Designer finds them by convention.
private bool ShouldSerializeMyProperty()
    => Properties.ContainsKey(s_myPropertyProperty);

private void ResetMyProperty()
    => Properties.RemoveValue(s_myPropertyProperty);
```

### 3.3 Change notification — `On[Property]Changed` + event

Unless explicitly stated otherwise, every new public property requires:

1. A **`protected virtual void On[Property]Changed(EventArgs e)`** method.
2. A corresponding **`[Property]Changed`** event of type `EventHandler?`.

**Event delegate storage** also uses a static key / centralized collection —
see [Section 4.3](#43-event-delegate-storage) below.

```csharp
private static readonly object s_myPropertyChangedEvent = new();

/// <summary>
///  Occurs when the value of <see cref="MyProperty"/> changes.
/// </summary>
[SRCategory(nameof(SR.CatPropertyChanged))]
[SRDescription(nameof(SR.ControlOnMyPropertyChangedDescr))]
public event EventHandler? MyPropertyChanged
{
    add => Events.AddHandler(s_myPropertyChangedEvent, value);
    remove => Events.RemoveHandler(s_myPropertyChangedEvent, value);
}

/// <summary>
///  Raises the <see cref="MyPropertyChanged"/> event.
/// </summary>
/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
[EditorBrowsable(EditorBrowsableState.Advanced)]
protected virtual void OnMyPropertyChanged(EventArgs e)
{
    if (Events[s_myPropertyChangedEvent] is EventHandler handler)
    {
        handler(this, e);
    }
}
```

### 3.4 Design-time attributes

Decorate new properties with attributes to support the **Properties window**
and **IntelliSense**. Check existing properties on the same control for
precedent:

| Attribute | Purpose |
|---|---|
| `[SRCategory(nameof(SR.CatXxx))]` | Groups the property in the Properties window |
| `[SRDescription(nameof(SR.XxxDescr))]` | Tooltip in the Properties window |
| `[Browsable(true/false)]` | Show/hide in Properties window |
| `[Bindable(true)]` | Marks the property as data-bindable |
| `[EditorBrowsable(...)]` | Controls IntelliSense visibility |
| `[Localizable(true)]` | Marks the property value as localizable |

**Resource strings:** Category and description strings go in `SR.resx` with
localization-ready keys — follow the naming conventions already present (e.g.,
`CatAppearance`, `CatBehavior`, `CatData`, `ControlOnXxxDescr`).

### 3.5 Naming conventions

* Check the target control and its base classes for naming precedent.
* Property names should follow the same naming style as peer properties.
* If similar properties exist on related controls (e.g., WPF's
  `DataContext`), match the established name where it makes sense.

---

## 4. New Event Conventions

### 4.1 Standard events

Use the standard `EventHandler` delegate and `EventArgs.Empty` when no
additional data is needed:

```csharp
public event EventHandler? SomethingHappened;
```

### 4.2 Events with custom data — dedicated EventArgs + Delegate

When the event carries data beyond what `EventArgs` provides, **do not use
generics** (`EventHandler<T>`). Instead, create:

1. A **dedicated `EventArgs` subclass** (e.g., `MyActionEventArgs`).
2. A **dedicated delegate** (e.g., `MyActionEventHandler`).

```csharp
/// <summary>
///  Provides data for the <see cref="Control.MyAction"/> event.
/// </summary>
public class MyActionEventArgs : EventArgs
{
    public MyActionEventArgs(string detail) => Detail = detail;

    /// <summary>
    ///  Gets the detail information associated with this event.
    /// </summary>
    public string Detail { get; }
}

/// <summary>
///  Represents the method that will handle the <see cref="Control.MyAction"/> event.
/// </summary>
public delegate void MyActionEventHandler(object? sender, MyActionEventArgs e);
```

### 4.3 Event delegate storage

Just as properties use `PropertyStore`, event delegates use the inherited
`Events` collection (`EventHandlerList` from `Component`) with **static key
objects** — one object per event, shared across all instances. This avoids
allocating a delegate field per instance for events that are rarely subscribed.

**Declare a static key object** for each event:

```csharp
private static readonly object s_myActionEvent = new();
```

**Declare the event** using custom `add`/`remove` accessors:

```csharp
public event MyActionEventHandler? MyAction
{
    add => Events.AddHandler(s_myActionEvent, value);
    remove => Events.RemoveHandler(s_myActionEvent, value);
}
```

**Raise the event** in the `On` method by retrieving the delegate from the
collection:

```csharp
protected virtual void OnMyAction(MyActionEventArgs e)
{
    if (Events[s_myActionEvent] is MyActionEventHandler handler)
    {
        handler(this, e);
    }
}
```

> **Exception:** Components with only a single event (e.g., `Timer.Tick`) may
> use a regular field-backed delegate instead. For controls — which inherit
> dozens of events from `Control` — always use the `Events` collection.

---

## 5. New Method Conventions

### 5.1 Argument validation

Use throw helpers — never hand-roll null or range checks:

```csharp
ArgumentNullException.ThrowIfNull(parameter);
ArgumentOutOfRangeException.ThrowIfNegative(value);
```

### 5.2 Virtual methods

When adding a method that derived controls should be able to override, make
it `protected virtual`. Follow the naming conventions already in use on the
control's class hierarchy.

---

## 6. XML Documentation

Every new public or protected member **must** have XML documentation. This is
the basis for the official docs.

### 6.1 Minimum requirements

* **`<summary>`** — always present. Concise statement of what the member does.
* **`<param>`** — for every parameter, with a meaningful description.
* **`<returns>`** — for non-void methods.
* **`<exception>`** — for every exception the method can throw.
* **`<value>`** — for properties, when the summary alone is not sufficient.

### 6.2 Non-trivial members

For APIs that are not self-explanatory, also include:

* **`<remarks>`** with `<para>` blocks — design rationale, usage patterns,
  threading considerations, inheritance notes.
* **`<example>`** — code samples using `<code>` blocks.

```csharp
/// <summary>
///  Gets or sets the data context for data binding purposes.
///  This is an ambient property.
/// </summary>
/// <remarks>
///  <para>
///   The data context is inherited by child controls that do not have
///   their own <see cref="DataContext"/> set. When a parent's data
///   context changes, <see cref="OnParentDataContextChanged"/> is
///   called on each child.
///  </para>
/// </remarks>
/// <example>
///  <code>
///   var form = new MyForm();
///   form.DataContext = new MyViewModel();
///  </code>
/// </example>
public virtual object? DataContext { get; set; }
```

### 6.3 On-methods and events

```csharp
/// <summary>
///  Raises the <see cref="MyPropertyChanged"/> event.
/// </summary>
/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
protected virtual void OnMyPropertyChanged(EventArgs e)
```

### 6.4 Formatting

* Indent XML structure with **1 space** per nesting level.
* Use **Unicode characters**, not HTML entities.
* Use `<see cref="..."/>` for cross-references.
* Use `<inheritdoc/>` on overrides when the base documentation is sufficient.

---

## 7. .NET Version Guard — Mandatory

All new public APIs **must** be guarded with a preprocessor directive for the
target .NET version. Currently, new APIs target at least **.NET 11**:

```csharp
#if NET11_0_OR_GREATER
    /// <summary>
    ///  Gets or sets the corner radius for the control's border.
    /// </summary>
    public int CornerRadius
    {
        get => Properties.GetValueOrDefault(s_cornerRadiusProperty, 0);
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            if (Properties.GetValueOrDefault(s_cornerRadiusProperty, 0) != value)
            {
                Properties.AddOrRemoveValue(s_cornerRadiusProperty, value, defaultValue: 0);
                OnCornerRadiusChanged(EventArgs.Empty);
            }
        }
    }
#endif
```

> **Why?** Version guards ensure new APIs are only available on the .NET version
> they were approved for, preventing accidental use on older runtimes. The guard
> applies to the entire API surface: property, event, `On` method, and any
> associated types.

The matching tests must use the **same** preprocessor guard:

```csharp
#if NET11_0_OR_GREATER
    [WinFormsFact]
    public void MyControl_CornerRadius_Set_GetReturnsExpected()
    {
        using MyControl control = new() { CornerRadius = 5 };
        Assert.Equal(5, control.CornerRadius);
    }
#endif
```

---

## 8. Checklist Before Submitting

Before considering the implementation complete, verify:

* [ ] API proposal issue exists (upstream or fork) with full proposal format
* [ ] All new public/protected members are in `PublicAPI.Unshipped.txt`
* [ ] New APIs guarded with `#if NET11_0_OR_GREATER` (or appropriate version)
* [ ] Property values stored via `PropertyStore` (not backing fields)
* [ ] Every property has a CodeDOM serialization strategy
* [ ] Every property has `On[Property]Changed` + `[Property]Changed` event
      (unless explicitly exempted)
* [ ] Event delegates stored via `Events` collection (not field-backed)
* [ ] Design-time attributes are applied (`SRCategory`, `SRDescription`, etc.)
* [ ] Resource strings added to `SR.resx` as XML entries
* [ ] Events with custom data use dedicated `EventArgs` + `Delegate` (no generics)
* [ ] XML documentation on every new public/protected member
* [ ] Naming follows precedent on the control and its base classes
* [ ] Publicly accessible interface members are tracked in PublicAPI files
* [ ] Unit tests cover the new API surface (with matching version guard)

### 8.1 API issue checklist

The API proposal issue itself should contain the following checklist at the
bottom (to be maintained as part of the issue, not just at PR time):

```markdown
### Status Checklist
- [ ] API proposal has `api-suggestion` label
- [ ] Background, API Proposal, API Usage, and Risks sections are complete
- [ ] API shape has been discussed with the team
- [ ] Review the issue for compatibility with what the API review board expects
- [ ] Change label to `api-ready-for-review`
- [ ] If late in the release cycle, also add the `blocking` label to expedite
      the review appointment
- [ ] API review completed — label changed to `api-approved`
```

---

## 9. Resource Strings and Localization

All user-facing strings — categories, descriptions, exception messages — must
be added as resource entries, never hard-coded.

### 9.1 Adding entries to `SR.resx`

Add new entries as XML to the `SR.resx` file in the project. Follow the
existing naming conventions:

| Purpose | Key pattern | Example |
|---|---|---|
| Property category | `Cat[CategoryName]` | `CatAppearance`, `CatData` |
| Property description | `[Control]On[Property]Descr` | `ControlOnMyPropertyChangedDescr` |
| Event description | `[Event]Descr` | `CommandChangedEventDescr` |
| Exception message | `[Context]_[Error]` | `InvalidArgument_OutOfRange` |

### 9.2 Generating localization files

After adding English entries to `SR.resx`, **build the solution once**. The
build automatically generates `.xlf` translation files for all supported
languages from the English originals. In a subsequent pass, localize the
generated `.xlf` entries into their respective languages.

---

## Open Issues

1. **Ambient property pattern:** Properties like `DataContext` that cascade
   from parent to children require a dedicated `OnParent[Property]Changed`
   propagation pattern. This is out of scope for this Skill — a separate
   **Ambient Properties Skill** is being tracked (see upstream issue).
