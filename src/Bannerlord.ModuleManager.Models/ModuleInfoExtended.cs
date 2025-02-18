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
    using System.Linq;
    using System.Xml;

#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
    internal
#else
    public
# endif
        record ModuleInfoExtended
    {
        private static readonly string NativeModuleId = "Native";
        private static readonly string[] OfficialModuleIds = { NativeModuleId, "SandBox", "SandBoxCore", "StoryMode", "CustomBattle", "BirthAndDeath", "Multiplayer" };

        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsOfficial { get; set; }
        public ApplicationVersion Version { get; set; } = ApplicationVersion.Empty;
        public bool IsSingleplayerModule { get; set; }
        public bool IsMultiplayerModule { get; set; }
        public bool IsServerModule { get; set; }
        public IReadOnlyList<SubModuleInfoExtended> SubModules { get; set; } = Array.Empty<SubModuleInfoExtended>();
        public IReadOnlyList<DependentModule> DependentModules { get; set; } = Array.Empty<DependentModule>();
        public IReadOnlyList<DependentModule> ModulesToLoadAfterThis { get; set; } = Array.Empty<DependentModule>();
        public IReadOnlyList<DependentModule> IncompatibleModules { get; set; } = Array.Empty<DependentModule>();
        public string Url { get; set; } = string.Empty;
        public string UpdateInfo { get; set; } = string.Empty;
        public IReadOnlyList<DependentModuleMetadata> DependentModuleMetadatas { get; set; } = Array.Empty<DependentModuleMetadata>();

#if BANNERLORDBUTRMODULEMANAGER_NULLABLE
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("xmlDocument")]
#endif
        public static ModuleInfoExtended? FromXml(XmlDocument? xmlDocument)
        {
            if (xmlDocument is null)
            {
                return null;
            }

            var moduleNode = xmlDocument.SelectSingleNode("Module");

            var id = moduleNode?.SelectSingleNode("Id")?.Attributes?["value"]?.InnerText ?? string.Empty;
            var name = moduleNode?.SelectSingleNode("Name")?.Attributes?["value"]?.InnerText ?? string.Empty;
            ApplicationVersion.TryParse(moduleNode?.SelectSingleNode("Version")?.Attributes?["value"]?.InnerText, out var version);

            var moduleType = moduleNode?.SelectSingleNode("ModuleType")?.Attributes?["value"].InnerText;
            var isOfficial = moduleNode?.SelectSingleNode("Official")?.Attributes?["value"]?.InnerText.Equals("true") == true ||
                             moduleType == "Official" || moduleType == "OfficialOptional";

            var moduleCategory = moduleNode?.SelectSingleNode("ModuleCategory")?.Attributes?["value"]?.InnerText;
            var isSingleplayerModule = moduleNode?.SelectSingleNode("SingleplayerModule")?.Attributes?["value"]?.InnerText.Equals("true") == true ||
                                       moduleCategory == "Singleplayer" || moduleCategory == "SingleplayerOptional";
            var isMultiplayerModule = moduleNode?.SelectSingleNode("MultiplayerModule")?.Attributes?["value"]?.InnerText.Equals("true") == true ||
                                      moduleCategory == "Multiplayer" || moduleCategory == "MultiplayerOptional";
            var isServerModule = moduleCategory == "Server" || moduleCategory == "ServerOptional";

            var dependentModulesNode = moduleNode?.SelectSingleNode("DependedModules");
            var dependentModulesList = dependentModulesNode?.SelectNodes("DependedModule");
            var dependentModules = new List<DependentModule>(dependentModulesList?.Count ?? 0);
            for (var i = 0; i < dependentModulesList?.Count; i++)
            {
                if (dependentModulesList?[i]?.Attributes?["Id"] is { } idAttr)
                {
                    ApplicationVersion.TryParse(dependentModulesList[i]?.Attributes?["DependentVersion"]?.InnerText, out var dVersion);
                    var isOptional = dependentModulesList[i]?.Attributes?["Optional"] is { } optional && optional.InnerText.Equals("true");
                    dependentModules.Add(new DependentModule(idAttr.InnerText, dVersion, isOptional));
                }
            }

            var modulesToLoadAfterThisNode = moduleNode?.SelectSingleNode("ModulesToLoadAfterThis");
            var modulesToLoadAfterThisList = modulesToLoadAfterThisNode?.SelectNodes("Module");
            var modulesToLoadAfterThis = new List<DependentModule>(modulesToLoadAfterThisList?.Count ?? 0);
            for (var i = 0; i < modulesToLoadAfterThisList?.Count; i++)
            {
                if (modulesToLoadAfterThisList?[i]?.Attributes?["Id"] is { } idAttr)
                {
                    modulesToLoadAfterThis.Add(new DependentModule
                    {
                        Id = idAttr.InnerText,
                        IsOptional = true
                    });
                }
            }

            var incompatibleModulesNode = moduleNode?.SelectSingleNode("IncompatibleModules");
            var incompatibleModulesList = incompatibleModulesNode?.SelectNodes("Module");
            var incompatibleModules = new List<DependentModule>(incompatibleModulesList?.Count ?? 0);
            for (var i = 0; i < incompatibleModulesList?.Count; i++)
            {
                if (incompatibleModulesList?[i]?.Attributes?["Id"] is { } idAttr)
                {
                    incompatibleModules.Add(new DependentModule
                    {
                        Id = idAttr.InnerText,
                        IsOptional = true
                    });
                }
            }

            var subModulesNode = moduleNode?.SelectSingleNode("SubModules");
            var subModuleList = subModulesNode?.SelectNodes("SubModule");
            var subModules = new List<SubModuleInfoExtended>(subModuleList?.Count ?? 0);
            for (var i = 0; i < subModuleList?.Count; i++)
            {
                if (SubModuleInfoExtended.FromXml(subModuleList?[i]) is { } subModule)
                {
                    subModules.Add(subModule);
                }
            }

            // Custom data
            //
            var url = moduleNode?.SelectSingleNode("Url")?.Attributes?["value"]?.InnerText ?? string.Empty;

            var updateInfo = moduleNode?.SelectSingleNode("UpdateInfo")?.Attributes?["value"]?.InnerText ?? string.Empty;

            var dependentModuleMetadatasNode = moduleNode?.SelectSingleNode("DependedModuleMetadatas");
            var dependentModuleMetadatasList = dependentModuleMetadatasNode?.SelectNodes("DependedModuleMetadata");

            // Fixed Launcher supported optional tag
            var loadAfterModules = moduleNode?.SelectSingleNode("LoadAfterModules");
            var loadAfterModuleList = loadAfterModules?.SelectNodes("LoadAfterModule");

            // Bannerlord Launcher supported optional tag
            var optionalDependentModules = moduleNode?.SelectSingleNode("OptionalDependModules");
            var optionalDependentModuleList =
                (dependentModulesNode?.SelectNodes("OptionalDependModule")?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>())
                .Concat(optionalDependentModules?.SelectNodes("OptionalDependModule")?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>())
                .Concat(optionalDependentModules?.SelectNodes("DependModule")?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>()).ToList();

            var dependentModuleMetadatas = new List<DependentModuleMetadata>(dependentModuleMetadatasList?.Count ?? 0 + loadAfterModuleList?.Count ?? 0 + optionalDependentModuleList.Count);
            for (var i = 0; i < dependentModuleMetadatasList?.Count; i++)
            {
                if (dependentModuleMetadatasList?[i]?.Attributes?["id"] is { } idAttr)
                {
                    var order = dependentModuleMetadatasList[i]?.Attributes?["order"] is { } orderAttr && Enum.TryParse<LoadTypeParse>(orderAttr.InnerText, out var loadType) ? (LoadType) loadType : LoadType.None;
                    var optional = dependentModuleMetadatasList[i]?.Attributes?["optional"]?.InnerText.Equals("true") ?? false;
                    var incompatible = dependentModuleMetadatasList[i]?.Attributes?["incompatible"]?.InnerText.Equals("true") ?? false;
                    var dVersion = ApplicationVersion.TryParse(dependentModuleMetadatasList[i]?.Attributes?["version"]?.InnerText, out var v) ? v : ApplicationVersion.Empty;
                    var dVersionRange = ApplicationVersionRange.TryParse(dependentModuleMetadatasList[i]?.Attributes?["version"]?.InnerText ?? string.Empty, out var vr) ? vr : ApplicationVersionRange.Empty;
                    dependentModuleMetadatas.Add(new DependentModuleMetadata
                    {
                        Id = idAttr.InnerText,
                        LoadType = (LoadType) order,
                        IsOptional = optional,
                        IsIncompatible = incompatible,
                        Version = dVersion,
                        VersionRange = dVersionRange
                    });
                }
            }
            for (var i = 0; i < loadAfterModuleList?.Count; i++)
            {
                if (loadAfterModuleList?[i]?.Attributes?["Id"] is { } idAttr)
                {
                    dependentModuleMetadatas.Add(new DependentModuleMetadata
                    {
                        Id = idAttr.InnerText,
                        LoadType = LoadType.LoadAfterThis,
                        IsOptional = false,
                        IsIncompatible = false,
                        Version = ApplicationVersion.Empty,
                        VersionRange = ApplicationVersionRange.Empty
                    });
                }
            }
            for (var i = 0; i < optionalDependentModuleList.Count; i++)
            {
                if (optionalDependentModuleList[i].Attributes?["Id"] is { } idAttr)
                {
                    dependentModuleMetadatas.Add(new DependentModuleMetadata
                    {
                        Id = idAttr.InnerText,
                        LoadType = LoadType.None,
                        IsOptional = true,
                        IsIncompatible = false,
                        Version = ApplicationVersion.Empty,
                        VersionRange = ApplicationVersionRange.Empty
                    });
                }
            }

            var requiredGameVersion = moduleNode?.SelectSingleNode("RequiredGameVersion");
            var requiredGameVersionVal = requiredGameVersion?.Attributes?["value"]?.InnerText ?? string.Empty;
            var requiredGameVersionOptional = requiredGameVersion?.Attributes?["optional"]?.InnerText.Equals("true") == true;
            if (!string.IsNullOrWhiteSpace(requiredGameVersionVal) && ApplicationVersion.TryParse(requiredGameVersionVal, out var gameVersion))
            {
                foreach (var moduleId in OfficialModuleIds)
                {
                    var isNative = moduleId.Equals(NativeModuleId);

                    // Override any existing metadata
                    if (dependentModuleMetadatas.Find(dmm => moduleId.Equals(dmm.Id, StringComparison.Ordinal)) is { } module)
                    {
                        dependentModuleMetadatas.Remove(module);
                    }

                    dependentModuleMetadatas.Add(new DependentModuleMetadata
                    {
                        Id = moduleId,
                        LoadType = LoadType.LoadBeforeThis,
                        IsOptional = requiredGameVersionOptional && !isNative,
                        IsIncompatible = false,
                        Version = gameVersion,
                        VersionRange = ApplicationVersionRange.Empty
                    });
                }
            }

            return new ModuleInfoExtended
            {
                Id = id,
                Name = name,
                IsOfficial = isOfficial,
                Version = version,
                IsSingleplayerModule = isSingleplayerModule,
                IsMultiplayerModule = isMultiplayerModule,
                IsServerModule = isServerModule,
                SubModules = subModules,
                DependentModules = dependentModules,
                ModulesToLoadAfterThis = modulesToLoadAfterThis,
                IncompatibleModules = incompatibleModules,
                Url = url,
                UpdateInfo = !string.IsNullOrEmpty(updateInfo) ? updateInfo : string.Empty,
                DependentModuleMetadatas = dependentModuleMetadatas
            };
        }

        public ModuleInfoExtended() { }
        public ModuleInfoExtended(string id, string name, bool isOfficial, ApplicationVersion version, bool isSingleplayerModule, bool isMultiplayerModule,
            IReadOnlyList<SubModuleInfoExtended> subModules, IReadOnlyList<DependentModule> dependentModules, IReadOnlyList<DependentModule> modulesToLoadAfterThis,
            IReadOnlyList<DependentModule> incompatibleModules, IReadOnlyList<DependentModuleMetadata> dependentModuleMetadatas, string url)
        {
            Id = id;
            Name = name;
            IsOfficial = isOfficial;
            Version = version;
            IsSingleplayerModule = isSingleplayerModule;
            IsMultiplayerModule = isMultiplayerModule;
            SubModules = subModules;
            DependentModules = dependentModules;
            ModulesToLoadAfterThis = modulesToLoadAfterThis;
            IncompatibleModules = incompatibleModules;
            DependentModuleMetadatas = dependentModuleMetadatas;
            Url = url;
        }

        public bool IsNative() => NativeModuleId.Equals(Id, StringComparison.OrdinalIgnoreCase);

        public override string ToString() => $"{Id} - {Version}";

        public virtual bool Equals(ModuleInfoExtended? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }
        public override int GetHashCode() => Id.GetHashCode();
    }
#nullable restore
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning restore
#endif
}