using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.Models
{
    public class IonDriverPathData : IEquatable<IonDriverPathData>
    {
        [JsonProperty(PropertyName = "path", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "recursive", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Recursive { get; set; }

        public bool Equals(IonDriverPathData? other)
        {
            return this?.Path == other?.Path
                && this?.Recursive == other?.Recursive;
        }
    }
    public class IonDriverMod : IEquatable<IonDriverMod>
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

        public bool Equals(IonDriverMod? other)
        {
            bool equal = this?.WorkshopName == other?.WorkshopName
                      && this?.Name == other?.Name
                      && this?.Image == other?.Image
                      && this?.Type == other?.Type
                      && (this?.SearchPaths?.Count ?? 0) == (other?.SearchPaths?.Count ?? 0)
                      && (this?.Maps?.Count ?? 0) == (other?.Maps?.Count ?? 0)
                      && (this?.Dependencies?.Count ?? 0) == (other?.Dependencies?.Count ?? 0);
            if (!equal) return false;
            if (this?.SearchPaths != null && other?.SearchPaths != null) equal &= this.SearchPaths.SequenceEqual(other.SearchPaths);
            if (!equal) return false;
            if (this?.Maps != null && other?.Maps != null) equal &= this.Maps.SequenceEqual(other.Maps);
            if (!equal) return false;
            if (this?.Dependencies != null && other?.Dependencies != null) equal &= this.Dependencies.SequenceEqual(other.Dependencies);
            return equal;
        }
    }

    public class IonDriverDataExtract
    {
        [JsonProperty(PropertyName = "mods", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, IonDriverMod> Mods { get; set; }

    }
}
