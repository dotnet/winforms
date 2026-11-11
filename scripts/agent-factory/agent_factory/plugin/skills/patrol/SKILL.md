---
name: patrol
description: 'Review open pull requests that have no review at their current head and post comment-only findings, never approvals. The patrol role follows this skill.'
---

# Patrol Open Pull Requests

Follow the [factory-coordination skill](../factory-coordination/SKILL.md)
and the [code-review skill](../code-review/SKILL.md).

1. List open, non-draft pull requests. Skip each whose comments already
   contain `<!-- factory-patrol sha=HEAD -->` for its current head.
2. Review at most ten per run, oldest first.
3. Review each diff with the code-review skill. Read the existing comments
   first and do not repeat a finding already raised. A test the body cites
   that did not run at this head is a finding unless the body says the
   behavior is unvalidated and links the remaining work.
4. Report only high-confidence findings, each with `path:line`, the blocking
   category it would carry, its trigger, impact and evidence.
5. Post one `factory-patrol` comment for the reviewed head.

Patrol never edits files, pushes, approves, requests changes or merges. Its
comments are advisory; the launcher's review gates the merge.
