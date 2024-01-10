using Flurl.Http;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                    var requirements = CreateRequirements(row, stage, module);
                    result.AddRange(requirements);
                }
            }

            return result;
        }

        private IEnumerable<Requirement> CreateRequirements(HtmlNode row, int level, string module)
        {
            var result = new List<Requirement>();
            var requirementList = row.SelectNodes("td/ul/li");
            foreach (var requirementNode in requirementList)
            {
                Requirement? requirement = CreateRequirement(level, module, requirementNode);
                if(requirement != null) 
                { 
                    result.Add(requirement);
                }
            }
            return result;
        }

        private Requirement? CreateRequirement(int level, string module, HtmlNode? requirementNode)
        {
            if (requirementNode == null)
            {
                return null;
            }
            var (count, name) = GetRequirementNameAndCount(requirementNode); 
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            return new Requirement
            {
                Module = module.Trim(),
                Level = level,
                Count = count,
                Name = name
            };
        }

        private (decimal, string?) GetRequirementNameAndCount(HtmlNode requirementNode)
        {
            var text = requirementNode.InnerText.Trim();
            if (!StratrsWithNumber(text))
            {
                return (-1, null);
            }
            if (StratrsWithNumber(text))
            {
                var match = Regex.Match(text, @"^(\d+,?)+(\.\d+)*");
                if(match.Success)
                {
                    if (decimal.TryParse(match.Value, new CultureInfo("us"), out decimal count))
                    {
                        return (count, text.Substring(match.Value.Length, text.Length-match.Value.Length).Trim());
                    }
                    return (1, text);
                }
            }
            return (1, text);
        }

        private string? GetRequirementName(HtmlNode requirementNode)
        {
            var result = requirementNode.InnerText.Trim();
            if (!StratrsWithNumber(result))
            {
                return null;
            }
            return result;
        }

        private bool StratrsWithNumber(string result)
        {
            return Regex.IsMatch(result, @"^\d") ;
        }

        private string? GetModuleName(HtmlNode table)
        {
            var header = table.SelectSingleNode("tbody/tr/th[@colspan='4']");
            if (header == null)
            {
                return null;
            }
            var noteIndex = header.InnerText.IndexOf("Note:");
            if(noteIndex > -1) 
            { 
                return header.InnerText.Substring(0, noteIndex).Trim();
            }

            return header.InnerText.Trim();
        }
    }
}
