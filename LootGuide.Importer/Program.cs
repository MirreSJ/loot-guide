// See https://aka.ms/new-console-template for more information
using CsvHelper;
using LootGuide.Importer;
using System.Globalization;

Console.WriteLine("Hello, World!");
var loader = new FandomLoader();
var requirements = await loader.LoadModulesAsync();
var lines = new List<string>();

using (var writer = new StreamWriter("../../../../latest-output.csv"))
using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
{
    csv.WriteRecords(requirements);
}
Console.WriteLine("done");
