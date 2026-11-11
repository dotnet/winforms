"""Read and validate a repository's profile, ``.agent-factory/profile.toml``.

Each setting starts at a generic default, then takes the preset's check modes, then the profile's value, then an
override the harness passes (for example from a host variable), which ``override_lines`` reports so a forgotten one
stays visible. Reading fails closed: every unknown key, wrong type or out-of-range value is a ``path:line``
finding, and a profile with findings is not used (F5). The adopting harness describes its built-in checks, roles
and scout lenses with a ``Registry``; this module holds no repository's values.
"""

from __future__ import annotations

import re
import tomllib
from collections.abc import Mapping
from dataclasses import dataclass, field
from pathlib import Path

PATH = ".agent-factory/profile.toml"
SCHEMA = 1
PRESETS = ("prototype", "standard", "production")
DEFAULT_PRESET = "production"  # without a profile, the strictest preset applies
MODES = ("blocking", "advisory", "off")
FORGES = ("github", "ado")
POLICIES = ("constitution", "conduct", "issue_tracking", "pr_template")
# Each budget's generic default and minimum.
BUDGETS = {"max_jobs": (3, 1), "pr_job_reserve": (2, 0), "pr_backlog": (6, 1), "credits_per_session": (30000, 30),
           "continues": (40, 1), "stall_minutes": (90, 1), "open_prs_per_owner": (30, 1)}
# Budgets whose lower override throttles the host below its profile, so ``override_lines`` warns about it.
THROTTLING = frozenset({"budgets.max_jobs", "budgets.pr_backlog", "budgets.open_prs_per_owner",
                        "budgets.credits_per_session"})
# Each janitor setting's generic default and minimum; agent_factory.janitor describes the sweeps.
JANITOR = {"worktree_idle_days": (7, 1), "archive_scratch_days": (14, 1), "build_cache_gib": (40, 1)}
CUSTOM_TIMEOUT = (600, 3600)  # default and maximum seconds
OVERRIDABLE = ("models", "budgets")  # no override sets a check mode
MODEL = re.compile(r"[A-Za-z0-9][A-Za-z0-9._-]*")
SLUG = re.compile(r"[a-z0-9][a-z0-9-]*")
URL = re.compile(r"https://([A-Za-z0-9][A-Za-z0-9.-]*(?::[0-9]+)?)/([^\s?#]*[^\s?#/])")


class ProfileError(Exception):
    """A profile that cannot be used, or an override that breaks its setting's rule."""


@dataclass(frozen=True)
class Role:
    """A built-in role or scout lens: the skill its jobs follow, or ``None`` for a role that runs no agent (such as
    a deterministic check of main), the ``models`` key (``author`` or ``reviewer``) whose model they use, and, for a
    scout lens, the minutes between its runs."""

    skill: str | None
    model: str = "author"
    interval_minutes: int | None = None

    def __post_init__(self):
        if self.model not in ("author", "reviewer"):
            raise ValueError(f"model must be author or reviewer, not {self.model}")


@dataclass(frozen=True)
class Registry:
    """What the adopting harness defines: built-in checks with their mode under each preset, roles, scout lenses,
    locked keys and the default model. ``None`` accepts any check, role or scout id, for validating a profile
    without its harness."""

    checks: Mapping[str, tuple[str, str, str]] | None = None
    locked: Mapping[str, str] = field(default_factory=dict)
    forges: tuple[str, ...] = FORGES
    default_model: str | None = None
    roles: Mapping[str, Role] | None = None
    scouts: Mapping[str, Role] | None = None


def leaf(test, message: str):
    def rule(value, path, report):
        if not test(value):
            report(path, message)
    return rule


def whole(minimum: int, maximum: int | None = None):
    bounds = f"from {minimum} to {maximum}" if maximum else f"of at least {minimum}"
    # bool is an int subclass, so `true` must not pass as 1.
    return leaf(lambda v: type(v) is int and v >= minimum and (maximum is None or v <= maximum),
                f"must be a whole number {bounds}")


def one_of(options):
    return leaf(lambda v: isinstance(v, str) and v in options, f"must be one of {', '.join(options)}")


def matches(pattern: re.Pattern, message: str):
    return leaf(lambda v: isinstance(v, str) and pattern.fullmatch(v) is not None, message)


def repo_file(root: Path, value) -> bool:
    if not isinstance(value, str) or not value or Path(value).is_absolute():
        return False
    target = (root / value).resolve()
    return target.is_relative_to(root.resolve()) and target.is_file()


def command(root: Path):
    def rule(value, path, report):
        if not (isinstance(value, list) and value and all(isinstance(word, str) and word for word in value)):
            report(path, "must be a non-empty list of strings")
        elif "/" in value[0] and not repo_file(root, value[0]):
            report(path, f"{value[0]} is not a file in the repository")  # a bare name resolves on PATH when run
    return rule


def custom(item: dict, required: tuple, builtins, kind: str, locked: Mapping[str, str]):
    """An array of tables, each with a unique ``id`` that no built-in uses. A tuple in ``required`` needs one of
    its keys."""
    def rule(value, path, report):
        if not isinstance(value, list):
            report(path, "must be an array of tables")
            return
        ids = set()
        for index, entry in enumerate(value):
            name = f"{path}[{index}]"
            walk(entry, item, name, report, locked)
            if not isinstance(entry, dict):
                continue
            for keys in required:
                keys = keys if isinstance(keys, tuple) else (keys,)
                if not any(key in entry for key in keys):
                    report(name, f"{' or '.join(keys)} is required")
            ident = entry.get("id")
            if isinstance(ident, str):
                if ident in (builtins or ()) or ident in ids:
                    report(f"{name}.id", f"must differ from every built-in and other custom {kind}")
                ids.add(ident)
    return rule


def rules(root: Path, registry: Registry) -> dict:
    """Each key's rule; a ``*`` entry accepts any lowercase id."""
    model = matches(MODEL, "must be one model name")
    ident = matches(SLUG, "must be lowercase letters, digits and hyphens")
    in_repository = leaf(lambda v: repo_file(root, v), "must name a file in the repository")
    mode = {"mode": one_of(MODES)}
    check = {"id": ident, "run": command(root), "mode": one_of(MODES), "timeout": whole(1, CUSTOM_TIMEOUT[1])}
    role = {"enabled": leaf(lambda v: type(v) is bool, "must be true or false"), "skill": ident,
            "instructions": in_repository, "model": model, "credits": whole(BUDGETS["credits_per_session"][1])}
    scout = {**role, "interval_minutes": whole(1)}

    def known(builtins, rule):
        return {"*": rule} if builtins is None else dict.fromkeys(builtins, rule)

    return {
        "schema": leaf(lambda v: True, ""),
        "preset": one_of(PRESETS),
        "repository": {"forge": one_of(registry.forges),
                       "url": matches(URL, "must be an https URL of the repository")},
        "policies": dict.fromkeys(POLICIES, in_repository),
        "models": {"author": model, "reviewer": model},
        "budgets": {key: whole(minimum) for key, (_, minimum) in BUDGETS.items()},
        "checks": {**known(registry.checks, mode),
                   "custom": custom(check, ("id", "run"), registry.checks, "check", registry.locked)},
        "roles": known(registry.roles, role),
        "scouts": {**known(registry.scouts, scout),
                   "custom": custom({"id": ident, **scout}, ("id", ("skill", "instructions"), "interval_minutes"),
                                    registry.scouts, "scout", registry.locked)},
        "janitor": {key: whole(minimum) for key, (_, minimum) in JANITOR.items()},
    }


def walk(value, rule, path: str, report, locked: Mapping[str, str]) -> None:
    if not isinstance(rule, dict):
        rule(value, path, report)
        return
    if not isinstance(value, dict):
        report(path, "must be a table")
        return
    for key, item in value.items():
        name = f"{path}.{key}" if path else str(key)
        if name in locked:
            report(name, f"sets locked invariant {locked[name]}")
        elif key in rule:
            walk(item, rule[key], name, report, locked)
        elif "*" in rule and SLUG.fullmatch(key):
            walk(item, rule["*"], name, report, locked)
        else:
            report(name, "unknown key")


PART = r"""(?:[A-Za-z0-9_-]+|"[^"]*"|'[^']*')"""
ARRAY = re.compile(rf"\[\[\s*({PART}(?:\s*\.\s*{PART})*)\s*\]\]\s*(?:#.*)?")
TABLE = re.compile(rf"\[\s*({PART}(?:\s*\.\s*{PART})*)\s*\]\s*(?:#.*)?")
KEY = re.compile(rf"({PART}(?:\s*\.\s*{PART})*)\s*=")


def dotted(name: str) -> str:
    return ".".join(part.strip("\"'") for part in re.findall(PART, name))


def mark(lines: dict[str, int], path: str, number: int) -> None:
    """Record ``number`` for ``path`` and each parent table that it defines, unless an earlier line did."""
    for end in [*(found.start() for found in re.finditer(r"[.\[]", path)), len(path)]:
        lines.setdefault(path[:end], number)


def key_lines(text: str) -> dict[str, int]:
    """The first line of each table, array element and key, and of each parent table that a dotted name defines
    implicitly. The scan is line-based, so a key inside a value that spans lines may be missed, leaving findings on
    its table's line."""
    lines: dict[str, int] = {}
    counts: dict[str, int] = {}
    table = ""
    for number, line in enumerate(text.splitlines(), 1):
        line = line.strip()
        if found := ARRAY.fullmatch(line):
            name = dotted(found.group(1))
            counts[name] = counts.get(name, -1) + 1
            table = f"{name}[{counts[name]}]"
        elif found := TABLE.fullmatch(line):
            table = dotted(found.group(1))
        elif found := KEY.match(line):
            key = dotted(found.group(1))
            mark(lines, f"{table}.{key}" if table else key, number)
            continue
        else:
            continue
        mark(lines, table, number)
    return lines


def line_of(lines: Mapping[str, int], path: str) -> int:
    """The line of ``path``, else of its nearest recorded parent, else 1."""
    while path not in lines:
        if not path:
            return 1
        path = path[:max(path.rfind("."), path.rfind("["), 0)]
    return lines[path]


def remote_url(remote: str | None) -> str | None:
    """The https URL of a git remote in https, ssh or scp form, or None for anything else."""
    remote = (remote or "").strip()
    found = (re.fullmatch(r"(?:https|ssh)://(?:[^@/\s]+@)?([^/:\s]+)(?::[0-9]+)?/(\S+?)(?:\.git)?/?", remote)
             or re.fullmatch(r"[^@/\s]+@([^/:\s]+):(?!/)(\S+?)(?:\.git)?/?", remote))
    if not found:
        return None
    host, path = found.groups()
    if host == "ssh.dev.azure.com" and path.startswith("v3/") and path.count("/") == 3:
        organization, project, name = path[3:].split("/")
        return f"https://dev.azure.com/{organization}/{project}/_git/{name}"
    return f"https://{host}/{path}"


def forge_of(url: str) -> str:
    found = URL.fullmatch(url)
    host = found.group(1).split(":")[0] if found else ""
    return "ado" if host == "dev.azure.com" or host.endswith(".visualstudio.com") else "github"


def read(root: Path, registry: Registry) -> tuple[dict, list[str]]:
    """The profile's settings and its findings; without a profile, no settings and no findings."""
    file = root / PATH
    if not file.is_file():
        return {}, []
    found: list[str] = []
    lines: dict[str, int] = {}

    def report(path, message, line=None):
        line = line or line_of(lines, path)
        found.append(f"{PATH}:{line} - {path}: {message}" if path else f"{PATH}:{line} - {message}")

    try:
        text = file.read_text(encoding="utf-8")
        data = tomllib.loads(text)
    except UnicodeDecodeError:
        report("", "must be UTF-8 text")
        return {}, found
    except tomllib.TOMLDecodeError as error:
        position = re.search(r"\(at line (\d+)", str(error))
        report("", str(error), int(position.group(1)) if position else 1)
        return {}, found
    lines.update(key_lines(text))
    schema = data.get("schema", SCHEMA)
    if type(schema) is not int or schema != SCHEMA:
        report("schema", f"unknown schema; this reader reads schema {SCHEMA}")
        return {}, found  # another schema's keys are not this schema's mistakes
    for key in ("schema", "preset"):
        if key not in data:
            report(key, "is required")
    walk(data, rules(root, registry), "", report, registry.locked)
    repository = data.get("repository")
    url = repository.get("url") if isinstance(repository, dict) else None
    shape = URL.fullmatch(url) if isinstance(url, str) else None
    forge = repository.get("forge", forge_of(url)) if shape else None
    if shape and "forge" not in repository and forge not in registry.forges:
        supported = ", ".join(registry.forges)
        report("repository.url", f"names a repository on {forge}; the forge must be one of {supported}")
    if forge == "github" and shape.group(2).count("/") != 1:
        report("repository.url", "must be https://host/owner/name for a github repository")
    return data, found


def effective(root: Path, data: dict, registry: Registry, remote: str | None = None,
              overrides: Mapping[str, tuple[str, str]] | None = None) -> dict:
    """Defaults, then the preset's check modes, then ``data``, then ``overrides``.

    ``data`` must have no findings. Without a profile URL, the repository comes from the ``remote`` git URL.
    ``overrides`` maps a ``models`` or ``budgets`` key to its source's name and raw value; each value meets the
    profile key's rule, so no layer reaches a value the profile may not set.
    """
    preset = data.get("preset", DEFAULT_PRESET)
    column = PRESETS.index(preset)
    result = {
        "schema": SCHEMA,
        "preset": preset,
        "repository": {"forge": None, "url": remote_url(remote)},
        "policies": dict.fromkeys(POLICIES),
        "models": {"author": registry.default_model, "reviewer": registry.default_model},
        "budgets": {key: default for key, (default, _) in BUDGETS.items()},
        "checks": {**{check: {"mode": modes[column]} for check, modes in (registry.checks or {}).items()},
                   "custom": []},
        "roles": dict.fromkeys(registry.roles or {}),
        "scouts": {**dict.fromkeys(registry.scouts or {}), "custom": []},
        "janitor": {key: default for key, (default, _) in JANITOR.items()},
    }
    overlay(result, data)
    repository = result["repository"]
    if repository["url"] and not repository["forge"]:
        repository["forge"] = forge_of(repository["url"])
    result["checks"]["custom"] = [
        {"id": check["id"], "run": check["run"], "mode": check.get("mode", "advisory"),
         "timeout": check.get("timeout", CUSTOM_TIMEOUT[0])} for check in result["checks"]["custom"]]
    table = rules(root, registry)
    for key, (name, raw) in (overrides or {}).items():
        group, _, setting = key.partition(".")
        if group not in OVERRIDABLE or setting not in table[group]:
            raise ProfileError(f"{name} cannot set {key}")
        value: str | int = raw
        if group == "budgets":
            try:
                value = int(raw)
            except ValueError:
                raise ProfileError(f"{name} must be a whole number") from None
        problems: list[str] = []
        table[group][setting](value, name, lambda _, message: problems.append(message))
        if problems:
            raise ProfileError(f"{name} {problems[0]}")
        result[group][setting] = value
    # Role and scout defaults follow the final models and budgets, so an override reaches them too.
    custom_scouts = result["scouts"].pop("custom")
    for group, builtins in (("roles", registry.roles), ("scouts", registry.scouts)):
        result[group] = {ident: job(result, (builtins or {}).get(ident), settings or {}, group == "scouts")
                         for ident, settings in result[group].items()}
    result["scouts"]["custom"] = [{"id": scout["id"], **job(result, None, scout, True)} for scout in custom_scouts]
    return result


def job(result: dict, role: Role | None, settings: Mapping, scout: bool) -> dict:
    """A role's or scout's settings over its built-in values and the final models and budgets."""
    values = {"enabled": True, "skill": role.skill if role else None, "instructions": None,
              "model": result["models"][role.model if role else "author"],
              "credits": result["budgets"]["credits_per_session"]}
    if scout:
        values["interval_minutes"] = role.interval_minutes if role else None
    return {key: settings.get(key, default) for key, default in values.items()}


def override_lines(root: Path, data: dict, registry: Registry,
                   overrides: Mapping[str, tuple[str, str]] | None = None) -> list[str]:
    """One line per override, given as ``effective`` takes them: ``<level> NAME=value overrides key=replaced
    (source)``, where the source of the replaced value is the profile file or the generic default.

    The level is ``WARN`` when the override lowers a ``THROTTLING`` budget, since a forgotten one silently throttles
    the host below its profile, else ``note``. A value that is not a whole number is never stricter.
    """
    base = effective(root, data, registry)
    lines = []
    for key, (name, raw) in (overrides or {}).items():
        group, _, setting = key.partition(".")
        if group not in OVERRIDABLE or setting not in base[group]:
            raise ProfileError(f"{name} cannot set {key}")
        replaced = base[group][setting]
        source = PATH if setting in (data.get(group) or {}) else "generic default"
        try:
            stricter = key in THROTTLING and int(raw) < replaced
        except ValueError:
            stricter = False
        lines.append(f"{'WARN' if stricter else 'note'} {name}={raw} overrides {key}={replaced} ({source})")
    return lines


def overlay(base: dict, data: Mapping) -> None:
    for key, value in data.items():
        if isinstance(value, Mapping) and isinstance(base.get(key), dict):
            overlay(base[key], value)
        else:
            base[key] = value


def valid(root: Path, registry: Registry) -> dict:
    """The profile's settings, or a ProfileError listing every finding."""
    data, found = read(root, registry)
    if found:
        raise ProfileError(f"{PATH} is invalid:\n" + "\n".join(found))
    return data


def dumps(data: Mapping) -> str:
    """TOML for tables, arrays of tables, strings, whole numbers, booleans and lists; ``None`` values are omitted.

    The standard library reads TOML but does not write it.
    """
    return _dump(data, "").lstrip("\n")


def _dump(data: Mapping, prefix: str) -> str:
    keys, tables = [], []
    for key, value in data.items():
        name = f"{prefix}.{_key(key)}" if prefix else _key(key)
        if value is None:
            continue
        if isinstance(value, Mapping):
            tables.append(f"\n[{name}]\n" + _dump(value, name))
        elif isinstance(value, list) and value and all(isinstance(item, Mapping) for item in value):
            tables.extend(f"\n[[{name}]]\n" + _dump(item, name) for item in value)
        else:
            keys.append(f"{_key(key)} = {_value(value)}\n")
    return "".join(keys) + "".join(tables)


def _key(key: str) -> str:
    return key if re.fullmatch(r"[A-Za-z0-9_-]+", key) else _string(key)


def _value(value) -> str:
    if isinstance(value, bool):
        return "true" if value else "false"
    if isinstance(value, int):
        return str(value)
    if isinstance(value, str):
        return _string(value)
    if isinstance(value, list):
        return "[" + ", ".join(_value(item) for item in value) + "]"
    raise TypeError(f"cannot write {type(value).__name__} as TOML")


def _string(value: str) -> str:
    escaped = "".join(f"\\{char}" if char in '"\\' else f"\\u{ord(char):04x}" if char < " " or char == "\x7f"
                      else char for char in value)
    return f'"{escaped}"'
