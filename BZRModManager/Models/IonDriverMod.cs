using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        // does this property actually exist?
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? Description { get; set; }

        [JsonProperty(PropertyName = "image", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? Image { get; set; }

        [JsonProperty(PropertyName = "type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ModType { get; set; }

        [JsonProperty(PropertyName = "types", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string>? ModTypes { get; set; }

        [JsonProperty(PropertyName = "search_paths", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<IonDriverPathData>? SearchPaths { get; set; }

        [JsonProperty(PropertyName = "maps", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string>? Maps { get; set; }

        [JsonProperty(PropertyName = "dependencies", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Dependencies { get; set; }

        [JsonProperty(PropertyName = "iondriver_tags", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> IondriverTags { get; set; }





        // data moved in here
        //[JsonProperty(PropertyName = "vehicles", DefaultValueHandling = DefaultValueHandling.Ignore)]
        //public Dictionary<string, IonDriverVehicle>? Vehicles { get; set; }





        public bool Equals(IonDriverMod? other)
        {
            bool equal = this?.WorkshopName == other?.WorkshopName
                      && this?.Name == other?.Name
                      && this?.Image == other?.Image
                      && this?.ModType == other?.ModType
                      && (this?.ModTypes?.Count ?? 0) == (other?.ModTypes?.Count ?? 0)
                      && (this?.SearchPaths?.Count ?? 0) == (other?.SearchPaths?.Count ?? 0)
                      && (this?.Maps?.Count ?? 0) == (other?.Maps?.Count ?? 0)
                      && (this?.Dependencies?.Count ?? 0) == (other?.Dependencies?.Count ?? 0)
                      && (this?.IondriverTags?.Count ?? 0) == (other?.IondriverTags?.Count ?? 0);
            if (!equal) return false;
            if (this?.ModTypes != null && other?.ModTypes != null) equal &= this.ModTypes.SequenceEqual(other.ModTypes);
            if (!equal) return false;
            if (this?.SearchPaths != null && other?.SearchPaths != null) equal &= this.SearchPaths.SequenceEqual(other.SearchPaths);
            if (!equal) return false;
            if (this?.Maps != null && other?.Maps != null) equal &= this.Maps.SequenceEqual(other.Maps);
            if (!equal) return false;
            if (this?.Dependencies != null && other?.Dependencies != null) equal &= this.Dependencies.SequenceEqual(other.Dependencies);
            if (!equal) return false;
            if (this?.IondriverTags != null && other?.IondriverTags != null) equal &= this.IondriverTags.SequenceEqual(other.IondriverTags);
            return equal;
        }
    }

    public class IonDriverDescriptionFile
    {
        [JsonProperty(PropertyName = "file", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? File { get; set; }

        [JsonProperty(PropertyName = "content", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? Content { get; set; }
    }
    public class IonDriverVehicle
    {
        [JsonProperty(PropertyName = "name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? Name { get; set; }

        [JsonProperty(PropertyName = "mod_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? ModId { get; set; }

        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, IonDriverDescriptionFile>? Description { get; set; }
    }

    public class IonDriverDataExtract
    {
        [JsonProperty(PropertyName = "mods", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, IonDriverMod>? Mods { get; set; }

        [JsonProperty(PropertyName = "vehicles", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(EmptyArrayOrDictionaryConverter))]
        public Dictionary<string, IonDriverVehicle>? Vehicles { get; set; }

    }
}

public class EmptyArrayOrDictionaryConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsAssignableFrom(typeof(Dictionary<string, object>));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JToken token = JToken.Load(reader);
        if (token.Type == JTokenType.Object)
        {
            return token.ToObject(objectType, serializer);
        }
        else if (token.Type == JTokenType.Array)
        {
            if (!token.HasValues)
            {
                // create empty dictionary
                return Activator.CreateInstance(objectType);
            }
        }

        throw new JsonSerializationException("Object or empty array expected");
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}