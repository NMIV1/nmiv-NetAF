"""
generate_nodemap.py – Generate DOT/SVG dependency graph from NodeMapState schema JSON.

Workflow:
  1. Build NetAF.MyGame and run a small inline exporter to produce nodemap.schema.json.
  2. Read the JSON and generate nodemap.dot (always) and nodemap.svg (if Graphviz is present).

Usage (manual / CI):
  python tools/nodemap/generate_nodemap.py
"""

import json
import os
import shutil
import subprocess
import sys
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[2]
GAME_PROJECT = REPO_ROOT / "NetAF.Game" / "NetAF.MyGame.csproj"
OUT_DIR = REPO_ROOT / "tools" / "nodemap" / "out"
SCHEMA_PATH = OUT_DIR / "nodemap.schema.json"
DOT_PATH = OUT_DIR / "nodemap.dot"
SVG_PATH = OUT_DIR / "nodemap.svg"

# ---------------------------------------------------------------------------
# Step 1 – export schema JSON from NodeMapState via a small dotnet-script
# ---------------------------------------------------------------------------

EXPORT_SCRIPT = r"""
using NetAF.MyGame;
using System.Text.Json;
using System.IO;
using System.Linq;

var outDir = args.Length > 0 ? args[0] : ".";
Directory.CreateDirectory(outDir);

var schema = NodeMapState.GetRuleSchema();

var doc = new {
    nodes = new object[] {
        new { id = NodeMapState.ObjAFound,            kind = "base" },
        new { id = NodeMapState.ObjBFound,            kind = "base" },
        new { id = NodeMapState.ObjCFound,            kind = "base" },
        new { id = NodeMapState.ObjDFound,            kind = "base" },
        new { id = NodeMapState.ObjEFound,            kind = "base" },
        new { id = NodeMapState.HasEvidenceHardDrive, kind = "base" },
        new { id = NodeMapState.HasEvidenceVR,        kind = "base" },
        new { id = NodeMapState.HasEvidenceShoes,     kind = "base" },
        new { id = NodeMapState.HasEvidenceReceipts,  kind = "base" },
        new { id = NodeMapState.HasEvidenceServer,    kind = "base" },
        new { id = NodeMapState.ConnLexicon,          kind = "derived" },
        new { id = NodeMapState.ConnHaunting,         kind = "derived" },
        new { id = NodeMapState.ConnPhantomDate,      kind = "derived" },
    },
    rules = schema.Select(kvp => new { target = kvp.Key, requires = kvp.Value }).ToArray()
};

var json = JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
File.WriteAllText(Path.Combine(outDir, "nodemap.schema.json"), json);
Console.WriteLine($"Wrote {Path.Combine(outDir, "nodemap.schema.json")}");
"""


def export_schema_via_dotnet() -> bool:
    """Build the game project and run the inline export script."""
    script_dir = OUT_DIR / "_export_tmp"
    script_dir.mkdir(parents=True, exist_ok=True)

    script_file = script_dir / "export.csx"
    script_file.write_text(EXPORT_SCRIPT, encoding="utf-8")

    # Use dotnet-script or fallback to a temp console project
    if shutil.which("dotnet-script"):
        cmd = ["dotnet-script", str(script_file), "--", str(OUT_DIR)]
    else:
        # Build a minimal console project inline
        proj_file = script_dir / "ExportNodeMap.csproj"
        cs_file = script_dir / "Program.cs"
        proj_file.write_text(
            f"""<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="{GAME_PROJECT}" />
  </ItemGroup>
</Project>
""",
            encoding="utf-8",
        )
        cs_file.write_text(EXPORT_SCRIPT, encoding="utf-8")
        cmd = [
            "dotnet",
            "run",
            "--project",
            str(proj_file),
            "--",
            str(OUT_DIR),
        ]

    try:
        result = subprocess.run(cmd, capture_output=True, text=True, cwd=str(REPO_ROOT))
        if result.returncode != 0:
            print(f"[export] STDOUT: {result.stdout}", file=sys.stderr)
            print(f"[export] STDERR: {result.stderr}", file=sys.stderr)
            return False
        print(result.stdout.strip())
        return True
    except Exception as exc:
        print(f"[export] Exception: {exc}", file=sys.stderr)
        return False


# ---------------------------------------------------------------------------
# Step 2 – read JSON schema and write DOT
# ---------------------------------------------------------------------------

def load_schema(path: Path) -> dict:
    with open(path, encoding="utf-8") as f:
        return json.load(f)


def write_dot(schema: dict) -> None:
    nodes: list[dict] = schema.get("nodes", [])
    rules: list[dict] = schema.get("rules", [])

    base_nodes = [n["id"] for n in nodes if n.get("kind") == "base"]
    derived_nodes = [n["id"] for n in nodes if n.get("kind") == "derived"]

    lines: list[str] = []
    lines.append("digraph nodemap {")
    lines.append('  rankdir=LR;')
    lines.append('  graph [fontname="Segoe UI"];')
    lines.append('  node  [fontname="Segoe UI"];')
    lines.append('  edge  [fontname="Segoe UI"];')
    lines.append("")

    # Group OBJ_* base nodes
    obj_nodes = [n for n in base_nodes if n.startswith("OBJ_")]
    evidence_nodes = [n for n in base_nodes if n.startswith("HAS_")]

    if obj_nodes:
        lines.append('  subgraph cluster_objects {')
        lines.append('    label="Discovered Objects";')
        lines.append('    color="#cccccc";')
        for node in obj_nodes:
            lines.append(f'    "{node}" [shape=box, style="rounded,filled", fillcolor="#e8f1ff"];')
        lines.append("  }")
        lines.append("")

    if evidence_nodes:
        lines.append('  subgraph cluster_evidence {')
        lines.append('    label="Evidence in Inventory";')
        lines.append('    color="#cccccc";')
        for node in evidence_nodes:
            lines.append(f'    "{node}" [shape=box, style="rounded,filled", fillcolor="#d4edda"];')
        lines.append("  }")
        lines.append("")

    if derived_nodes:
        lines.append('  subgraph cluster_connections {')
        lines.append('    label="Derived Connections";')
        lines.append('    color="#cccccc";')
        for node in derived_nodes:
            lines.append(f'    "{node}" [shape=ellipse, style="filled", fillcolor="#fff2cc"];')
        lines.append("  }")
        lines.append("")

    for rule in rules:
        target = rule["target"]
        for req in rule.get("requires", []):
            lines.append(f'  "{req}" -> "{target}";')

    lines.append("}")

    DOT_PATH.write_text("\n".join(lines) + "\n", encoding="utf-8")
    print(f"Wrote: {DOT_PATH}")


# ---------------------------------------------------------------------------
# Step 3 – render SVG (optional, requires Graphviz)
# ---------------------------------------------------------------------------

def try_render_svg() -> bool:
    dot_exe = shutil.which("dot")
    if not dot_exe:
        return False
    try:
        subprocess.run(
            [dot_exe, "-Tsvg", str(DOT_PATH), "-o", str(SVG_PATH)],
            check=True, capture_output=True, text=True,
        )
        return True
    except Exception:
        return False


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

def main() -> None:
    OUT_DIR.mkdir(parents=True, exist_ok=True)

    if not SCHEMA_PATH.exists():
        print("nodemap.schema.json not found — attempting to export from NodeMapState...")
        if not export_schema_via_dotnet():
            raise SystemExit(
                "Could not generate nodemap.schema.json. "
                "Run `dotnet build` on NetAF.MyGame first, or add NodeMapState.ExportSchemaJson() "
                "to a build step and point this script at the output."
            )

    schema = load_schema(SCHEMA_PATH)
    write_dot(schema)

    rendered = try_render_svg()
    if rendered:
        print(f"Wrote: {SVG_PATH}")
    else:
        print("SVG not generated (Graphviz 'dot' not found or failed). DOT file still produced.")


if __name__ == "__main__":
    main()
