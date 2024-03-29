using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.Models
{
    public class ModIniData
    {
        private string IniFilename { get; set; }
        public bool ParseError { get; set; }
        public string? ModName { get; set; }
        public string? ModManagerName { get; set; }
        public string? ModType { get; set; }
        public string? Description { get; set; }
        public List<string>? AssetDependencies { get; set; }
        public List<string>? CustomTags { get; set; }
        public ModIniData(string iniFilename)
        {
            this.IniFilename = iniFilename;
            this.ParseError = false;
        }
    }
}
