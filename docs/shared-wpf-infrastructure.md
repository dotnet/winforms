# Shared WinForms/WPF Infrastructure

## Overview

This document describes infrastructure components shared between **WinForms** (in the `dotnet/winforms` repository) and **WPF** (in the `dotnet/wpf` repository). Understanding this sharing is critical for contributors and maintainers when making changes to shared code, as modifications can impact both UI stacks.

**Key takeaway:** Changes to `System.Private.Windows.Core` and certain other shared assemblies in this repository affect both WinForms and WPF applications. Always consider cross-stack implications when modifying these components.

## Which Components Are Shared

The primary shared component is **`System.Private.Windows.Core`**, which contains low-level Windows interop and OLE/COM support used by both WinForms and WPF. The most critical shared subsystem is the **clipboard and OLE data handling** implementation.

### System.Private.Windows.Core

**Location:** `src/System.Private.Windows.Core/`

**Key shared areas:**
- **OLE/Clipboard implementation:** `src/System.Private.Windows.Core/src/System/Private/Windows/Ole/`
  - `Composition.NativeToManagedAdapter.cs` - Adapts native `IDataObject` to managed interface
  - `Composition.ManagedToNativeAdapter.cs` - Adapts managed `IDataObject` to native COM
  - `DataObjectCore.cs` - Core data object implementation
  - `ClipboardCore.cs` - Core clipboard operations
  - `BinaryFormatUtilities.cs` - Binary format handling for serialization
  - Related support classes for format conversion, drag-and-drop, and data transfer

**Why this matters:** These classes handle fundamental clipboard and drag-and-drop operations. Bugs in this code can manifest in both WinForms and WPF applications, and fixes here benefit both stacks.

## How Sharing Works

The sharing mechanism involves packaging, transport, and ingestion into the Windows Desktop shared framework:

1. **Packaging in dotnet/winforms:**
   - The `Microsoft.Private.Winforms` transport package is built from this repository
   - This package bundles `System.Private.Windows.Core` and other shared assemblies
   - See: `pkg/Microsoft.Private.Winforms/README.md`

2. **Ingestion by WPF:**
   - The `dotnet/wpf` repository consumes the `Microsoft.Private.Winforms` NuGet package
   - Props and targets from `pkg/Microsoft.Private.Winforms/sdk/dotnet-wpf/` are used by WPF's SDK
   - WPF builds integrate these assemblies into their artifact set

3. **Windows Desktop Shared Framework:**
   - Both WinForms and WPF assemblies are bundled into the `Microsoft.WindowsDesktop.App` shared framework
   - The `dotnet/windowsdesktop` repository orchestrates this bundling
   - Applications targeting Windows Desktop get a single unified set of assemblies at runtime

**Result:** At runtime, WinForms and WPF applications in the same process share a single instance of `System.Private.Windows.Core.dll`, ensuring consistent behavior for clipboard, OLE, and related operations.

## Concrete Examples and Case Studies

### Case Study 1: IStream Double-Release (WPF #11401, WinForms #14257/#14296)

**Issue:** When clipboard data was stored using `TYMED_ISTREAM`, the code would wrap the `IStream` pointer in a `ComScope` and then call `ReleaseStgMedium`. This caused a double-release: `ComScope.Dispose()` would release the stream, and then `ReleaseStgMedium` would release it again, leading to crashes or undefined behavior.

**Root cause:** Located in `Composition.NativeToManagedAdapter.cs`, method `TryGetIStreamData<T>`

**Fix in PR #14257:**
```csharp
// Don't wrap in ComScope - ReleaseStgMedium will release the stream.
Com.IStream* pStream = (Com.IStream*)medium.hGlobal;
```

**Impact:**
- **WPF #11401:** Reported as a WPF issue, but the bug was in shared code in dotnet/winforms
- **WinForms #14257:** Fixed in main branch
- **WinForms #14296:** Draft PR to backport the fix to release/10.0
- **Outcome:** Single fix in dotnet/winforms resolved the issue for both WinForms and WPF

**Lesson:** Bugs in shared clipboard/OLE code often surface in both stacks. When WPF reports a clipboard or data transfer issue, check `System.Private.Windows.Core` code in this repository.

### Case Study 2: GetData Failure and Uninitialized STGMEDIUM (WPF #11402, WinForms #14257)

**Issue:** When clipboard content changed between `QueryGetData` (which checks availability) and `GetData` (which retrieves the data), `GetData` would fail but the code would still attempt to read from the uninitialized `STGMEDIUM` structure. This led to reading garbage data or crashing.

**Root cause:** Located in `Composition.NativeToManagedAdapter.cs`, method `TryGetHGLOBALData<T>`

**Fix in PR #14257:**
```csharp
// If GetData failed, don't try to read from the medium - it may contain uninitialized data.
// (This can easily happen when the clipboard content changes between QueryGetData and GetData calls.)
if (hr.Failed)
{
    return false;
}
```

**Impact:**
- **WPF #11402:** Reported as a WPF clipboard reliability issue
- **WinForms #14257:** Fixed in main branch along with other hardening improvements
- **Outcome:** Better error handling and null returns instead of corrupted data, benefiting both stacks

**Lesson:** Race conditions and error handling in shared code require careful validation. Changes should anticipate contention scenarios common in clipboard operations.

### Additional Improvements in PR #14257

- **UTF-8 HGLOBAL hardening:** Improved string reading to prevent malformed data from causing exceptions
- **Test coverage:** Added mocks and tests for STGMEDIUM handling scenarios
- **Constants:** Added `CLIPBRD_E_CANT_OPEN` and other clipboard-related error constants

## Advantages of Sharing

1. **Consistency:** Clipboard and OLE behavior is identical across WinForms and WPF applications
2. **Single fix benefits both:** Bug fixes in shared code automatically improve both stacks
3. **Shared test coverage:** Tests in dotnet/winforms validate behavior for WPF as well
4. **Reduced duplication:** Avoids maintaining two separate implementations of complex OLE/COM interop
5. **Better quality:** Concentrated development and review effort on a single implementation

## Liabilities and Risks

1. **Coupling:** Changes in dotnet/winforms can break WPF, and vice versa (though WPF changes rarely touch these components)
2. **Regressions affect both stacks:** A bug introduced in shared code will impact both WinForms and WPF
3. **Validation requirements:** Changes must be tested in both WinForms and WPF scenarios
4. **Discoverability:** It may not be obvious to contributors that code changes impact WPF

## Guidance for Contributors

### When Working on Shared Code

**Which code is shared?**
- Primarily `src/System.Private.Windows.Core/`, especially the `Ole/` directory
- If in doubt, check if the code is in `System.Private.Windows.Core` or referenced in `pkg/Microsoft.Private.Winforms/`

**Before making changes:**
1. **Consider WPF impact:** Ask yourself: "Could this change affect WPF clipboard, drag-and-drop, or data transfer?"
2. **Search for related WPF issues:** Check the [dotnet/wpf issue tracker](https://github.com/dotnet/wpf/issues) for similar problems
3. **Review existing tests:** Look at test coverage in `src/System.Private.Windows.Core.Tests/` and `src/System.Windows.Forms.Tests/` for clipboard/OLE scenarios

**When making changes:**
1. **Prefer small, targeted fixes:** Especially for release branches (e.g., `release/10.0`), minimize scope to reduce risk
2. **Larger refactorings belong in main:** Structural changes and broad refactorings should target the `main` branch
3. **Add or update tests:** Ensure your changes have test coverage, including error handling and edge cases
4. **Add cross-repo references:** If your PR addresses a WPF issue, link it in the PR description:
   ```
   Fixes dotnet/wpf#11401
   Relates to dotnet/wpf#11402
   ```
5. **Document breaking changes:** If behavior changes in observable ways, call it out explicitly

**Validation checklist:**
- Run WinForms tests: `build.cmd -test`
- Test clipboard operations manually:
  - Copy/paste text between applications
  - Copy/paste images
  - Try custom data formats if your change affects them
  - Test drag-and-drop scenarios
- If possible, validate with a WPF application
  - WPF development follows the same patterns (`build.cmd`, `start-vs.cmd`, etc.)
  - Consider testing via manual insertion of the updated shared DLL or repointing to a locally built transport package
- Check for memory leaks or COM reference counting issues in loops or stress scenarios

**Code review considerations:**
- Ensure error handling is robust (null checks, HRESULT validation, etc.)
- Look for potential double-release or use-after-free patterns in COM/OLE code
- Verify that `ComScope` disposal is correct and that different release mechanisms don't overlap
- Consider thread safety and race conditions (clipboard content can change at any time)
- Look for changes in state return (different values, exceptions). Changes must be carefully considered and have strong justification to minimize breaking changes.
- Document all breaking changes.
- OLE / clipboard state is shared machine wide. It is expected that state can change at any time. Handle state change gracefully.
- Avoid second guessing application intent and shared OLE state. If the data is not retrieved successfully just accept the result, don't retry. Existing retry logic can be kept- if API users want retry logic they should either implement their own wrappers or present new API requests where intent can be made explicit.

### Backporting to Release Branches

When backporting clipboard/OLE fixes to release branches (e.g., for WPF servicing):
- **Minimize risk:** Only backport critical bug fixes, not refactorings or enhancements
- **Preserve behavior:** Avoid changing API signatures or introducing observable behavioral changes
- **Test thoroughly:** Release branches require extra validation since they're servicing supported releases
- **Coordinate with WPF team:** Reach out on GitHub or in discussions if the issue originated in WPF

### Related Open Issues

The following issues track ongoing work and awareness in this area:
- [dotnet/winforms#14304](https://github.com/dotnet/winforms/issues/14304)
- [dotnet/winforms#14305](https://github.com/dotnet/winforms/issues/14305)

## Additional Resources

- **Packaging and transport:** `pkg/Microsoft.Private.Winforms/README.md`
- **Clipboard/DataObject API changes:** `docs/dataobject-serialization.md`
- **Contributing to WinForms:** `CONTRIBUTING.md`
- **Issue guidelines:** `docs/issue-guide.md`
- **WPF repository:** https://github.com/dotnet/wpf
- **Windows Desktop repository:** https://github.com/dotnet/windowsdesktop

## Summary

Shared infrastructure between WinForms and WPF provides significant benefits but requires awareness and care when making changes. Contributors should:
- Recognize when they're working on shared code (especially `System.Private.Windows.Core`)
- Consider the impact on both UI stacks
- Add appropriate test coverage
- Link related WPF issues in PRs
- Validate changes in both WinForms and WPF scenarios when possible
- Prefer small, targeted changes for release branches

By following these guidelines, we can maintain high-quality, reliable clipboard and OLE functionality for both WinForms and WPF applications.
