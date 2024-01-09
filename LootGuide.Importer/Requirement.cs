using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootGuide.Importer
{
    public class Requirement
    {
        public required int Level { get; set; }
        public required string Module { get; set; }
        public required string Name { get; set; }
        public required decimal Count { get; set; }
    }
}
