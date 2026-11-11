---
name: fix-issue
description: 'Take one claimed agent-ready issue to a validated draft pull request in its own worktree, without merging or approving. The fix role follows this skill.'
---

# Fix an Issue

Follow the [factory-coordination skill](../factory-coordination/SKILL.md).
The launcher names the issue, the worktree, the `factory/<issue>-<slug>`
branch, the remote and the trailers each commit carries.

1. Read the issue, its comments and what it links. Confirm it is open,
   assigned to you and labeled `agent-ready`. Restate its acceptance
   criteria. If they are missing or ambiguous, or the work needs a product,
   risk or architecture decision, stop with a `blocked` status,
   `reason=scope`, that names the question. A low-risk, reversible choice the
   evidence leaves open needs no question: take the simplest option and
   record it on an `Assumption:` line of the pull request body. A choice that
   changes a public contract, a persisted format, security, cost or
   infrastructure, or that deletes data, still stops.
2. Run the factory-coordination checks before starting. If `main` already
   meets every criterion, post `state=done` with evidence and close the
   issue. If the issue is larger than one reviewable pull request, split it
   with the [work-queue skill](../work-queue/SKILL.md) and stop. Count size
   the way a merge gate should: added plus deleted lines, tests included,
   from `git --attr-source=<empty tree> diff --no-renames --numstat <remote>/main...HEAD`,
   so an attributes file the branch adds cannot hide lines, a moved file
   counts as deleted plus added lines and a changed binary file as one.
   Prefer a slice within the component count the prompt names, counted the
   way it says. That count is guidance, not a merge limit: split by
   component when the halves are independently mergeable, and keep one
   coherent change together rather than split it artificially. An urgent
   issue (the launcher's prompt says so, such as an incident or a red
   `main`) ships whole in one pull request: never split it, and file only
   work outside its core scope as follow-up issues. So does an issue whose
   owner or a write-role maintainer applied the `ship-whole` label. A pull
   request over the size limit or touching a human-review path auto-merges
   only where the policy allows a waiver and the review accepts the body's
   justification (which cites any `ship-whole` label), and never for a path
   the policy marks unwaivable; otherwise an operator merges it. When such a
   path is unavoidable and not justified, or is unwaivable, write
   `Operator merge required: <path>` where the body lists decisions made
   without the owner, so the owner sees it before review.
3. Read the code the change touches, its callers and tests, and the
   constitution rules that apply to it. Make the simplest complete change
   inside the issue's scope.
4. Give each behavior change a test that fails without it. For a defect,
   first write a test that reproduces the reported failure and fails for the
   reported reason, not a missing tool or a compile error. A test of a guard
   or rejection uses inputs that violate only that guard. Search the changed
   files and their callers for the same defect, and fix each sibling or
   record why not. Prove each new test discriminates: revert the production
   change in a throwaway checkout (never your worktree or a stash), run the
   test, and record `red on revert: <command> → <failing assertion>` in the
   pull request body. The recorded failure is the test's own assertion
   naming the reverted behavior's expected and actual values; a bare
   `failed`, `exit 1`, a panic elsewhere or a compile error does not count,
   and an assertion over a formatted or derived value counts only when that
   value differs between the two behaviors.
   <a id="red-on-mutation"></a>Reverting a whole hunk cannot discriminate a
   guard, an ordering or a persisted write inside it. For each new
   conditional guard, stated ordering or persistence promise, also mutate
   only that construct in another throwaway checkout: replace the guard's
   condition with its pass-through constant, swap the two ordered calls or
   drop the persisted write. Run the test the same way and record
   `red on mutation: <command> → <failing assertion>` next to it.
   Patch a value where the code under test reads it, not a copy bound at
   import or as a default argument. Point every patched state or output path,
   and any path derived from it, at a temporary directory. Await an event
   under a timeout instead of sleeping for a fixed window.
5. Compile or type-check after each edit, before slower or remote checks.
   Run the narrowest tests that cover the change, then the profile's checks
   that apply: a `blocking` check must pass; report each `advisory` failure.
   Classify every failure before acting on it. While iterating, rerun only
   the failed checks; before pushing, run every affected check once on the
   final committed head.
6. Read the complete diff and `git status`; remove any runtime state, logs or
   scratch output the change added, and fix whatever wrote them. Search the
   documentation, specifications, examples, configuration and tests that
   describe or pin each changed behavior or value, and update them in the
   same change; list them on a `Co-change:` line of the body, or write
   `none: <search scope>`.
   <a id="decision-restatement-sweep"></a>When the change amends or reverses
   a numbered design decision, also `git grep -n` the decision ID and the key
   terms of the replaced behavior across the design, API and specification
   folders, including files the change already edits, since prose restates
   a decision without naming it. Fix each hit or record why it still holds,
   and name the sweep's command and folders on the `Co-change:` line.
   For each new sequence of writes to durable or external state, check what
   a restarted owner sees after a crash between any two steps, including a
   completion marker written before its effect. Give every shutdown, drain
   or join a deadline, and keep each field added to a persisted,
   configuration or wire format readable by the previous release during a
   rolling deploy or rollback.
7. Before the first push, have a fresh-context subagent that did not write
   the change review `git --attr-source=<remote>/main diff <remote>/main...HEAD`
   (under `main`'s attributes, not the branch's) against the issue with
   the [code-review skill](../code-review/SKILL.md), on a different model
   when one is available. Start every subagent, this one included, with an
   explicit model the profile allows, the job's own unless a step asks for
   another: a subagent started without one takes the host's settings or the
   CLI default. Besides that skill's checks, the review covers:
   - <a id="records-at-rest"></a>**Records at rest.** When the diff tightens
     a validator over stored values, renames a stored document or field ID,
     or stops reading, writing or stripping a stored field, the pull request
     body lists the durable records `main` already wrote in the old shape
     and states for each one that it is migrated, deleted by a documented
     release purge (quoting the selector that matches it) or still handled
     at a cited `path:line`. A shape change without that statement is
     blocking.
   - <a id="removal-inventory"></a>**Removal inventory.** It lists each
     deleted branch, fallback, error mapping or default, and each new
     cancellation or deadline source on a multi-step write, with the reason
     it is safe; one without a reason is blocking.
   - **Guidance edits.** When a hunk changes skills, agent definitions,
     instruction files or launcher prompt strings, it lists each new or
     changed rule (an action the agent must, may or must never take, such as
     file, skip, close, comment, label, key or block) and quotes the rule on
     the same action from the runner skill the guidance runs under, the
     factory-coordination skill, the repository's shared rules and the
     launcher code or prompt that enforces it. A rule that contradicts one
     of them, unless the same change updates that rule too, or an action the
     runner skill does not permit is blocking.

   Fix its blocking findings.
8. Commit with an imperative subject, `Fixes #N` (or `Refs #N` while
   criteria remain) and the launcher's trailers.
9. <a id="claim-check"></a>Check every claim in the pull request body
   against the committed head: each statement of behavior cites the
   `path:line` that supports it, and each check it names passed at that
   head. Correct the claim or the code wherever they differ. Apply the same
   rule to every document section the diff edits: a status or coverage label
   covers only what a test or measurement shows, and the section's unchanged
   sentences and derived numbers still hold.
   One citation supports an example but cannot prove a list complete: a
   quantified or closed-list claim ("every", "all", "only", "none", or a
   list of the cases that count) also names the command that enumerates its
   whole population, such as `git grep -n` or a symbol search over the full
   scope, and states that the claim covers every hit. Record
   `Enumerated: <claim> → <command> (<n> hits)` in the body's validation
   section, or narrow the claim.
   The check also covers each sentence the diff deletes, or relabels as
   superseded, stale, resolved or no longer true: cite the `path:line` at
   `HEAD` that shows the fact is now false, or keep the sentence and record
   in the body that the issue's premise did not hold.
   Then sweep references outside the diff: `git grep -n` each identifier,
   key and quoted string the diff removes or changes, and each behavior
   handle (route path, decision or requirement ID, test-suite description or
   design-conflict row), across specifications, docs and doc comments, since
   sibling prose restates a behavior under its handle rather than the diff's
   identifiers. Follow every link anchor, source link and embedded image
   (including an SVG's `<text>`) that a behavioral passage the diff adds or
   changes points to. Reconcile each hit and target, or record in the body
   why it still holds. For each `*.md` or `*.md#anchor` reference the diff
   adds or changes in a code comment, string literal or test message, open
   the cited section and correct the citation or the claim unless it states
   the attributed fact.
10. Push only your branch and open a **draft** pull request that follows the
    profile's `pr_template`. Report exact commands and results, skipped
    checks and limitations. If the forge refuses because a pull request this
    job did not open already uses the branch, stop with a `blocked` status,
    `reason=collision`; never edit, close, reopen or comment on that pull
    request.
11. Post one `factory-progress` comment on the issue.

The launcher, not the agent, reviews and merges. Never push to `main`,
approve, merge, enable auto-merge, change branch protection, deploy or touch
secrets. Stop with a `blocked` status on a missing prerequisite, a question
outside the approved scope or the same failure three times.
