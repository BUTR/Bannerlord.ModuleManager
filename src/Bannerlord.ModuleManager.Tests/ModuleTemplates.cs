using System.Collections.Generic;

namespace Bannerlord.ModuleManager.Tests
{
    internal static class ModuleTemplates
    {
        public static ModuleInfoExtended NativeModuleBase => new()
        {
            Id = "NativeModule",
            Name = "Native Module Module",
            Version = ApplicationVersion.Empty,
            IsOfficial = true,
            IsSingleplayerModule = true,
            IsMultiplayerModule = false,
        };
        public static ModuleInfoExtended Native2ModuleBase => new()
        {
            Id = "Native2Module",
            Name = "Native2 Module Module",
            Version = ApplicationVersion.Empty,
            IsOfficial = true,
            IsSingleplayerModule = true,
            IsMultiplayerModule = false,
            DependentModules = new List<DependentModule>
            {
                new("NativeModule", ApplicationVersion.Empty)
            },
        };

        public static ModuleInfoExtended CommunityFrameworkModuleBase => new()
        {
            Id = "CommunityFrameworkModule",
            Name = "Community Framework Module",
            Version = ApplicationVersion.Empty,
            IsOfficial = false,
            IsSingleplayerModule = true,
            IsMultiplayerModule = false,
            DependentModules = new List<DependentModule>
            {
                new("NativeModule", ApplicationVersion.Empty)
            },
            DependentModuleMetadatas = new List<DependentModuleMetadata>
            {
                new()
                {
                    Id = "NativeModule",
                    IsOptional = false,
                    LoadType = LoadType.LoadAfterThis,
                    Version = ApplicationVersion.Empty
                }
            }
        };

        public static ModuleInfoExtended CustomModuleBase => new()
        {
            Id = "CustomModule",
            Name = "Custom Module",
            Version = ApplicationVersion.Empty,
            IsOfficial = false,
            IsSingleplayerModule = true,
            IsMultiplayerModule = false,
        };

        public static ModuleInfoExtended Custom2ModuleBase => new()
        {
            Id = "Custom2Module",
            Name = "Custom2 Module",
            Version = ApplicationVersion.Empty,
            IsOfficial = false,
            IsSingleplayerModule = true,
            IsMultiplayerModule = false,
        };


        public static ModuleInfoExtended CustomModuleWithNativeDM => CustomModuleBase with
        {
            DependentModules = new List<DependentModule>
            {
                new("NativeModule", ApplicationVersion.Empty)
            },
        };
        public static ModuleInfoExtended CustomModuleWithNativeDMM => CustomModuleBase with
        {
            DependentModuleMetadatas = new List<DependentModuleMetadata>
            {
                new()
                {
                    Id = "NativeModule",
                    IsOptional = false,
                    LoadType = LoadType.LoadBeforeThis,
                    Version = ApplicationVersion.Empty
                }
            }
        };
        public static ModuleInfoExtended CustomModuleWithNativeOptional => CustomModuleBase with
        {
            DependentModuleMetadatas = new List<DependentModuleMetadata>
            {
                new()
                {
                    Id = "NativeModule",
                    IsOptional = true,
                    LoadType = LoadType.LoadBeforeThis,
                    Version = ApplicationVersion.Empty
                }
            }
        };

        public static ModuleInfoExtended CustomModule2WithCustomModuleDM => Custom2ModuleBase with
        {
            DependentModules = new List<DependentModule>
            {
                new("CustomModule", ApplicationVersion.Empty)
            },
        };
        public static ModuleInfoExtended CustomModule2WithCustomModuleDMM => Custom2ModuleBase with
        {
            DependentModuleMetadatas = new List<DependentModuleMetadata>
            {
                new()
                {
                    Id = "CustomModule",
                    IsOptional = false,
                    LoadType = LoadType.LoadBeforeThis,
                    Version = ApplicationVersion.Empty
                }
            }
        };
        public static ModuleInfoExtended CustomModule2WithCustomModuleDMMVersion => Custom2ModuleBase with
        {
            DependentModuleMetadatas = new List<DependentModuleMetadata>
            {
                new()
                {
                    Id = "CustomModule",
                    IsOptional = false,
                    LoadType = LoadType.LoadBeforeThis,
                    Version = new ApplicationVersion(ApplicationVersionType.EarlyAccess, 1, 0, 0, 0)
                }
            }
        };
        public static ModuleInfoExtended CustomModule2WithCustomModuleOptional => Custom2ModuleBase with
        {
            DependentModuleMetadatas = new List<DependentModuleMetadata>
            {
                new()
                {
                    Id = "CustomModule",
                    IsOptional = true,
                    LoadType = LoadType.LoadBeforeThis,
                    Version = ApplicationVersion.Empty
                }
            }
        };
        public static ModuleInfoExtended CustomModule2WithCustomModuleIncompatible => Custom2ModuleBase with
        {
            DependentModuleMetadatas = new List<DependentModuleMetadata>
            {
                new()
                {
                    Id = "CustomModule",
                    IsIncompatible = true
                }
            }
        };

        public static ModuleInfoExtended CommunityFrameworkModuleBeforeNative => CommunityFrameworkModuleBase with
        {
            DependentModuleMetadatas = new List<DependentModuleMetadata>
            {
                new()
                {
                    Id = "NativeModule",
                    IsOptional = false,
                    LoadType = LoadType.LoadAfterThis,
                    Version = ApplicationVersion.Empty
                }
            }
        };
    }

    public enum ModuleListTemplates
    {
        /// <summary>
        /// One module depends on another module using the TW's approach
        /// </summary>
        SimpleTW,
        /// <summary>
        /// One module depends on another module using the TW's approach
        /// Two times
        /// </summary>
        SimpleTW2,
        /// <summary>
        /// One module depends on another module using the BUTR's approach
        /// </summary>
        SimpleBUTR,
        /// <summary>
        /// One module depends on another module using the BUTR's approach
        /// Two times
        /// </summary>
        SimpleBUTR2,

        /// <summary>
        /// One module depends on another module as an optional
        /// </summary>
        Optional,
        /// <summary>
        /// One module depends on another module as an optional
        /// Two times
        /// </summary>
        Optional2,
        /// <summary>
        /// One module depends on another module as an optional, but that module doesn't exist
        /// </summary>
        OptionalNonExisting,

        /// <summary>
        /// One module declares another module as incompatible
        /// </summary>
        Incompatible0,
        /// <summary>
        /// One module declares another module as incompatible
        /// </summary>
        Incompatible1,

        /// <summary>
        /// Framework - load before Native
        /// Native    - nothing
        /// Native2   - after Native
        /// Custom    - after Native
        /// </summary>
        Complex1,
        /// <summary>
        /// Framework - load before Native
        /// Native    - nothing
        /// Custom    - after Native
        /// Custom2   - incompatible with Custom
        /// </summary>
        Complex2,

        /// <summary>
        /// Native    - nothing
        /// Custom2   - load after with Custom
        /// </summary>
        MissingModule1,
        /// <summary>
        /// Native    - nothing
        /// Custom2   - load after with Custom with version check
        /// </summary>
        MissingModule2,
    }

    internal class ModuleStorage
    {
        public ModuleListTemplates ModuleListTemplate;

        public ModuleStorage(ModuleListTemplates moduleListTemplate)
        {
            ModuleListTemplate = moduleListTemplate;
        }

        private (List<ModuleInfoExtended> ModuleInfoExtendeds, List<string> ExpectedIdOrder) Tuple => ModuleListTemplate switch
        {
            ModuleListTemplates.SimpleTW => (
                new()
                {
                    ModuleTemplates.CustomModuleWithNativeDM,
                    ModuleTemplates.NativeModuleBase,
                },
                new()
                {
                    ModuleTemplates.NativeModuleBase.Id,
                    ModuleTemplates.CustomModuleWithNativeDM.Id,
                }),
            ModuleListTemplates.SimpleTW2 => (
                new()
                {
                    ModuleTemplates.CustomModule2WithCustomModuleDM,
                    ModuleTemplates.CustomModuleWithNativeDM,
                    ModuleTemplates.NativeModuleBase,
                },
                new()
                {
                    ModuleTemplates.NativeModuleBase.Id,
                    ModuleTemplates.CustomModuleWithNativeDM.Id,
                    ModuleTemplates.CustomModule2WithCustomModuleDM.Id,
                }),
            ModuleListTemplates.SimpleBUTR => (
                new()
                {
                    ModuleTemplates.CustomModuleWithNativeDMM,
                    ModuleTemplates.NativeModuleBase,
                },
                new()
                {
                    ModuleTemplates.NativeModuleBase.Id,
                    ModuleTemplates.CustomModuleWithNativeDMM.Id,
                }),
            ModuleListTemplates.SimpleBUTR2 => (
                new()
                {
                    ModuleTemplates.CustomModule2WithCustomModuleDMM,
                    ModuleTemplates.CustomModuleWithNativeDMM,
                    ModuleTemplates.NativeModuleBase,
                },
                new()
                {
                    ModuleTemplates.NativeModuleBase.Id,
                    ModuleTemplates.CustomModuleWithNativeDMM.Id,
                    ModuleTemplates.CustomModule2WithCustomModuleDMM.Id,
                }),

            ModuleListTemplates.Optional => (
                new()
                {
                    ModuleTemplates.CustomModule2WithCustomModuleOptional,
                    ModuleTemplates.NativeModuleBase,
                    ModuleTemplates.CustomModuleWithNativeDMM,
                },
                new()
                {
                    ModuleTemplates.NativeModuleBase.Id,
                    ModuleTemplates.CustomModuleWithNativeDMM.Id,
                    ModuleTemplates.CustomModule2WithCustomModuleOptional.Id,
                }),
            ModuleListTemplates.Optional2 => (
                new()
                {
                    ModuleTemplates.CustomModule2WithCustomModuleOptional,
                    ModuleTemplates.NativeModuleBase,
                    ModuleTemplates.CustomModuleWithNativeOptional,
                },
                new()
                {
                    ModuleTemplates.NativeModuleBase.Id,
                    ModuleTemplates.CustomModuleWithNativeOptional.Id,
                    ModuleTemplates.CustomModule2WithCustomModuleOptional.Id,
                }),
            ModuleListTemplates.OptionalNonExisting => (
                new()
                {
                    ModuleTemplates.Custom2ModuleBase,
                    ModuleTemplates.NativeModuleBase,
                },
                new()
                {
                    ModuleTemplates.NativeModuleBase.Id,
                    ModuleTemplates.Custom2ModuleBase.Id,
                }),
            ModuleListTemplates.Incompatible0 => (
                new()
                {
                    ModuleTemplates.CustomModuleWithNativeDMM,
                    ModuleTemplates.NativeModuleBase,
                    ModuleTemplates.CustomModule2WithCustomModuleIncompatible,
                },
                new()
                {
                    ModuleTemplates.NativeModuleBase.Id,
                    ModuleTemplates.CustomModuleWithNativeDMM.Id,
                }),
            ModuleListTemplates.Incompatible1 => (
                new()
                {
                    ModuleTemplates.CustomModule2WithCustomModuleIncompatible,
                    ModuleTemplates.NativeModuleBase,
                    //ModuleTemplates.CustomModuleWithNativeDMM with { IsSelected = false },
                },
                new()
                {
                    ModuleTemplates.NativeModuleBase.Id,
                    ModuleTemplates.CustomModule2WithCustomModuleIncompatible.Id,
                }),

            ModuleListTemplates.Complex1 => (
                new()
                {
                    ModuleTemplates.CustomModuleWithNativeDMM,
                    ModuleTemplates.NativeModuleBase,
                    ModuleTemplates.Native2ModuleBase,
                    ModuleTemplates.CommunityFrameworkModuleBeforeNative,
                },
                new()
                {
                    ModuleTemplates.CommunityFrameworkModuleBeforeNative.Id,
                    ModuleTemplates.NativeModuleBase.Id,
                    ModuleTemplates.Native2ModuleBase.Id,
                    ModuleTemplates.CustomModuleWithNativeDMM.Id,
                }),
            ModuleListTemplates.Complex2 => (
                new()
                {
                    ModuleTemplates.CustomModuleWithNativeDMM,
                    ModuleTemplates.NativeModuleBase,
                    ModuleTemplates.CustomModule2WithCustomModuleIncompatible,
                    ModuleTemplates.CommunityFrameworkModuleBeforeNative,
                },
                new()
                {
                    ModuleTemplates.CommunityFrameworkModuleBeforeNative.Id,
                    ModuleTemplates.NativeModuleBase.Id,
                    ModuleTemplates.CustomModuleWithNativeDMM.Id,
                }),

            ModuleListTemplates.MissingModule1 => (
                new()
                {
                    ModuleTemplates.CustomModule2WithCustomModuleDMM,
                    ModuleTemplates.NativeModuleBase,
                },
                new()
                {
                    ModuleTemplates.NativeModuleBase.Id
                }),
            ModuleListTemplates.MissingModule2 => (
                new()
                {
                    ModuleTemplates.CustomModule2WithCustomModuleDMMVersion,
                    ModuleTemplates.NativeModuleBase,
                },
                new()
                {
                    ModuleTemplates.NativeModuleBase.Id
                }),

            _ => (new(), new())
        };

        public List<ModuleInfoExtended> ModuleInfoExtendeds => Tuple.ModuleInfoExtendeds;
        public List<string> ExpectedIdOrder => Tuple.ExpectedIdOrder;

        //private static IEnumerable<ModuleInfoExtended> PreSort(IEnumerable<ModuleInfoExtended> source) => source
        //    .OrderByDescending(mim => mim.Id, new AlphanumComparatorFast())
        //    .ThenByDescending(mim => mim.IsOfficial);
    }
}