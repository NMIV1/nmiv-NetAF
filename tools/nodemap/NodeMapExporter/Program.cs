using NetAF.MyGame;
using System.IO;

// Export NodeMapState schema (and optionally current state) to JSON.
// Usage: NodeMapExporter [output-directory]
//   output-directory defaults to tools/nodemap/out

var outDir = args.Length > 0 ? args[0] : Path.Combine(
    AppContext.BaseDirectory, "..", "..", "..", "tools", "nodemap", "out");

Directory.CreateDirectory(outDir);

var schemaPath = Path.Combine(outDir, "nodemap.schema.json");
NodeMapState.ExportSchemaJson(schemaPath);
Console.WriteLine($"Wrote schema: {schemaPath}");
