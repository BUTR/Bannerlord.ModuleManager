#region License
// MIT License
//
// Copyright (c) Bannerlord's Unofficial Tools & Resources
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

#nullable enable
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning disable
#endif

namespace Bannerlord.ModuleManager
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Xml;

#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
    internal
#else
    public
# endif
        record SubModuleInfoExtended
    {
        public string Name { get; set; } = string.Empty;
        public string DLLName { get; set; } = string.Empty;
        public IReadOnlyList<string> Assemblies { get; set; } = Array.Empty<string>();
        public string SubModuleClassType { get; set; } = string.Empty;
        public IReadOnlyDictionary<string, IReadOnlyList<string>> Tags { get; set; } = new Dictionary<string, IReadOnlyList<string>>();

        public SubModuleInfoExtended() { }
        public SubModuleInfoExtended(string name, string dllName, IReadOnlyList<string> assemblies, string subModuleClassType, IReadOnlyDictionary<string, IReadOnlyList<string>> tags)
        {
            Name = name;
            DLLName = dllName;
            Assemblies = assemblies;
            SubModuleClassType = subModuleClassType;
            Tags = tags;
        }

#if BANNERLORDBUTRMODULEMANAGER_NULLABLE
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("xmlDocument")]
#endif
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
                    assemblies[i] = assembliesList?[i]?.Attributes?["value"]?.InnerText is { } value ? value : string.Empty;
                }
            }

            var tagsList = subModuleNode.SelectSingleNode("Tags")?.SelectNodes("Tag");
            var tags = new Dictionary<string, List<string>>();
            for (var i = 0; i < tagsList?.Count; i++)
            {
                if (tagsList?[i]?.Attributes?["key"]?.InnerText is { } key && tagsList[i]?.Attributes?["value"]?.InnerText is { } value)
                {
                    if (tags.TryGetValue(key, out var list))
                    {
                        list.Add(value);
                    }
                    else
                    {
                        tags[key] = new List<string> { value };
                    }
                }
            }

            return new SubModuleInfoExtended
            {
                Name = name,
                DLLName = dllName,
                Assemblies = assemblies,
                SubModuleClassType = subModuleClassType,
                Tags = new ReadOnlyDictionary<string, IReadOnlyList<string>>(tags.ToDictionary(x => x.Key, x => new ReadOnlyCollection<string>(x.Value) as IReadOnlyList<string>))
            };
        }

        public override string ToString() => $"{Name} - {DLLName}";

        public virtual bool Equals(SubModuleInfoExtended? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }
        public override int GetHashCode() => Name.GetHashCode();
    }
#nullable restore
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning restore
#endif
}