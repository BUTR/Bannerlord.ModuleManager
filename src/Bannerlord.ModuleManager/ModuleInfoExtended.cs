using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Bannerlord.ModuleManager
{
    public sealed record ModuleInfoExtended
    {
        private static readonly string NativeModuleId = "Native";
        private static readonly string[] OfficialModuleIds = { NativeModuleId, "SandBox", "SandBoxCore", "StoryMode", "CustomBattle" };

        public string Id { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public bool IsOfficial { get; init; }
        public ApplicationVersion Version { get; init; } = ApplicationVersion.Empty;
        public bool IsSingleplayerModule { get; init; }
        public bool IsMultiplayerModule { get; init; }
        public IReadOnlyList<SubModuleInfoExtended> SubModules { get; init; } = Array.Empty<SubModuleInfoExtended>();
        public IReadOnlyList<DependentModule> DependentModules { get; init; } = Array.Empty<DependentModule>();
        public IReadOnlyList<DependentModule> ModulesToLoadAfterThis { get; init; } = Array.Empty<DependentModule>();
        public IReadOnlyList<DependentModule> IncompatibleModules { get; init; } = Array.Empty<DependentModule>();
        public string Url { get; init; } = string.Empty;
        public IReadOnlyList<DependentModuleMetadata> DependentModuleMetadatas { get; init; } = Array.Empty<DependentModuleMetadata>();

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

            var isOfficial = moduleNode?.SelectSingleNode("Official")?.Attributes?["value"]?.InnerText.Equals("true") == true;
            var isSingleplayerModule = moduleNode?.SelectSingleNode("SingleplayerModule")?.Attributes?["value"]?.InnerText.Equals("true") == true;
            var isMultiplayerModule = moduleNode?.SelectSingleNode("MultiplayerModule")?.Attributes?["value"]?.InnerText.Equals("true") == true;

            var dependentModulesNode = moduleNode?.SelectSingleNode("DependedModules");
            var dependentModulesList = dependentModulesNode?.SelectNodes("DependedModule");
            var dependentModules = new DependentModule[dependentModulesList?.Count ?? 0];
            for (var i = 0; i < dependentModulesList?.Count; i++)
            {
                if (dependentModulesList[i]?.Attributes?["Id"] is { } idAttr)
                {
                    ApplicationVersion.TryParse(dependentModulesList[i]?.Attributes?["DependentVersion"]?.InnerText, out var dVersion);
                    var isOptional = dependentModulesList[i]?.Attributes?["Optional"] is { } optional && optional.InnerText.Equals("true");
                    dependentModules[i] = new DependentModule(idAttr.InnerText, dVersion, isOptional);
                }
            }

            var modulesToLoadAfterThisNode = moduleNode?.SelectSingleNode("ModulesToLoadAfterThis");
            var modulesToLoadAfterThisList = modulesToLoadAfterThisNode?.SelectNodes("Module");
            var modulesToLoadAfterThis = new DependentModule[modulesToLoadAfterThisList?.Count ?? 0];
            for (var i = 0; i < modulesToLoadAfterThisList?.Count; i++)
            {
                if (modulesToLoadAfterThisList[i]?.Attributes?["Id"] is { } idAttr)
                {
                    modulesToLoadAfterThis[i] = new DependentModule
                    {
                        Id = idAttr.InnerText
                    };
                }
            }

            var incompatibleModulesNode = moduleNode?.SelectSingleNode("IncompatibleModules");
            var incompatibleModulesList = incompatibleModulesNode?.SelectNodes("Module");
            var incompatibleModules = new DependentModule[incompatibleModulesList?.Count ?? 0];
            for (var i = 0; i < incompatibleModulesList?.Count; i++)
            {
                if (incompatibleModulesList[i]?.Attributes?["Id"] is { } idAttr)
                {
                    incompatibleModules[i] = new DependentModule
                    {
                        Id = idAttr.InnerText
                    };
                }
            }

            var subModulesNode = moduleNode?.SelectSingleNode("SubModules");
            var subModuleList = subModulesNode?.SelectNodes("SubModule");
            var subModules = new List<SubModuleInfoExtended>(subModuleList?.Count ?? 0);
            for (var i = 0; i < subModuleList?.Count; i++)
            {
                if (SubModuleInfoExtended.FromXml(subModuleList[i]) is { } subModule)
                {
                    subModules.Add(subModule);
                }
            }

            // Custom data
            //
            var url = moduleNode?.SelectSingleNode("Url")?.Attributes?["value"]?.InnerText ?? string.Empty;

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
                if (dependentModuleMetadatasList[i]?.Attributes?["id"] is { } idAttr)
                {
                    var incompatible = dependentModuleMetadatasList[i]?.Attributes?["incompatible"]?.InnerText.Equals("true") ?? false;
                    if (incompatible)
                    {
                        dependentModuleMetadatas.Add(new DependentModuleMetadata
                        {
                            Id = idAttr.InnerText,
                            LoadType = LoadType.None,
                            IsOptional = false,
                            IsIncompatible = incompatible,
                            Version = ApplicationVersion.Empty,
                            VersionRange = ApplicationVersionRange.Empty
                        });
                    }
                    else if (dependentModuleMetadatasList[i]?.Attributes?["order"] is { } orderAttr && Enum.TryParse<LoadTypeParse>(orderAttr.InnerText, out var order))
                    {
                        var optional = dependentModuleMetadatasList[i]?.Attributes?["optional"]?.InnerText.Equals("true") ?? false;
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
            }
            for (var i = 0; i < loadAfterModuleList?.Count; i++)
            {
                if (loadAfterModuleList[i]?.Attributes?["Id"] is { } idAttr)
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
                    if (dependentModuleMetadatas.Find(dmm => dmm.Id.Equals(moduleId, StringComparison.Ordinal)) is { } module)
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
                SubModules = subModules,
                DependentModules = dependentModules,
                ModulesToLoadAfterThis = modulesToLoadAfterThis,
                IncompatibleModules = incompatibleModules,
                Url = url,
                DependentModuleMetadatas = dependentModuleMetadatas
            };
        }

        public bool IsNative() => Id.Equals(NativeModuleId, StringComparison.OrdinalIgnoreCase);

        public override string ToString() => $"{Id} - {Version}";

        public bool Equals(ModuleInfoExtended? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }
        public override int GetHashCode() => Id.GetHashCode();
    }
}