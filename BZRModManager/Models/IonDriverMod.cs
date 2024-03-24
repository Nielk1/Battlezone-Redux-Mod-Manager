using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.Models
{
    public class IonDriverPathData
    {
        [JsonProperty(PropertyName = "path", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "recursive", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Recursive { get; set; }

    }
    public class IonDriverMod
    {
        [JsonProperty(PropertyName = "workshop_name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? WorkshopName { get; set; }

        [JsonProperty(PropertyName = "name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? Name { get; set; }

        [JsonProperty(PropertyName = "image", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? Image { get; set; }

        [JsonProperty(PropertyName = "type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "search_paths", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<IonDriverPathData>? SearchPaths { get; set; }

        [JsonProperty(PropertyName = "maps", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string>? Maps { get; set; }

        [JsonProperty(PropertyName = "dependencies", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Dependencies { get; set; }
    }

    public class IonDriverDataExtract
    {
        [JsonProperty(PropertyName = "mods", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, IonDriverMod> Mods { get; set; }

    }
}
