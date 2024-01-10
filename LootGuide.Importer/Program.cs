// See https://aka.ms/new-console-template for more information
using LootGuide.Importer;

Console.WriteLine("Hello, World!");
var loader = new FandomLoader();
var requirements = await loader.LoadModulesAsync();
Console.WriteLine($"Module;Level;Count;Requirement;");
foreach (var requirement in requirements)
{
    Console.WriteLine($"{requirement.Module};{requirement.Level};{requirement.Count:#,0.##};{requirement.Name};");
}
Console.WriteLine("done");
