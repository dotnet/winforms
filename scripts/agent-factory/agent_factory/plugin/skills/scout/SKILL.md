---
name: scout
description: 'Find small, concrete, high-confidence work through one lens (security, pentest, bugs, triage, compactor, quality, hygiene, dreamer, rsi, docs, language or a repository-defined lens) and file deduplicated agent-ready issues within a per-run cap.'
---

# Scout for Work

Follow the [factory-coordination skill](../factory-coordination/SKILL.md)
and the profile's `issue_tracking` policy. The launcher supplies the lens,
the per-run issue cap, the queue cap and, when known, the `main` commit the
previous scout of this lens reviewed. A lens the repository defines names
its own skill or instructions; follow them in step 2.

The launcher advances a lens's commit cursor only when its run exits
successfully, to the last reviewed commit the run recorded or otherwise to
the run's launch commit, so a failed or stopped run never skips commits. If
you stop at the issue cap, or early for any other reason, before reviewing
every commit since the previous one, write the full SHA of the last commit
you fully reviewed to the path your prompt names. The launcher reads a
missing file as a run that covered its whole range, and keeps the earlier
cursor when the recorded commit is not an ancestor of the launch commit.

1. Do not recount the `agent-ready` queue: the launcher applies the queue
   cap before it starts a scout (the security, pentest and rsi lenses are
   exempt), so a launched scout continues. The compactor lens never files
   issues.
2. Inspect `main` through the lens:
   - **triage:** classify only the issues the launcher names. Read each with
     its comments and skip one that is no longer open and unassigned, is now
     `agent-ready` or `epic`, or was already answered. Apply exactly one
     decision to each of the rest and post one `factory-triage` comment:
     - `ready`: approved, small and clear. Add `agent-ready`, and an
       `owner:<author>` label when none exists.
     - `split`: approved but too large. Split it with the
       [work-queue skill](../work-queue/SKILL.md).
     - `needs-human`: needs a product, risk or architecture decision, lacks
       acceptance criteria or looks obsolete. Add `needs-human`. Close a
       duplicate as not planned with `Duplicate of #N`, keeping the older or
       further advanced issue.
   - **compactor:** propose closing open issues that clearly duplicate
     another, that `main` or a merged pull request already completed, or
     that a later decision made obsolete. Cite the canonical issue, pull
     request, commit or `path:line`, and keep the older issue. Before
     proposing a closure:
     - Confirm completion on `main`, not from a pull request title. Search
       `main` for the symbol, path or quoted text the issue names, and cite
       each acceptance criterion met at `path:line`. Work on an unmerged
       branch or in a pull request closed unmerged does not count, and stale
       text the issue quotes that is still on `main` keeps it open.
     - Map renames first, using the repository's decision register when it
       keeps one. A name or path that moved does not make an issue obsolete;
       re-check the ask under its current name.
     - Propose `obsolete` only by citing the decision or deletion that
       removes the ask. Work that an approved decision or specification
       requires but `main` has not built yet is open work, not obsolete.
     - Leave open partly done work, work whose code is merged but whose
       acceptance still needs a live or owner check the files cannot show,
       and an issue whose truncated snapshot hides part of its ask.
     Write only the proposal file the launcher names; the launcher re-checks
     live state and closes. When unsure, leave the issue open.
   - **bugs:** defects, unhandled errors, missing validation, missing
     regression tests and stale TODOs with a clear fix. Prefer code changed
     since the previous commit.
   - **quality:** behavior-preserving improvements that existing tests pin:
     duplicated logic, overgrown functions or modules, unclear names,
     needless abstraction, and missing types or validation that would make a
     defect impossible. Propose at most one module split or dependency cycle
     per issue, naming the responsibilities to separate; production modules
     come before test files. File size alone is never blocking.
   - **hygiene:** compiler, analyzer and formatter warnings; dead code and
     unused dependencies; floating or unpinned versions; tests missing from
     the repository's test inventory; stale generated files; and secrets or
     customer data in logs.
   - **dreamer:** decisions, approaches and facts that a later decision
     superseded or reversed but that active guidance still presents as
     current. Keep one line of history and point to the current source.
   - **rsi:** factory efficiency. From the launcher's metrics and previous
     report, find why pull requests need several review or ship rounds, or
     why jobs end without moving one forward, for example finished work
     discarded on a transient failure, a review run without its check
     results, a re-review that repeats the whole review instead of what
     changed, one shared file causing most merge conflicts, or a stage that
     fails without keeping its output. For each recurring class, name the
     skill, instruction or deterministic check that should have caught it
     earlier and the gap that let it through, preferring an earlier check
     over another review round, and file an issue that changes it, with a
     `Metric this should move:` line that the next rsi run reports on. When
     the metrics count forge API calls by call site, compare them with the
     hourly quota all hosts of the account share, and file issues for
     per-item reads repeated every pass, re-reads of a list already read,
     full-history reads where a bounded read suffices and missing
     conditional requests; never propose dropping the fresh read before a
     mutation. When the inputs cannot separate a category, file an issue to
     measure it. Never propose weakening review, tests, human approval or
     security gates.
   - **docs:** broken links, stale status, and documentation that
     contradicts source or states a fact in more than one place.
   - **language:** spelling, grammar and unclear wording in documentation,
     comments, error messages and command help. Never change public API
     names, persisted strings or quoted input.
   - **security:** first file the forge's security alerts the launcher
     lists, grouped by package or rule, with the patched version and every
     affected manifest and lockfile. Without a patched version, the fix
     removes the dependency or switches to an approved one the repository
     already uses. A leaked secret, a breaking major upgrade or a
     replacement that changes behavior or persisted formats gets
     `needs-human` instead of `agent-ready`. Then read every change since
     the previous commit and trace each untrusted input to its effect,
     checking authentication, isolation between customers or tenants,
     injection, request forgery and credential forwarding, unsafe
     deserialization, unbounded input, replay, privilege escalation, secret
     exposure and insecure defaults. Never copy a secret value.
   - **pentest:** a static penetration test of the public API. Map every
     route with its authentication, authorization and gateway policy, attack
     each against the [OWASP API Security Top 10](https://owasp.org/API-Security/),
     and trace each attack through the code. Never start a host or send a
     network request. Each issue's acceptance criteria include an abuse-case
     test that fails before the fix.
3. Keep only high-confidence findings with `path:line` evidence that fit one
   reviewable pull request with a test or check. Skip anything that needs a
   product, risk or architecture decision or that a design conflict the
   repository records covers, except a confirmed exploitable weakness: file
   it with `needs-human` instead of `agent-ready`, stating the decision it
   needs. A `needs-human` finding keeps `factory-scout`, the owner label and
   the cap. Never turn an unapproved requirement or an already-tracked
   conflict into a new defect. Describe the weakness and the fix, never a
   working exploit.
4. Give each finding a stable key such as `bugs-retry-bound`. Skip keys that
   `<!-- factory-source key=KEY -->` already marks on an open or closed
   issue, and findings an open pull request or `factory/*` branch already
   covers, then apply the factory-coordination check before filing.
5. File at most the cap, labeled `agent-ready`, `factory-scout` and the owner
   label the launcher names, titled `[LENS] short imperative summary`. Start
   the body with the marker, then give the evidence, expected behavior, fix
   scope, acceptance criteria one pull request can verify and validation
   commands, then any `Metric this should move:` line (never an acceptance
   criterion), `Depends on #N` for each issue the fix must follow, `Epic: #N`
   and the launcher's `## Provenance` section.

Scouts never edit repository files, commit, push, comment on pull requests,
close issues or change labels, except that triage labels, comments on and
closes as duplicate the issues it was given. They write only the local files
their prompt names, such as the compactor's proposal or the last reviewed
commit. Filing nothing is a valid result; report what was checked.
