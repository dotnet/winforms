"""The harness's built-in roles and scout lenses, and the Copilot CLI plugin whose skills their jobs follow.

A harness with its own checks, lenses or locks builds its registry from these, for example
``dataclasses.replace(REGISTRY, scouts={**SCOUTS, "dependencies": Role("dependency-scout", interval_minutes=1440)})``.
"""

from __future__ import annotations

from pathlib import Path

from .profile import Registry, Role

PLUGIN = Path(__file__).parent / "plugin"

ROLES = {
    "fix": Role("fix-issue"),
    "ship": Role("ship-it"),
    "review": Role("code-review", "reviewer"),
    "patrol": Role("patrol", "reviewer"),
    "main-check": Role(None),
    "janitor": Role(None),
}
# Scout lenses in launch priority order, with the minutes between runs.
SCOUTS = {lens: Role("scout", interval_minutes=minutes) for lens, minutes in (
    ("security", 15), ("pentest", 1440), ("bugs", 240), ("triage", 60), ("compactor", 60), ("quality", 480),
    ("hygiene", 720), ("dreamer", 60), ("rsi", 360), ("docs", 1440), ("language", 1440))}
LOCKED = {"roles.review.enabled": "F2: every change gets an independent review"}
REGISTRY = Registry(locked=LOCKED, roles=ROLES, scouts=SCOUTS)
