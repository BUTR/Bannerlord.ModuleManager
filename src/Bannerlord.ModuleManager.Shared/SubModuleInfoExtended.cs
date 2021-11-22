using System;
using System.Collections.Generic;
using System.Xml;

namespace Bannerlord.ModuleManager
{
    public sealed record SubModuleInfoExtended
    {
        public string Name { get; init; } = string.Empty;
        public string DLLName { get; init; } = string.Empty;
        public IReadOnlyList<string> Assemblies { get; init; } = Array.Empty<string>();
        public string SubModuleClassType { get; init; } = string.Empty;
        public IReadOnlyDictionary<SubModuleTags, string> Tags { get; init; } = new Dictionary<SubModuleTags, string>();

        public SubModuleInfoExtended() { }
        public SubModuleInfoExtended(string name, string dllName, IReadOnlyList<string> assemblies, string subModuleClassType, IReadOnlyDictionary<SubModuleTags, string> tags)
        {
            Name = name;
            DLLName = dllName;
            Assemblies = assemblies;
            SubModuleClassType = subModuleClassType;
            Tags = tags;
        }

        public static SubModuleInfoExtended? FromXml(XmlNode? subModuleNode)
        {
            if (subModuleNode is null) return null;

            var name = subModuleNode.SelectSingleNode("Name")?.Attributes?["value"]?.InnerText ?? string.Empty;
            var dllName = subModuleNode.SelectSingleNode("DLLName")?.Attributes?["value"]?.InnerText ?? string.Empty;

            var subModuleClassType = subModuleNode.SelectSingleNode("SubModuleClassType")?.Attributes?["value"]?.InnerText ?? string.Empty;
            var assemblies = Array.Empty<string>();
            if (subModuleNode.SelectSingleNode("Assemblies") != null)
            {
                var assembliesList = subModuleNode.SelectSingleNode("Assemblies")?.SelectNodes("Assembly");
                assemblies = new string[assembliesList?.Count ?? 0];
                for (var i = 0; i < assembliesList?.Count; i++)
                {
                    assemblies[i] = assembliesList[i]?.Attributes?["value"]?.InnerText is { } value ? value : string.Empty;
                }
            }

            var tagsList = subModuleNode.SelectSingleNode("Tags")?.SelectNodes("Tag");
            var tags = new Dictionary<SubModuleTags, string>(tagsList?.Count ?? 0);
            for (var i = 0; i < tagsList?.Count; i++)
            {
                if (tagsList[i]?.Attributes?["key"]?.InnerText is { } key && tagsList[i]?.Attributes?["value"]?.InnerText is { } value && Enum.TryParse<SubModuleTags>(key, out var subModuleTags))
                {
                    tags.Add(subModuleTags, value);
                }
            }
            
            return new SubModuleInfoExtended
            {
                Name = name,
                DLLName = dllName,
                Assemblies = assemblies,
                SubModuleClassType = subModuleClassType,
                Tags = tags
            };
        }

        public override string ToString() => $"{Name} - {DLLName}";

        public bool Equals(SubModuleInfoExtended? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }
        public override int GetHashCode() => HashCode.Combine(Name);
    }
}