# Agent factory: architecture and setup

[Tier 1 adoption](adoption.md) | [Harness design](https://msft.ghe.com/coreai/agent-factory/blob/main/docs/design/reusable-harness.md) | [Firefly's live factory](https://msft.ghe.com/coreai/firefly/blob/main/docs/factory/how-it-works.md)

This document explains the agent-factory architecture end to end, what this
repository has actually adopted so far (Tier 1), and the exact commands to
install the prerequisites and validate the configuration.

## 1. What agent-factory is

A reusable harness for running a repository in an agentic delivery loop:
**discover work → claim it → run Copilot CLI sessions → review independently
→ gate → merge → clean up.** It targets GitHub and Azure DevOps repositories.
Each repository supplies its own policy; the harness supplies the generic
engine, roles and skills.

Source: `https://msft.ghe.com/coreai/agent-factory`.

## 2. The three layers of the architecture

```text
             repository (consumer, e.g. this one)                 agent-factory (harness repo)
┌───────────────────────────────────────────┐      ┌────────────────────────────────────────────────┐
│ .agent-factory/profile.toml   (policy)    │      │ agent_factory.profile   schema, validation      │
│ .agent-factory/harness.lock   (pin)       │─────▶│ agent_factory.roles     built-in roles/scouts   │
│ scripts/agent-factory/        (vendored)  │      │ agent_factory.janitor   disk sweeps             │
│ docs/factory/                 (this doc)  │      │ agent_factory/plugin/   Copilot CLI skills      │
└───────────────────────────────────────────┘      └────────────────────────────────────────────────┘
                                                      core engine (claim/build/review/merge loop):
                                                      designed in docs/design/reusable-harness.md,
                                                      NOT YET published as a reusable package
```

| Layer | Status | What it does |
|---|---|---|
| **Profile schema** (`agent_factory.profile`) | Shipped | Reads `.agent-factory/profile.toml`, validates every key, layers generic defaults → preset → profile value → host override, fails closed on anything unknown |
| **Roles & scouts registry** (`agent_factory.roles`) | Shipped | Declares the built-in job roles (`fix`, `ship`, `review`, `patrol`, `main-check`, `janitor`) and scout lenses (`security`, `bugs`, `triage`, `docs`, ...), each mapped to a skill |
| **Janitor** (`agent_factory.janitor`) | Shipped | Disk-hygiene sweeps (archives, idle worktrees, build cache, stale images) — a library a harness/launcher calls, not a service itself |
| **Copilot CLI skills** (`agent_factory/plugin/skills/*/SKILL.md`) | Shipped | The actual prompts: `fix-issue`, `ship-it`, `code-review`, `patrol`, `scout`, `factory-coordination`, `work-queue`, `overcooked-kitchen` |
| **Core engine** (claim → build → review → gate → merge loop, forge adapters for GitHub/ADO) | **Design only** (`docs/design/reusable-harness.md`, Phases P2–P4) | Not implemented as a reusable package. [Firefly](https://msft.ghe.com/coreai/firefly) runs its own ~55,000-line hand-built implementation of this loop; it is not what `pip install agent-factory` gives you |

## 3. The 12 safety floors (design, not yet enforced by code here)

Regardless of profile settings, the harness's design fixes these invariants
(a profile may tighten, never loosen them): exact-head merge only (F1),
independent review by a different model (F2), trusted base-branch policy
reads (F3), fail-closed on unknown config (F5), credit/continuation/wall-clock
caps (F6), model-usage audits (F7), a minimum tool-deny list (F8), credential
separation (F9), write-ahead intent logs before any forge write (F10),
remote advisories that can only remove permission (F11), and survival of host
sleep/crash (F12). These are enforced by the (not-yet-shipped) core engine,
not by the profile reader alone.

## 4. The end-to-end runtime flow (how it works where it IS running, e.g. Firefly)

```mermaid
sequenceDiagram
    autonumber
    participant L as Launcher (tmux loop on a dev box)
    participant F as Forge (GitHub/ADO)
    participant W as Worker session (Copilot CLI)
    participant R as Reviewer session
    Note over L: A person runs `factory.py begin` once;<br/>opens tmux windows loop-cycle (5m), loop-main-check (10m), loop-doctor (2m, watchdog)
    loop every cycle (5 min)
        L->>F: merge PRs that pass every gate
        L->>F: review new/updated PR heads (fresh session, independent model)
        L->>F: resend changes-requested PRs to a ship/repair round
        L->>F: claim up to max_jobs queued issues
        L->>W: run fix-issue skill in a fresh git worktree, bounded by credit/time caps
        W-->>L: commits + draft PR
        L->>F: launch due scout lenses (read-only, file new issues only)
    end
    Note over L: Between full cycles, a 1-minute poll merges PRs whose review just finished
```

Triggering is **not** webhook/event driven — it is a self-sustaining polling
loop kept alive by a watchdog (`loop-doctor`), started once by a human and
surviving dev-box sleep/hibernation by reconciling state on wake. Merging
itself is always deterministic launcher code, never a model decision.

## 5. What this repository has adopted: Tier 1 only

| Adopted | Not adopted (yet) |
|---|---|
| `.agent-factory/profile.toml` — validated policy (preset, budgets, check modes, policy file paths) | The core engine / launcher (§2, §4) — nothing claims issues, reviews PRs or merges automatically |
| `.agent-factory/harness.lock` — pins the exact vendored commit + sha256 | A merge policy file (`factory-merge-policy.json`), sensitive-path list, placement file |
| `scripts/agent-factory/agent_factory/` — vendored `profile.py`/`roles.py`/`janitor.py` + the full skill library | `policies.constitution` / `policies.conduct` (no such docs exist in this repo yet) |
| This document + [`docs/factory/adoption.md`](adoption.md) | Full claim/discover/merge automation — only the single-issue [thin wrapper](thin-wrapper.md) exists |
| [`.github/workflows/copilot-fix-issue.yml`](../../.github/workflows/copilot-fix-issue.yml) ([docs](thin-wrapper.md)) — one issue in, one draft PR out, human merges | Any multi-host coordination, claiming or scheduling |

See [`adoption.md`](adoption.md) for the full rationale and file-by-file breakdown.

## 6. Prerequisites: exact install commands

Run from a machine with internet/VPN access to `msft.ghe.com` and GitHub.

### 6.1 Python 3.11+

Check first:

```powershell
python --version
```

If missing (Windows, via winget):

```powershell
winget install --id Python.Python.3.12 -e --accept-source-agreements --accept-package-agreements
```

Linux/macOS: install Python 3.11+ via your usual package manager (`apt
install python3.11`, `brew install python@3.12`, etc.) — the harness needs
only the standard library (`tomllib`).

Restart the shell (or refresh `PATH`) after install so `python`/`pip` resolve:

```powershell
$env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")
```

### 6.2 The `agent-factory` package

There is no published package feed yet (open decision D1 in the harness
design doc), so install directly from the pinned commit this repository's
`.agent-factory/harness.lock` records:

```powershell
pip install "git+https://msft.ghe.com/coreai/agent-factory@110c979461bf6d70867c85f3792740f8f4d748a9"
```

This requires `gh auth login -h msft.ghe.com` (or another credential helper
that can authenticate git over HTTPS to `msft.ghe.com`) beforehand.

Alternative if you already have a local clone of the harness repo checked out
at that commit:

```powershell
pip install path\to\your\agent-factory-clone
```

Verify:

```powershell
agent-factory --help
```

## 7. Exact validation commands

Run from this repository's root (`Q:\g\winforms`, or your checkout's root):

```powershell
agent-factory profile validate --root .
```

Expected output: `.agent-factory/profile.toml is valid` (exit code `0`). Any
problem is reported as `.agent-factory/profile.toml:LINE - key: message` and
exits non-zero.

To inspect the fully resolved configuration (defaults + preset + profile
merged):

```powershell
agent-factory profile show --root .
```

This prints the effective settings as JSON: repository identity, policy
paths, budgets, check modes, and every built-in role/scout with its skill,
model and credit defaults.

## 8. Re-validating after any change

Any edit to `.agent-factory/profile.toml` should be followed by:

```powershell
agent-factory profile validate --root .
agent-factory profile show --root .
```

Any re-vendoring of `scripts/agent-factory/agent_factory/` (on a harness
update) should update `.agent-factory/harness.lock`'s `commit` and `sha256`
fields and be reviewed like any other change under `.agent-factory/` or
`scripts/agent-factory/`.
