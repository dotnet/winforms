---
name: netfx-breaking-change-guidance
description: >-
  Guidance for tasks involving WinForms compatibility differences from .NET
  Framework, breaking-change release notes, migration guidance, or issues
  labeled releasenotes-breaking-change-NetFx. Covers linked issues and PRs,
  before/after behavior, affected applications, compatibility switches,
  workaround risk, and test evidence.
metadata:
  author: dotnet-winforms
  version: "1.0"
---

# WinForms .NET Framework Breaking-Change Guidance

> **Golden rule:** Do not call a change breaking solely because behavior differs.
> Verify the .NET Framework behavior, the intended new behavior, customer impact,
> and the implementation status. Open PRs are proposals, not shipped changes.

## Current tracked change

Refresh this section before relying on it. The inventory below reflects label
usage as of 2026-07-18.

### Collection Editor movement buttons

- Tracking issue: dotnet/winforms#3152
- Implementation PR: dotnet/winforms#14668
- Label state: The PR carries `releasenotes-breaking-change-NetFx`; the issue
  did not carry the label when this skill text was drafted.
- Before (.NET Framework): The Collection Editor Up and Down buttons remain
  enabled at the first and last items. The handlers reject invalid moves only
  after the command is invoked.
- Proposed after behavior: Up is disabled for the first item and Down is
  disabled for the last item. No-selection and single-item states disable both.
- Affected scenarios: Shared design-time Collection Editor experiences,
  including ImageList, ListView, TabControl, TreeView, MonthCalendar, and other
  editable collections using the same form.
- Migration guidance: Application code normally needs no change. Update UI
  automation, accessibility assertions, screenshots, and workflows that expect
  boundary buttons to remain enabled. Do not depend on invoking an invalid
  boundary command or on incidental side effects from that command.
- Evidence: The PR adds first, middle, last, no-selection, and single-item unit
  coverage and has a green CI run. It remains open, so do not describe the
  behavior as shipped.

## Discovery workflow

1. Record the as-of date.
2. Confirm the exact label name and case in `dotnet/winforms`.
3. Search issues and PRs separately for
   `releasenotes-breaking-change-NetFx`; GitHub search otherwise mixes both.
4. For every labeled item, identify its tracking issue, implementation PR,
   target release, current state, and relevant commit.
5. Read the complete issue and PR discussion, source diff, tests, reviews, and
   CI results.
6. Reproduce or verify the .NET Framework behavior from reference source, a
   targeted test application, historical tests, or authoritative documentation.
7. Separate these categories:
   - regression fix restoring .NET Framework behavior;
   - intentional divergence from .NET Framework;
   - long-standing bug fix that may break customer workarounds;
   - implementation-only change with no observable compatibility impact.
8. Never fabricate an entry when the label search is empty.

## Required documentation for each change

Every entry must contain:

- linked tracking issue and implementation PR;
- current state and target .NET version;
- **Before (.NET Framework)** behavior;
- **After (.NET)** behavior;
- affected controls, designers, applications, and scenarios;
- source compatibility, binary compatibility, and behavioral compatibility;
- known customer workarounds that may regress;
- migration or mitigation guidance;
- compatibility switch or opt-out, or an explicit statement that none exists;
- unit, integration, manual, and CI evidence;
- uncertainty and missing comparison evidence.

## Writing guidance

- Lead with the observable behavior change, not implementation details.
- State whether action is required and who is affected.
- Prefer a minimal before/after example when it clarifies impact.
- Distinguish an open proposal from merged, preview, and shipped behavior.
- If a fix can invalidate common workarounds, also consider the
  `releasenotes-may-break-workaround` label.
- Keep the skill inventory synchronized with release notes and compatibility
  documentation when a PR merges, changes shape, or is abandoned.
