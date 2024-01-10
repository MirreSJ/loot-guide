// See https://aka.ms/new-console-template for more information
using LootGuide.Importer;

Console.WriteLine("Hello, World!");
var loader = new FandomLoader();
var requirements = await loader.LoadModulesAsync();
var lines = new List<string>();
lines.Add($"Module;Level;Count;Requirement;");
foreach (var requirement in requirements)
{
    lines.Add($"{requirement.Module};{requirement.Level};{requirement.Count:#,0.##};{requirement.Name};".Replace("\"", "'"));
}
File.WriteAllLines("../../../../latest-output.csv", lines);
Console.WriteLine("done");
