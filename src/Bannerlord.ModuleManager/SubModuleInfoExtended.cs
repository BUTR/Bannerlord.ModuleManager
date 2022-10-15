﻿// <auto-generated>
//   This code file has automatically been added by the "Bannerlord.ModuleManager.Source" NuGet package (https://www.nuget.org/packages/Bannerlord.ModuleManager.Source).
//   Please see https://github.com/BUTR/Bannerlord.ModuleManager for more information.
//
//   IMPORTANT:
//   DO NOT DELETE THIS FILE if you are using a "packages.config" file to manage your NuGet references.
//   Consider migrating to PackageReferences instead:
//   https://docs.microsoft.com/en-us/nuget/consume-packages/migrate-packages-config-to-package-reference
//   Migrating brings the following benefits:
//   * The "Bannerlord.ModuleManager.Source" folder and the "SubModuleInfoExtended.cs" file don't appear in your project.
//   * The added file is immutable and can therefore not be modified by coincidence.
//   * Updating/Uninstalling the package will work flawlessly.
// </auto-generated>

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

namespace Bannerlord.ModuleManager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Collections.ObjectModel;
    using global::System.Linq;
    using global::System.Xml;

#nullable enable
#pragma warning disable
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
#pragma warning restore
#nullable restore
}