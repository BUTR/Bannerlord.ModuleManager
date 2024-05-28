﻿// <auto-generated>
//   This code file has automatically been added by the "Bannerlord.ModuleManager.Source" NuGet package (https://www.nuget.org/packages/Bannerlord.ModuleManager.Source).
//   Please see https://github.com/BUTR/Bannerlord.ModuleManager for more information.
//
//   IMPORTANT:
//   DO NOT DELETE THIS FILE if you are using a "packages.config" file to manage your NuGet references.
//   Consider migrating to PackageReferences instead:
//   https://docs.microsoft.com/en-us/nuget/consume-packages/migrate-packages-config-to-package-reference
//   Migrating brings the following benefits:
//   * The "Bannerlord.ModuleManager.Source" folder and the "ModuleUtilities.cs" file don't appear in your project.
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

#nullable enable
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning disable
#endif

namespace Bannerlord.ModuleManager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;

#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
    internal
#else
    public
# endif
        static class ModuleUtilities
    {
        /// <summary>
        /// Checks if all dependencies for a module are present
        /// </summary>
        /// <param name="modules">Assumed that only valid and selected to launch modules are in the list</param>
        /// <param name="module">The module that is being checked</param>
        public static bool AreDependenciesPresent(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleInfoExtended module)
        {
            foreach (var metadata in module.DependenciesLoadBeforeThisDistinct())
            {
                if (metadata.IsOptional)
                    continue;

                if (modules.All(x => !string.Equals(x.Id, metadata.Id, StringComparison.Ordinal)))
                    return false;
            }
            foreach (var metadata in module.DependenciesIncompatiblesDistinct())
            {
                if (modules.Any(x => string.Equals(x.Id, metadata.Id, StringComparison.Ordinal)))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Returns all dependencies for a module
        /// </summary>
        public static IEnumerable<ModuleInfoExtended> GetDependencies(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleInfoExtended module)
        {
            var visited = new HashSet<ModuleInfoExtended>();
            return GetDependencies(modules, module, visited, new ModuleSorterOptions() { SkipOptionals = false, SkipExternalDependencies = false });
        }
        /// <summary>
        /// Returns all dependencies for a module
        /// </summary>
        public static IEnumerable<ModuleInfoExtended> GetDependencies(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleInfoExtended module, ModuleSorterOptions options)
        {
            var visited = new HashSet<ModuleInfoExtended>();
            return GetDependencies(modules, module, visited, options);
        }
        /// <summary>
        /// Returns all dependencies for a module
        /// </summary>
        public static IEnumerable<ModuleInfoExtended> GetDependencies(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleInfoExtended module, HashSet<ModuleInfoExtended> visited, ModuleSorterOptions options)
        {
            var dependencies = new List<ModuleInfoExtended>();
            ModuleSorter.Visit(module, x => GetDependenciesInternal(modules, x, options), moduleToAdd =>
            {
                if (moduleToAdd != module)
                    dependencies.Add(moduleToAdd);
            }, visited);
            return dependencies;
        }
        private static IEnumerable<ModuleInfoExtended> GetDependenciesInternal(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleInfoExtended module, ModuleSorterOptions options)
        {
            foreach (var dependentModuleMetadata in module.DependenciesLoadBeforeThisDistinct())
            {
                if (dependentModuleMetadata.IsOptional && options.SkipOptionals)
                    continue;

                var moduleInfo = modules.FirstOrDefault(x => string.Equals(x.Id, dependentModuleMetadata.Id, StringComparison.Ordinal));
                if (!dependentModuleMetadata.IsOptional && moduleInfo is null)
                {
                    // We should not hit this place. If we do, the module list is invalid
                }
                else if (moduleInfo is not null)
                {
                    yield return moduleInfo;
                }
            }

            if (!options.SkipExternalDependencies)
            {
                foreach (var moduleInfo in modules)
                {
                    foreach (var dependentModuleMetadata in moduleInfo.DependenciesLoadAfterThisDistinct())
                    {
                        if (dependentModuleMetadata.IsOptional && options.SkipOptionals)
                            continue;

                        if (!string.Equals(dependentModuleMetadata.Id, module.Id, StringComparison.Ordinal))
                            continue;

                        yield return moduleInfo;
                    }
                }
            }
        }

        /// <summary>
        /// Validates a module
        /// </summary>
        /// <param name="modules">All available modules</param>
        /// <returns>Any error that were detected during inspection</returns>
        public static IEnumerable<ModuleIssue> ValidateModule(IReadOnlyList<ModuleInfoExtended> modules, ModuleInfoExtended targetModule, Func<ModuleInfoExtended, bool> isSelected)
        {
            var visited = new HashSet<ModuleInfoExtended>();
            foreach (var issue in ValidateModule(modules, targetModule, visited, isSelected, x => ValidateModule(modules, x, isSelected).Count() == 0))
                yield return issue;
        }
        /// <summary>
        /// Validates a module
        /// </summary>
        /// <param name="modules">All available modules</param>
        /// <returns>Any error that were detected during inspection</returns>
        public static IEnumerable<ModuleIssue> ValidateModule(IReadOnlyList<ModuleInfoExtended> modules, ModuleInfoExtended targetModule, Func<ModuleInfoExtended, bool> isSelected, Func<ModuleInfoExtended, bool> isValid)
        {
            var visited = new HashSet<ModuleInfoExtended>();
            foreach (var issue in ValidateModule(modules, targetModule, visited, isSelected, isValid))
                yield return issue;
        }
        /// <summary>
        /// Validates a module
        /// </summary>
        /// <param name="modules">All available modules</param>
        /// <returns>Any error that were detected during inspection</returns>
        public static IEnumerable<ModuleIssue> ValidateModule(IReadOnlyList<ModuleInfoExtended> modules, ModuleInfoExtended targetModule, HashSet<ModuleInfoExtended> visitedModules, Func<ModuleInfoExtended, bool> isSelected, Func<ModuleInfoExtended, bool> isValid)
        {
            // Validate common data
            foreach (var issue in ValidateModuleCommonData(targetModule))
                yield return issue;
            
            // Validate dependency declaration
            foreach (var issue in ValidateModuleDependenciesDeclarations(modules, targetModule))
                yield return issue;
            
            // Check that all dependencies are present
            foreach (var issue in ValidateModuleDependencies(modules, targetModule, visitedModules, isSelected, isValid))
                yield return issue;
        }

        /// <summary>
        /// Validates a module's common data
        /// </summary>
        public static IEnumerable<ModuleIssue> ValidateModuleCommonData(ModuleInfoExtended module)
        {
            if (string.IsNullOrWhiteSpace(module.Id))
            {
                yield return new ModuleIssue(module, "UNKNOWN", ModuleIssueType.MissingModuleId)
                {
                    Reason = $"Module Id is missing for '{module.Name}'"
                };
            }
            if (string.IsNullOrWhiteSpace(module.Name))
            {
                yield return new ModuleIssue(module, module.Id, ModuleIssueType.MissingModuleName)
                {
                    Reason = $"Module Name is missing in '{module.Id}'"
                };
            }
            foreach (var dependentModule in module.DependentModules)
            {
                if (dependentModule is null)
                {
                    yield return new ModuleIssue(module, "UNKNOWN", ModuleIssueType.DependencyIsNull)
                    {
                        Reason = $"Found a null dependency in '{module.Id}'",
                    };
                    break;
                }
                if (string.IsNullOrWhiteSpace(dependentModule.Id))
                {
                    yield return new ModuleIssue(module, "UNKNOWN", ModuleIssueType.DependencyMissingModuleId)
                    {
                        Reason = $"Module Id is missing for one if the dependencies of '{module.Id}'",
                    };
                }
            }
            foreach (var dependentModuleMetadata in module.DependentModuleMetadatas)
            {
                if (dependentModuleMetadata is null)
                {
                    yield return new ModuleIssue(module, "UNKNOWN", ModuleIssueType.DependencyIsNull)
                    {
                        Reason = $"Found a null dependency in '{module.Id}'",
                    };
                    break;
                }
                if (string.IsNullOrWhiteSpace(dependentModuleMetadata.Id))
                {
                    yield return new ModuleIssue(module, "UNKNOWN", ModuleIssueType.DependencyMissingModuleId)
                    {
                        Reason = $"Module Id is missing for one if the dependencies of '{module.Id}'",
                    };
                }
            }
        }
        
        /// <summary>
        /// Validates a module metadata
        /// </summary>
        /// <param name="modules">All available modules</param>
        /// <returns>Any error that were detected during inspection</returns>
        public static IEnumerable<ModuleIssue> ValidateModuleDependenciesDeclarations(IReadOnlyList<ModuleInfoExtended> modules, ModuleInfoExtended targetModule)
        {
            // Any Incompatible module is depended on
            foreach (var moduleId in targetModule.DependenciesToLoadDistinct().Select(x => x.Id).Intersect(targetModule.DependenciesIncompatiblesDistinct().Select(x => x.Id)))
            {
                yield return new ModuleIssue(targetModule, moduleId, ModuleIssueType.DependencyConflictDependentAndIncompatible)
                {
                    Reason = $"Module '{moduleId}' is both depended upon and marked as incompatible"
                };
            }
            // Check raw metadata too
            foreach (var dependency in targetModule.DependentModuleMetadatas.Where(x => x.IsIncompatible && x.LoadType != LoadType.None))
            {
                yield return new ModuleIssue(targetModule, dependency.Id, ModuleIssueType.DependencyConflictDependentAndIncompatible)
                {
                    Reason = $"Module '{dependency.Id}' is both depended upon and marked as incompatible"
                };
            }

            // LoadBeforeThis conflicts with LoadAfterThis
            foreach (var module in targetModule.DependenciesLoadBeforeThisDistinct())
            {
                if (targetModule.DependenciesLoadAfterThisDistinct().FirstOrDefault(x => string.Equals(x.Id, module.Id, StringComparison.Ordinal)) is { } metadata)
                {
                    yield return new ModuleIssue(targetModule, metadata.Id, ModuleIssueType.DependencyConflictDependentLoadBeforeAndAfter)
                    {
                        Reason = $"Module '{metadata.Id}' is both depended upon as LoadBefore and LoadAfter"
                    };
                }
            }

            // Circular dependency detection
            foreach (var module in targetModule.DependenciesToLoadDistinct().Where(x => x.LoadType != LoadType.None))
            {
                var moduleInfo = modules.FirstOrDefault(x => string.Equals(x.Id, module.Id, StringComparison.Ordinal));
                if (moduleInfo?.DependenciesToLoadDistinct().Where(x => x.LoadType != LoadType.None).FirstOrDefault(x => string.Equals(x.Id, targetModule.Id, StringComparison.Ordinal)) is { } metadata)
                {
                    if (metadata.LoadType == module.LoadType)
                    {
                        yield return new ModuleIssue(targetModule, metadata.Id, ModuleIssueType.DependencyConflictCircular)
                        {
                            Reason = $"Circular dependencies. '{targetModule.Id}' and '{moduleInfo.Id}' depend on each other"
                        };
                    }
                }
            }
        }
        /// <summary>
        /// Validates a module relative to other modules
        /// </summary>
        /// <param name="modules">All available modules</param>
        /// <param name="isValid">Whether another module is valid. Can be checked by this function</param>
        /// <returns>Any error that were detected during inspection</returns>
        public static IEnumerable<ModuleIssue> ValidateModuleDependencies(IReadOnlyList<ModuleInfoExtended> modules, ModuleInfoExtended targetModule, HashSet<ModuleInfoExtended> visitedModules, Func<ModuleInfoExtended, bool> isSelected, Func<ModuleInfoExtended, bool> isValid)
        {
            // Check that all dependencies are present
            foreach (var metadata in targetModule.DependenciesToLoadDistinct())
            {
                // Ignore the check for Optional
                if (metadata.IsOptional) continue;

                if (!modules.Any(x => string.Equals(x.Id, metadata.Id, StringComparison.Ordinal)))
                {
                    if (metadata.Version != ApplicationVersion.Empty)
                    {
                        yield return new ModuleIssue(targetModule, metadata.Id, ModuleIssueType.MissingDependencies)
                        {
                            Reason = $"Missing '{metadata.Id}' {metadata.Version}",
                            SourceVersion = new(metadata.Version, metadata.Version)
                        };
                    }
                    else if (metadata.VersionRange != ApplicationVersionRange.Empty)
                    {
                        yield return new ModuleIssue(targetModule, metadata.Id, ModuleIssueType.MissingDependencies)
                        {
                            Reason = $"Missing '{metadata.Id}' {metadata.VersionRange}",
                            SourceVersion = metadata.VersionRange
                        };
                    }
                    else
                    {
                        yield return new ModuleIssue(targetModule, metadata.Id, ModuleIssueType.MissingDependencies)
                        {
                            Reason = $"Missing '{metadata.Id}'"
                        };
                    }
                    yield break;
                }
            }
            
            // Check that the dependencies themselves have all dependencies present
            var opts = new ModuleSorterOptions { SkipOptionals = true, SkipExternalDependencies = true };
            var dependencies = GetDependencies(modules, targetModule, visitedModules, opts).ToArray();
            foreach (var dependency in dependencies)
            {
                if (targetModule.DependenciesAllDistinct().FirstOrDefault(x => string.Equals(x.Id, dependency.Id, StringComparison.Ordinal)) is { } metadata)
                {
                    // Not found, should not be possible
                    if (metadata is null) continue;
                    
                    // Handle only direct dependencies
                    if (metadata.LoadType != LoadType.LoadBeforeThis) continue;
                    
                    // Ignore the check for Optional
                    if (metadata.IsOptional) continue;
                    
                    // Ignore the check for Incompatible
                    if (metadata.IsIncompatible) continue;
                    
                    // Check missing dependency dependencies
                    if (modules.FirstOrDefault(x => string.Equals(x.Id, dependency.Id, StringComparison.Ordinal)) is not { } depencencyModuleInfo)
                    {
                        yield return new ModuleIssue(targetModule, dependency.Id, ModuleIssueType.DependencyMissingDependencies)
                        {
                            Reason = $"'{dependency.Id}' is missing it's dependencies!"
                        };
                        continue;
                    }

                    // Check depencency correctness
                    if (!isValid(depencencyModuleInfo))
                    {
                        yield return new ModuleIssue(targetModule, dependency.Id, ModuleIssueType.DependencyValidationError)
                        {
                            Reason = $"'{dependency.Id}' has unresolved issues!"
                        };
                    }
                }
            }

            // Check that the dependencies have the minimum required version set by DependedModuleMetadatas
            foreach (var metadata in targetModule.DependenciesToLoadDistinct())
            {
                // Ignore the check for empty versions
                if (metadata.Version == ApplicationVersion.Empty && metadata.VersionRange == ApplicationVersionRange.Empty) continue;

                // Dependency is loaded
                if (modules.FirstOrDefault(x => string.Equals(x.Id, metadata.Id, StringComparison.Ordinal)) is not { } metadataModule) continue;

                if (metadata.Version != ApplicationVersion.Empty)
                {
                    // dependedModuleMetadata.Version > dependedModule.Version
                    if (!metadata.IsOptional && (ApplicationVersionComparer.CompareStandard(metadata.Version, metadataModule.Version) > 0))
                    {
                        yield return new ModuleIssue(targetModule, metadataModule.Id, ModuleIssueType.VersionMismatchLessThanOrEqual)
                        {
                            Reason = $"'{metadataModule.Id}' wrong version <= {metadata.Version}",
                            SourceVersion = new(metadata.Version, metadata.Version)
                        };
                        continue;
                    }
                }
                if (metadata.VersionRange != ApplicationVersionRange.Empty)
                {
                    // dependedModuleMetadata.Version > dependedModule.VersionRange.Min
                    // dependedModuleMetadata.Version < dependedModule.VersionRange.Max
                    if (!metadata.IsOptional)
                    {
                        if (ApplicationVersionComparer.CompareStandard(metadata.VersionRange.Min, metadataModule.Version) > 0)
                        {
                            yield return new ModuleIssue(targetModule, metadataModule.Id, ModuleIssueType.VersionMismatchLessThan)
                            {
                                Reason = $"'{metadataModule?.Id}' wrong version < [{metadata.VersionRange}]",
                                SourceVersion = metadata.VersionRange
                            };
                            continue;
                        }
                        if (ApplicationVersionComparer.CompareStandard(metadata.VersionRange.Max, metadataModule.Version) < 0)
                        {
                            yield return new ModuleIssue(targetModule, metadataModule.Id, ModuleIssueType.VersionMismatchGreaterThan)
                            {
                                Reason = $"'{metadataModule.Id}' wrong version > [{metadata.VersionRange}]",
                                SourceVersion = metadata.VersionRange
                            };
                            continue;
                        }
                    }
                }
            }
            
            // Do not load this mod if an incompatible mod is selected
            foreach (var metadata in targetModule.DependenciesIncompatiblesDistinct())
            {
                // Dependency is loaded
                if (modules.FirstOrDefault(x => string.Equals(x.Id, metadata.Id, StringComparison.Ordinal)) is not { } metadataModule || !isSelected(metadataModule)) continue;

                // If the incompatible mod is selected, this mod should be disabled
                if (isSelected(metadataModule))
                {
                    yield return new ModuleIssue(targetModule, metadataModule.Id, ModuleIssueType.Incompatible)
                    {
                        Reason = $"'{metadataModule.Id}' is incompatible with this module"
                    };
                }
            }

            // If another mod declared incompatibility and is selected, disable this
            foreach (var module in modules)
            {
                // Ignore self
                if (string.Equals(module.Id, targetModule.Id, StringComparison.Ordinal)) continue;

                if (!isSelected(module)) continue;
                
                foreach (var metadata in module.DependenciesIncompatiblesDistinct())
                {
                    if (!string.Equals(metadata.Id, targetModule.Id, StringComparison.Ordinal)) continue;
                    
                    // If the incompatible mod is selected, this mod is disabled
                    if (isSelected(module))
                    {
                        yield return new ModuleIssue(targetModule, module.Id, ModuleIssueType.Incompatible)
                        {
                            Reason = $"'{module.Id}' is incompatible with this module"
                        };
                    }
                }
            }
        }
      
        /// <summary>
        /// Validates whether the load order is correctly sorted
        /// </summary>
        /// <param name="modules">Assumed that only valid and selected to launch modules are in the list</param>
        /// <param name="targetModule">Assumed that it's present in <paramref name="modules"/></param>
        /// <returns>Any error that were detected during inspection</returns>
        public static IEnumerable<ModuleIssue> ValidateLoadOrder(IReadOnlyList<ModuleInfoExtended> modules, ModuleInfoExtended targetModule)
        {
            var visited = new HashSet<ModuleInfoExtended>();
            foreach (var issue in ValidateLoadOrder(modules, targetModule, visited))
                yield return issue;
        }
        /// <summary>
        /// Validates whether the load order is correctly sorted
        /// </summary>
        /// <param name="modules">Assumed that only valid and selected to launch modules are in the list</param>
        /// <param name="targetModule">Assumed that it's present in <paramref name="modules"/></param>
        /// <param name="visitedModules">Used to track that we traverse each module only once</param>
        /// <returns>Any error that were detected during inspection</returns>
        public static IEnumerable<ModuleIssue> ValidateLoadOrder(IReadOnlyList<ModuleInfoExtended> modules, ModuleInfoExtended targetModule, HashSet<ModuleInfoExtended> visitedModules)
        {
            var targetModuleIdx = CollectionsExtensions.IndexOf(modules, targetModule);
            if (targetModuleIdx == -1)
            {
                yield return new ModuleIssue(targetModule, targetModule.Id, ModuleIssueType.Missing)
                {
                    Reason = $"Missing '{targetModule.Id}' {targetModule.Version} in modules list",
                    SourceVersion = new(targetModule.Version, targetModule.Version)
                };
                yield break;
            }

            // Validate common data
            foreach (var issue in ValidateModuleCommonData(targetModule))
                yield return issue;

            // Check that all dependencies are present
            foreach (var metadata in targetModule.DependenciesToLoad().DistinctBy(x => x.Id))
            {
                var metadataIdx = CollectionsExtensions.IndexOf(modules, x => x.Id == metadata.Id);
                if (metadataIdx == -1)
                {
                    if (!metadata.IsOptional)
                    {
                        if (metadata.Version != ApplicationVersion.Empty)
                        {
                            yield return new ModuleIssue(targetModule, metadata.Id, ModuleIssueType.MissingDependencies)
                            {
                                Reason = $"Missing '{metadata.Id}' {metadata.Version}",
                                SourceVersion = new(metadata.Version, metadata.Version)
                            };
                        }
                        else if (metadata.VersionRange != ApplicationVersionRange.Empty)
                        {
                            yield return new ModuleIssue(targetModule, metadata.Id, ModuleIssueType.MissingDependencies)
                            {
                                Reason = $"Missing '{metadata.Id}' {metadata.VersionRange}",
                                SourceVersion = metadata.VersionRange
                            };
                        }
                        else
                        {
                            yield return new ModuleIssue(targetModule, metadata.Id, ModuleIssueType.MissingDependencies)
                            {
                                Reason = $"Missing '{metadata.Id}'"
                            };
                        }
                    }
                    continue;
                }

                if (metadata.LoadType == LoadType.LoadBeforeThis && metadataIdx > targetModuleIdx)
                {
                    yield return new ModuleIssue(targetModule, metadata.Id, ModuleIssueType.DependencyNotLoadedBeforeThis)
                    {
                        Reason = $"'{metadata.Id}' should be loaded before '{targetModule.Id}'"
                    };
                }

                if (metadata.LoadType == LoadType.LoadAfterThis && metadataIdx < targetModuleIdx)
                {
                    yield return new ModuleIssue(targetModule, metadata.Id, ModuleIssueType.DependencyNotLoadedAfterThis)
                    {
                        Reason = $"'{metadata.Id}' should be loaded after '{targetModule.Id}'"
                    };
                }
            }
        }
        
        /// <summary>
        /// Will enable the target module and all its dependencies. Assumes that validation was being done before
        /// </summary>
        public static void EnableModule(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleInfoExtended targetModule, Func<ModuleInfoExtended, bool> getSelected, Action<ModuleInfoExtended, bool> setSelected, Func<ModuleInfoExtended, bool> getDisabled, Action<ModuleInfoExtended, bool> setDisabled)
        {
            var visited = new HashSet<ModuleInfoExtended>();
            EnableModuleInternal(modules, targetModule, visited, getSelected, setSelected, getDisabled, setDisabled);
        }
        private static void EnableModuleInternal(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleInfoExtended targetModule, HashSet<ModuleInfoExtended> visitedModules, Func<ModuleInfoExtended, bool> getSelected, Action<ModuleInfoExtended, bool> setSelected, Func<ModuleInfoExtended, bool> getDisabled, Action<ModuleInfoExtended, bool> setDisabled)
        {
            if (visitedModules.Contains(targetModule)) return;
            visitedModules.Add(targetModule);
            
            setSelected(targetModule, true);

            var opt = new ModuleSorterOptions { SkipOptionals = true, SkipExternalDependencies = true };
            var dependencies = GetDependencies(modules, targetModule, opt).ToArray();

            // Select all dependencies
            foreach (var module in modules)
            {
                if (!getSelected(module) && dependencies.Any(d => string.Equals(d.Id, module.Id, StringComparison.Ordinal)))
                {
                    EnableModuleInternal(modules, module, visitedModules, getSelected, setSelected, getDisabled, setDisabled);
                }
            }
            
            // Enable modules that are marked as LoadAfterThis
            foreach (var metadata in targetModule.DependenciesLoadAfterThisDistinct())
            {
                if (metadata.IsOptional) continue;
                
                if (metadata.IsIncompatible) continue;
                
                if (modules.FirstOrDefault(x => string.Equals(x.Id, metadata.Id, StringComparison.Ordinal)) is not { } metadataModule) continue;

                if (!getSelected(metadataModule))
                {
                    EnableModuleInternal(modules, metadataModule, visitedModules, getSelected, setSelected, getDisabled, setDisabled);
                }
            }

            // Deselect and disable any mod that is incompatible with this one
            foreach (var metadata in targetModule.DependenciesIncompatiblesDistinct())
            {
                if (modules.FirstOrDefault(x => string.Equals(x.Id, metadata.Id, StringComparison.Ordinal)) is not { } metadataModule) continue;
                
                if (getSelected(metadataModule))
                {
                    DisableModuleInternal(modules, metadataModule, visitedModules, getSelected, setSelected, getDisabled, setDisabled);
                }

                setDisabled(metadataModule, true);
            }

            // Disable any mod that declares this mod as incompatible
            foreach (var module in modules)
            {
                foreach (var metadata in module.DependenciesIncompatiblesDistinct())
                {
                    if (!string.Equals(metadata.Id, targetModule.Id, StringComparison.Ordinal)) continue;
                    
                    if (getSelected(module))
                    {
                        DisableModuleInternal(modules, module, visitedModules, getSelected, setSelected, getDisabled, setDisabled);
                    }

                    // We need to re-check that everything is alright with the external dependency
                    setDisabled(module, getDisabled(module) | !AreDependenciesPresent(modules, module));
                }
            }
        }
        
        /// <summary>
        /// Will disable the target module and all its dependencies. Assumes that validation was being done before
        /// </summary>
        public static void DisableModule(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleInfoExtended targetModule, Func<ModuleInfoExtended, bool> getSelected, Action<ModuleInfoExtended, bool> setSelected, Func<ModuleInfoExtended, bool> getDisabled, Action<ModuleInfoExtended, bool> setDisabled)
        {
            var visited = new HashSet<ModuleInfoExtended>();
            DisableModuleInternal(modules, targetModule, visited, getSelected, setSelected, getDisabled, setDisabled);
        }
        private static void DisableModuleInternal(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleInfoExtended targetModule, HashSet<ModuleInfoExtended> visitedModules, Func<ModuleInfoExtended, bool> getSelected, Action<ModuleInfoExtended, bool> setSelected, Func<ModuleInfoExtended, bool> getDisabled, Action<ModuleInfoExtended, bool> setDisabled)
        {
            if (visitedModules.Contains(targetModule)) return;
            visitedModules.Add(targetModule);
            
            setSelected(targetModule, false);

            var opt = new ModuleSorterOptions { SkipOptionals = true, SkipExternalDependencies = true };

            // Vanilla check
            // Deselect all modules that depend on this module if they are selected
            foreach (var module in modules)
            {
                var dependencies = ModuleUtilities.GetDependencies(modules, module, opt);
                if (getSelected(module) && dependencies.Any(d => string.Equals(d.Id, targetModule.Id, StringComparison.Ordinal)))
                {
                    DisableModuleInternal(modules, module, visitedModules, getSelected, setSelected, getDisabled, setDisabled);
                }
                
                // Enable modules that are marked as LoadAfterThis
                foreach (var metadata in module.DependenciesLoadAfterThisDistinct())
                {
                    if (!string.Equals(metadata.Id, targetModule.Id, StringComparison.Ordinal)) continue;
                    
                    if (metadata.IsOptional) continue;
                
                    if (metadata.IsIncompatible) continue;
                    
                    if (getSelected(module))
                    {
                        DisableModuleInternal(modules, module, visitedModules, getSelected, setSelected, getDisabled, setDisabled);
                    }
                }
                
                // Check if any mod that declares this mod as incompatible can be Enabled
                foreach (var metadata in module.DependenciesIncompatiblesDistinct())
                {
                    if (!string.Equals(metadata.Id, targetModule.Id, StringComparison.Ordinal)) continue;

                    // We need to re-check that everything is alright with the external dependency
                    setDisabled(module, getDisabled(module) & !ModuleUtilities.AreDependenciesPresent(modules, module));
                }
            }

            // Enable for selection any mod that is incompatible with this one
            foreach (var metadata in targetModule.DependenciesIncompatiblesDistinct())
            {
                if (modules.FirstOrDefault(x => string.Equals(x.Id, metadata.Id, StringComparison.Ordinal)) is not { } metadataModule) continue;
                setDisabled(metadataModule, false);
            }
        }
    }
}
#nullable restore
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning restore
#endif