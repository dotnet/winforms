# Agent factory: Tier 1 adoption

This repository has adopted the ["Tier 1"](https://msft.ghe.com/coreai/agent-factory)
slice of the reusable agent-factory harness: a validated repository profile
and the harness's Copilot CLI skills, vendored locally. It does **not**
include the autonomous claim → build → review → merge loop that
[Firefly](https://msft.ghe.com/coreai/firefly) runs — that engine is still
being extracted from Firefly upstream (see the harness's
[`docs/design/reusable-harness.md`](https://msft.ghe.com/coreai/agent-factory/blob/main/docs/design/reusable-harness.md),
Phases P2–P4) and is not published as a reusable package yet.

## What is here

| Path | Purpose |
|---|---|
| `.agent-factory/profile.toml` | This repository's policy: preset, budgets, check modes, policy file paths. Schema defined by the vendored `agent_factory.profile` module. |
| `.agent-factory/harness.lock` | Pins the exact agent-factory commit these files were copied from, with a sha256 digest over them (same approach as Firefly's `.agent-factory/harness.lock`). |
| `scripts/agent-factory/agent_factory/` | Vendored copy of `profile.py`, `roles.py`, `janitor.py`, `__init__.py` and the `plugin/` directory (the built-in roles' Copilot CLI skills: `fix-issue`, `ship-it`, `code-review`, `patrol`, `scout`, `factory-coordination`, `work-queue`, `overcooked-kitchen`). Copied verbatim; do not hand-edit — re-vendor and update the lock instead. |

## Why vendored instead of `pip install`

The harness has no published package feed yet (its own design doc, §5.2/D1,
still lists this as an open decision). Firefly solves this by vendoring a
few files and checking their hash in a unit test; this repository does the
same rather than wait for a feed.

## Using it today

- **Validate the profile.** Install the harness locally (e.g.
  `pip install git+https://msft.ghe.com/coreai/agent-factory@110c979461bf6d70867c85f3792740f8f4d748a9`
  on a machine with Python 3.11+) and run:
  ```
  agent-factory profile validate --root .
  agent-factory profile show --root .
  ```
- **Load the skills in a Copilot CLI session.** Point `--plugin-dir` at the
  vendored plugin directory:
  ```
  copilot --plugin-dir scripts/agent-factory/agent_factory/plugin
  ```
  A skill of the same name already in `.github/skills/` (this repository has
  its own `code-review`) takes precedence over the vendored one, so only
  `fix-issue`, `ship-it`, `patrol`, `scout`, `factory-coordination` and
  `work-queue` actually load from here today.
- **Re-vendor on a harness update.** Re-copy the files listed in
  `harness.lock`, recompute its `sha256` (`"<file>\n<sha256 hex>\n"` per file,
  concatenated and re-hashed, in list order), and bump `commit`. Do this as
  its own reviewed PR, same as any other change under `.agent-factory/`.

## What is intentionally not done yet

- No claim/queue/merge automation runs against `profile.toml` — nothing in
  this repository currently starts, reviews or merges a change on its own.
  `budgets` in the profile are set conservatively (`max_jobs = 1`) for when
  that changes, not because anything enforces them today.
- `policies.constitution` and `policies.conduct` are left unset: this
  repository has no `docs/constitution.md` or equivalent "what an agent may
  and may not do" document yet. Add one and point the profile at it before
  relying on any future automation.
- No merge policy, sensitive-path list or placement file
  (`.github/factory-merge-policy.json` / `factory-placement.json` in
  Firefly) has been added, because nothing yet reads one.

Treat this as groundwork: a validated, version-controlled description of
policy and a local copy of the task skills, ready for the day the harness
ships an actual GitHub engine (see the harness's Phase P3).
