---
name: code-review
description: >-
  Review code changes in dotnet/winforms for concrete problems. Always use this
  skill when GitHub Copilot code review is requested on a pull request, or when
  a user asks to review, inspect, audit, or check a PR, branch, commit range,
  staged changes, uncommitted changes, or local changes in this repository.
  Supports hosted GitHub.com reviews that post findings directly and
  interactive reviews of local or remote changes. Focus on correctness,
  compatibility, native resource and handle lifetime, designer serialization,
  accessibility, DPI/rendering, performance, security, and regression coverage;
  omit praise, style-only nits, and speculative concerns.
metadata:
  author: dotnet-winforms
  version: "1.1"
---

# WinForms Code Review

Review changes as a skeptical WinForms maintainer. Identify concrete problems
that could affect customers, maintainers, or repository health. A review may
target a GitHub pull request or changes that exist only on the user's machine.

The repository is a mature Windows desktop framework with strict compatibility
requirements. Small changes can affect native window lifetime, message
dispatch, layout, accessibility, design-time serialization, binary and source
compatibility, high DPI behavior, or applications that depend on legacy quirks.
Treat the change description as a claim to verify, not as proof.

## Review contract

* Remain read-only. Do not edit source, reset files, switch the user's branch,
  stash changes, or alter commits during a review.
* In hosted GitHub Copilot code review, publish verified findings directly as
  review comments. The user's review request is authorization to post the
  review; do not wait for another confirmation that the hosted workflow cannot
  provide.
* In an interactive CLI or cloud-agent session, present findings before posting
  and let the user select which findings become GitHub comments.
* Report problems only. Do not add praise, summaries of code that is correct,
  style preferences, or unrelated refactoring suggestions.
* Ground every finding in a specific changed line and a concrete failure
  mechanism. Read enough surrounding code to prove the mechanism.
* Prefer a smaller set of actionable findings over a large set of guesses.
  High- and medium-confidence findings belong in the report. Put unresolved
  questions in a separate section and never present them as defects.
* Review the complete requested change set. Do not stop after finding the first
  issue or after reading only the diff.
* Respect the existing style of the changed file. Automated analyzer or
  formatting failures are not review findings unless they expose a substantive
  problem.

## 1. Select the review mode

### Hosted GitHub Copilot code review

If this skill is running inside GitHub Copilot code review on GitHub.com,
GitHub Mobile, or another hosted Copilot code review surface, use **hosted PR
mode**. The platform already supplies the pull request identity, head checkout,
diff, repository context, and review-comment publishing mechanism.

Use the platform-provided PR context and tools. Do not run `gh` to rediscover
the PR, fetch pull refs, create a worktree, ask which comments to publish, or
wait for an additional user response. Repository skills are loaded from the
PR's head branch, so apply the version of this skill present in that branch.

### Pull request mode

Use PR mode when the user supplies a PR number or URL, asks to review the
current branch's PR, or explicitly refers to a pull request. The default
repository is `dotnet/winforms`.

Outside hosted Copilot code review, if the user asks for a PR review without an
identifier, discover a PR for the current branch:

```powershell
gh pr view --repo dotnet/winforms --json number,title,headRefName,url
```

If no PR exists, continue in local mode. Do not create a PR.

### Local mode

Use local mode when the user asks to review local, uncommitted, staged,
unstaged, untracked, committed-but-unpushed, or branch changes. Git is the
source of truth; do not require a PR.

Honor an explicit scope. Otherwise use these defaults:

| Request | Reviewed change set |
| --- | --- |
| "review staged changes" | Index only: `git diff --staged` |
| "review unstaged changes" | Tracked working-tree changes: `git diff` |
| "review uncommitted changes" | Index + tracked working tree: `git diff HEAD`, plus untracked files |
| "review this commit/range" | The named commit or range |
| "review this branch against X" | Merge base with the named base through the current working tree |
| "review my local changes" | Current branch changes from the `origin/main` merge base, plus index, working tree, and untracked files |

For the comprehensive local default, one diff from the merge base to the
working tree includes committed branch changes plus staged and unstaged tracked
changes:

```powershell
git status --short --branch
git fetch origin main --quiet
$base = git merge-base HEAD origin/main
git diff --name-status $base
git diff --find-renames --find-copies $base
git ls-files --others --exclude-standard
```

If `origin/main` is unavailable, fall back to local `main`. Never include
ignored build output. Untracked files do not appear in `git diff`; list and
read them explicitly. Note the exact scope used in the review result.

## 2. Prepare PR source for the current host

### Hosted PR mode

Use the checkout and PR diff supplied by GitHub Copilot code review. Review the
head commit that the platform selected and use its repository-wide context
gathering and configured MCP tools when relevant. Do not create a second
checkout or temporary worktree, and do not use `gh api` to publish comments;
return findings through the hosted review mechanism.

### Interactive PR mode

Read and follow the
[interactive PR review workflow](references/interactive-pr-review.md). It
resolves the PR head in an isolated worktree, preserves the user's checkout,
and documents safe cleanup and comment publication. Never use
`gh pr checkout`, stash, or switch branches in the user's working tree.

## 3. Gather context before judging

For both modes:

1. Inventory all added, modified, deleted, copied, and renamed files.
2. Read every changed file in full when practical. For large generated or data
   files, inspect the relevant structure and semantic changes rather than
   flooding the context.
3. Read the base version of deleted, moved, or substantially rewritten code.
   Compare old and new behavior explicitly.
4. Use code intelligence to inspect definitions, references, callers,
   overrides, derived types, and data access. Prefer LSP or semantic navigation
   over text search; use text search for patterns and generated references.
5. Find canonical sibling implementations. WinForms frequently has parallel
   implementations across controls, C#/VB analyzers, runtime/design-time code,
   integer/float drawing overloads, and UIA/MSAA providers.
6. Inspect relevant tests, project files, `PublicAPI` files, resources, and
   documentation even when they are unchanged.
7. Use `git log`, `git blame`, linked issues, and earlier PRs when they explain
   a compatibility constraint or a deliberately unusual implementation.

In PR mode, form an independent understanding of the diff before relying on
the PR description. Inspect existing review threads and status checks with the
host-provided context and tools so findings are not duplicated. In an
interactive session, use these `gh` fallbacks:

```powershell
gh pr view $pr --repo dotnet/winforms --comments
gh api --paginate "repos/dotnet/winforms/pulls/$pr/comments?per_page=100"
gh pr checks $pr --repo dotnet/winforms
```

Do not duplicate an existing review comment. A green CI run is useful evidence,
not proof that the behavior is correct.

## 4. Classify the changed area

Use the classification to choose the relevant review passes and supporting
skills.

| Area | Common paths | Primary risks |
| --- | --- | --- |
| Controls and runtime | `src\System.Windows.Forms\**` | Compatibility, handle creation/recreation, message handling, layout, events, painting, threading |
| Native primitives and interop | `src\System.Windows.Forms.Primitives\**`, `src\System.Private.Windows.Core\**` | ABI correctness, marshalling, pointer width, native ownership, last-error handling |
| Drawing and GDI+ | `src\System.Drawing.Common\**`, `src\System.Private.Windows.GdiPlus\**`, `src\System.Drawing\**` | Graphics state, GDI/GDI+ lifetime, geometry, rendering parity, version guards |
| Designer | `src\System.Windows.Forms.Design\**`, `src\System.Drawing.Design\**`, `src\System.Design\**` | CodeDOM serialization, property metadata, services, out-of-process designer behavior |
| Accessibility | `src\Accessibility\**`, runtime `**\Accessibility\**`, accessibility tests | UIA/MSAA contracts, roles, control types, navigation, patterns, events, runtime IDs |
| Analyzers and generators | `src\System.Windows.Forms.Analyzers*\**`, `src\System.Windows.Forms.PrivateSourceGenerators\**` | C#/VB parity, false positives, generated-code handling, diagnostics/code-fix correctness |
| Visual Basic | `src\Microsoft.VisualBasic*\**` | Legacy behavior, C#/VB parity where intended, project and runtime compatibility |
| Tests | `src\test\**`, project-local `tests\**` | Scenario fidelity, STA/thread isolation, native cleanup, handle-state assertions |
| Build and packaging | `eng\**`, `pkg\**`, root props/targets, pipelines | Arcade flow, package contents, source build, architecture/configuration conditions |
| Contributor and AI guidance | `docs\**`, `.github\skills\**`, `.github\**` | Commands or conventions drifting from executable repository behavior |

Read the relevant existing skill when its area appears in the diff:

* [New control API](../new-control-api/SKILL.md)
* [Control API tests](../control-api-tests/SKILL.md)
* [Using and extending GDI+](../using-and-extending-gdi-plus/SKILL.md)
* [GDI rendering tests](../gdi-rendering-tests/SKILL.md)
* [Building code](../building-code/SKILL.md)
* [Running tests](../running-tests/SKILL.md)

Repository documentation and the nearest existing implementation take
precedence over generic .NET advice.

## 5. Perform focused review passes

### Pass A: Behavioral and compatibility contracts

* State the old and new observable behavior in your own words.
* Check source, binary, serialization, and behavioral compatibility. WinForms
  intentionally preserves many legacy behaviors; "cleaner" behavior can still
  be a breaking change.
* Trace virtual dispatch and inheritance. Look for removed overrides, changed
  call order, skipped `base` calls, relaxed validation, different exceptions,
  and changes visible to derived controls.
* Check default values, event order, event sender/arguments, idempotent sets,
  invalidation/layout timing, focus/selection, keyboard and mouse behavior, and
  right-to-left behavior.
* Check app-context switches, feature flags, target framework conditions, and
  compatibility fallbacks on both sides of each branch.
* Verify null, empty, duplicate, disposed, reentrant, extreme-size, and failure
  paths that reach the changed code.

### Pass B: Public API and component model

For public or protected surface changes:

* Require an approved or actively tracked API proposal consistent with
  [CONTRIBUTING.md](../../../CONTRIBUTING.md).
* Verify every new public/protected member and newly introduced override is in
  the correct `PublicAPI.Unshipped.txt`; shipped APIs must not disappear or
  change incompatibly.
* Compare implementation and `PublicAPI` nullability, modifiers, parameter
  names, and return types.
* For `System.Windows.Forms`, new APIs are stable by default and are not wrapped
  in `NETxx_0_OR_GREATER` guards. `System.Drawing` additions follow their
  area's current target-version guard. Do not apply one area's rule to another.
* For control properties, verify `PropertyStore` use where per-instance fields
  would increase control size, the default is represented consistently, and
  change events are raised only when the effective value changes.
* Verify component-model metadata: `DefaultValue`,
  `DesignerSerializationVisibility`, `Browsable`, `Bindable`, `Localizable`,
  `SRCategory`, `SRDescription`, `EditorBrowsable`, type converters, and
  editors.
* Check dedicated `EventArgs`/delegate conventions, `Events` storage, virtual
  `OnXxx` methods, and XML documentation of customer-visible behavior.

### Pass C: Native handles, messages, interop, and threading

* Check whether a getter, setter, or query accidentally creates a native handle.
  If no handle should be created, expect tests to assert
  `IsHandleCreated == false`.
* For `CreateHandle`, `DestroyHandle`, `RecreateHandle`, `CreateParams`, and
  `WndProc` changes, trace state across recreation and disposal. Verify window
  styles, message results, base/def-window processing, and child/native state.
* Look for reentrancy caused by `SendMessage`, event callbacks, layout,
  accessibility queries, handle creation, or user overrides. State must be
  valid before calling code that can reenter.
* Verify UI-thread affinity, synchronization-context assumptions, COM apartment
  requirements, cancellation propagation, and shutdown/disposal races.
* Compare P/Invoke signatures and native structs with the platform definition
  and nearby generated interop. Check character set, signedness, packing,
  unions, pointer-sized values, buffer lengths, BOOL/HRESULT semantics, and
  last-error use.
* Establish ownership for every HWND, HDC, GDI object, COM pointer, global
  memory block, callback, pin, and `GCHandle`. Release each resource with its
  matching API exactly once, including exceptional paths. Never delete borrowed
  or stock objects.
* Treat clipboard, drag/drop, ActiveX, COM, deserialization, and external native
  data as hostile input. Check type restrictions, size bounds, and unsafe
  legacy fallbacks.

### Pass D: Designer and serialization

* Verify runtime and design-time behavior remain aligned. A property that works
  at runtime can still fail to persist, reset, localize, or round-trip in the
  designer.
* Check `DefaultValue` against the actual getter default. For complex defaults,
  verify private `ShouldSerializeXxx` and `ResetXxx` methods.
* Trace CodeDOM serialization, inherited/read-only properties, ambient values,
  content serialization, extender providers, type converters, editors, and
  designer services.
* Consider both in-process legacy behavior and the modern out-of-process
  designer boundary. Do not assume an internal runtime type or service can
  cross that boundary.
* Verify generated code remains loadable and semantically equivalent in C# and
  VB where the changed feature supports both languages.

### Pass E: Rendering, layout, DPI, themes, and accessibility

* Check logical/device coordinate conversions, rounding, negative and empty
  rectangles, overflow, per-monitor DPI transitions, font scaling, autoscaling,
  preferred-size calculations, and layout convergence.
* Verify painting clips correctly, invalidates the necessary region, avoids
  flicker, preserves graphics state, and does not leak pens, brushes, paths,
  regions, fonts, HDCs, or selected GDI objects.
* Prefer existing cached pens/brushes when the repository convention applies.
  Ensure cached or stock objects are not disposed and owned objects are.
* Check visual styles, classic theme, dark mode, high contrast, disabled state,
  right-to-left layout, text rendering mode, and non-100% scaling.
* For drawing APIs, verify integer/float overload parity, draw/fill pairs,
  `GraphicsPath` behavior, version guards, and meaningful pixel/geometry tests.
* For UI changes, verify accessible name, role/control type, state, patterns,
  parent/child navigation, hit testing, focus, stable runtime IDs, and UIA/MSAA
  property or structure-change events. Visual correctness does not prove
  accessibility correctness.

### Pass F: Performance, resources, and security

* Scrutinize painting, layout, message dispatch, accessibility, and control-tree
  traversal as hot paths. Look for new allocations, boxing, repeated native
  calls, global locks, linear scans in nested loops, and unbounded caches.
* Check disposal and finalization paths, event unsubscription, timers,
  cancellation registrations, thread-static caches, and process-wide state.
* Check integer overflow, buffer sizing, path handling, format strings, resource
  lookup, and malformed native or serialized input.
* Verify user-facing strings use the appropriate `SR.resx` resources and that
  resource keys/format arguments are correct. Generated localization churn is
  not itself evidence of a bug.
* Flag manual edits under `eng\common` unless they are clearly part of the
  Arcade code-flow process.

### Pass G: Regression coverage

Map each changed production path to an observable risk before evaluating tests.
Do not accept "tests were added" without checking what they prove.

| Change | Expected evidence |
| --- | --- |
| Control property/event/API | Default, set/get, idempotency, event lifecycle, no-handle and with-handle behavior in `src\test\unit\System.Windows.Forms` |
| Handle/message/native state | Tests for pre-handle, created handle, recreation/disposal, native failure, and relevant 32/64-bit or thread behavior |
| Designer metadata/serialization | Designer unit tests and, when behavior crosses the boundary, integration/round-trip coverage |
| Accessibility | Property, role/control type, navigation/pattern, focus/event behavior with the relevant accessibility level/configuration |
| Rendering/layout/DPI | Geometry or bitmap assertions, meaningful DPI/theme variants, and both integer/float overloads where applicable |
| Analyzer/code fix/generator | Positive and negative cases, generated code, language-version edges, and C#/VB parity when both front ends exist |
| Bug fix | A focused test that fails before the fix for the reported scenario |

Use `[WinFormsFact]`/`[WinFormsTheory]` for tests requiring controls or a
synchronization context. Tests involving process-wide UI state, clipboard,
drag/drop, or other global state may require sequential isolation. Async tests
must propagate `TestContext.Current.CancellationToken`.

Do not require tests for documentation-only changes, comments, generated-only
updates, or mechanical refactors whose behavior equivalence is demonstrated.
When reporting a coverage gap, name the exact regression scenario and where the
test belongs.

## 6. Validation during review

Run only validation that helps confirm or refute a suspected problem. Prefer
the smallest relevant test project or filtered test. This repository uses
xUnit v3 on Microsoft.Testing.Platform (not the legacy VSTest adapter). Note
that `dotnet test --filter` does not work; filtering is still available through
`dotnet test -- --filter-method/--filter-class ...` or by running the built test
executable directly (see the `running-tests` skill for exact command syntax).

Use `.\build.cmd` for an authoritative repository build because it enables the
Arcade CI configuration, PublicAPI analyzers, code-style/documentation
analyzers, and warnings-as-errors. A single-project `dotnet build` is only
inner-loop evidence and must not be presented as equivalent to CI.

Record exactly what was run and the result. Do not claim a scenario was tested
when it was only inspected. Do not turn pre-existing or unrelated failures into
findings against the change.

In hosted PR mode, use the ephemeral environment and setup supplied by Copilot
code review. Do not create another checkout or attempt to access the user's
local machine. An environment limitation is not a finding against the PR.

## 7. What to report

Prioritize:

1. Customer-visible correctness and compatibility regressions.
2. Native memory, handle, GDI/GDI+, COM, threading, and reentrancy defects.
3. Public API or designer serialization contract violations.
4. Accessibility, DPI, layout, rendering, theme, and input regressions.
5. Security vulnerabilities and unsafe processing of external data.
6. Significant hot-path performance or resource-lifetime regressions.
7. Missing scenario-accurate regression coverage for changed behavior.
8. Repository guidance or documentation made materially false by the change.

Do not report:

* Formatting, naming preferences, or modern-language suggestions that do not
  affect behavior.
* Problems already guaranteed to be caught clearly by an existing analyzer or
  compiler diagnostic, unless the change suppresses or bypasses that check.
* Cross-platform concerns whose only premise is that WinForms is Windows-only.
* Generic requests for more tests without a specific uncovered regression.
* Hypothetical races, leaks, or compatibility concerns without a reachable
  code path and mechanism.
* Unrelated pre-existing issues, except defects copied or moved into newly
  owned code. Label those explicitly as pre-existing.

For moved or rewritten code, compare implementations line by line and treat the
new location as reviewed code. Check callers of removed types/members and any
state or validation previously supplied by the old owner.

## 8. Present findings

### Hosted GitHub Copilot code review output

Publish each verified problem directly as an actionable review comment, using
the hosted review system's severity labels and comment format. Attach a finding
to the smallest relevant changed line or range. If the problem spans files or
cannot be attached accurately to a changed line, put it in the review overview.

Keep one root cause per comment. Explain the reachable mechanism, customer or
maintainer impact, and concise fix direction. Do not emit a numbered
"findings for selection" report, ask the user which comments to post, invoke
`gh api`, or defer comments to a later interactive step.

If no problems meet the evidence threshold, publish no inline comments and let
the hosted review overview report that no issues were found. Do not invent a
finding to ensure the review has comments.

### Interactive review output

Order findings by severity, then confidence. Use this format:

```markdown
## Findings

1. **[P1][High] Preserve native state when recreating the handle**
   — `src\System.Windows.Forms\...\Control.cs:123`

   `RecreateHandle()` now destroys the native state before it is captured, so
   the value is reset after a DPI transition. This is reachable from ...

   **Fix direction:** Capture ... before ... and restore it after ...
```

Severity:

* **P0** — Catastrophic or broadly exploitable; blocks all use or release.
* **P1** — Must fix before merge; customer-visible regression, security issue,
  corruption, crash, leak, or broken public/native/designer contract.
* **P2** — Should fix; meaningful edge-case correctness, performance,
  accessibility, reliability, or regression-coverage problem.
* **P3** — Minor but concrete behavioral or maintainability defect. Never use
  P3 for style.

Confidence:

* **High** — Directly verified in code, a test/repro, or authoritative contract.
* **Medium** — Mechanism is supported by code, but one environmental or
  behavioral detail could not be executed or confirmed.

Each finding must:

* identify one problem;
* cite a changed file and line;
* explain the reachable mechanism and impact;
* mention key evidence or what remains unverified;
* give concise fix direction when it is not obvious.

If there are no findings, say:

```markdown
## Findings

No findings.

**Residual risk:** <untested or unverified area, or "None identified">
**Review scope:** <exact diff/range and whether untracked files were included>
**Validation:** <commands run, or "Not run">
```

Do not invent a finding to avoid an empty review.

## 9. Posting PR feedback

This section applies only to PR mode.

### Hosted GitHub Copilot code review

The review request itself authorizes publication. Submit all verified findings
through the platform's normal review output without requesting further user
selection or confirmation. Let the hosted product apply its configured review
event and approval policy; do not independently submit another review through
`gh` or the GitHub API.

Before returning a comment, check the available existing review threads to
avoid duplication. Post one root cause once, on a changed line when possible.
The Copilot reviewer identity already discloses that the review is
AI-generated, so no additional disclosure is required.

### Interactive PR review

Follow the publication procedure in the
[interactive PR review workflow](references/interactive-pr-review.md).
Present findings for selection before using a GitHub review API, and never post
draft, duplicate, or unselected findings.

Local mode ends after presenting findings; there is no review to post.
