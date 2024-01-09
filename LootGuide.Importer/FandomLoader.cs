using Flurl.Http;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootGuide.Importer
{
    public class FandomLoader
    {
        private const string _fandomUrl = "https://escapefromtarkov.fandom.com/wiki/Hideout";
        public async Task<IEnumerable<Requirement>> LoadModulesAsync()
        {
            var result = new List<Requirement>();
            var html = await "https://escapefromtarkov.fandom.com/wiki/Hideout".GetStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//table[contains(@class, 'wikitable')]"))
            {
                var moduleName = GetModuleName(table);
                if(moduleName != null)
                {
                    var requirements = CreateLevels(table, moduleName);
                    result.AddRange(requirements);
                }                
            }

            return result;
        }

        private IEnumerable<Requirement> CreateLevels(HtmlNode table, string module)
        {
            var result = new List<Requirement>();
            var rows = table.SelectNodes("tbody/tr");
            for (int i = 2; i < rows.Count; i++)
            {
                var row = rows[i];
                var stageTxt = row.SelectSingleNode("th");
                int stage;
                if (int.TryParse(stageTxt.InnerText, out stage))
                {
                    var requirementList = row.SelectNodes("td/ul/li");
                    var requirements = CreateRequirements(requirementList, stage, module);
                    result.AddRange(requirements);
                }
            }

            return result;
        }

        private IEnumerable<Requirement> CreateRequirements(HtmlNodeCollection requirementList, int level, string module)
        {
            var result = new List<Requirement>();
            foreach (var requirementNode in requirementList)
            {
                var requirement = new Requirement
                {
                    Module = module.Trim(),
                    Level = level,
                    Count = 1,
                    Name = requirementNode.InnerText.Trim()
                };
                result.Add(requirement);
            }
            return result;
        }

        private string? GetModuleName(HtmlNode table)
        {
            var header = table.SelectSingleNode("tbody/tr/th[@colspan='4']");
            if (header == null)
            {
                return null;
            }
            return header.InnerText;
        }
    }
}
