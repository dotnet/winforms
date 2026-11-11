---
name: work-queue
description: 'Split a large approved issue into deduplicated, pull-request-sized child issues labeled agent-ready, within the queue cap.'
---

# Split Work into the Queue

Follow the [factory-coordination skill](../factory-coordination/SKILL.md)
and the profile's `issue_tracking` policy.

1. If the launcher reports the `agent-ready` queue is at its cap, stop.
2. Read the parent issue and the documents it links. Skip work that needs a
   product, risk or architecture decision or that a design conflict the
   repository records covers; it stays with a person.
3. Propose children that each fit one reviewable pull request: one behavior
   with its test, the files it owns and acceptance criteria that its own
   pull request can verify. No two children change the same behavior. When
   the repository limits a pull request's changed lines, each child fits
   under it, tests included.
   Drop any child whose work already exists on `main` or in an open pull
   request.
4. Give each child a stable key, such as `parent-52-retry-bound`, and skip
   keys that `<!-- factory-source key=KEY -->` already marks on an issue.
5. Create each child with that marker, `Refs #PARENT`, its scope,
   acceptance criteria, validation commands and `Depends on #N` for each
   child it must follow, labeled `agent-ready` and with the owner label the
   launcher names. Add no other labels or assignees.
6. Post one `factory-progress` comment on the parent listing the children.

Never exceed the cap, close the parent or change its requirements.
