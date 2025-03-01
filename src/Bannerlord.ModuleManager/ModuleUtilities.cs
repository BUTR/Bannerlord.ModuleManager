﻿#region License
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
    using Models.Issues;

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

        #region Polyfills
        /// <summary>
        /// Validates a module
        /// </summary>
        /// <param name="modules">All available modules (in any order)</param>
        /// <param name="targetModule">The module to validate</param>
        /// <param name="isSelected">Function that determines if a module is selected (is enabled)</param>
        /// <returns>Any errors that were detected during inspection</returns>
        public static IEnumerable<ModuleIssue> ValidateModule(
            IReadOnlyList<ModuleInfoExtended> modules, 
            ModuleInfoExtended targetModule, 
            Func<ModuleInfoExtended, bool> isSelected)
        {
            var visited = new HashSet<ModuleInfoExtended>();
            return ValidateModuleEx(modules, targetModule, visited, isSelected, 
                    x => !ValidateModuleEx(modules, x, isSelected).Any())
                .Select(x => x.ToLegacy());
        }

        /// <summary>
        /// Validates a module
        /// </summary>
        /// <param name="modules">All available modules (in any order)</param>
        /// <param name="targetModule">The module to validate</param>
        /// <param name="isSelected">Function that determines if a module is selected (is enabled)</param>
        /// <param name="isValid">Function that determines if a module is valid</param>
        /// <returns>Any errors that were detected during inspection</returns>
        public static IEnumerable<ModuleIssue> ValidateModule(
            IReadOnlyList<ModuleInfoExtended> modules, 
            ModuleInfoExtended targetModule, 
            Func<ModuleInfoExtended, bool> isSelected, 
            Func<ModuleInfoExtended, bool> isValid)
        {
            var visited = new HashSet<ModuleInfoExtended>();
            return ValidateModuleEx(modules, targetModule, visited, isSelected, isValid)
                .Select(x => x.ToLegacy());
        }

        /// <summary>
        /// Validates a module's common data
        /// </summary>
        /// <param name="module">The module to validate</param>
        /// <returns>Any errors that were detected during inspection of common data</returns>
        public static IEnumerable<ModuleIssue> ValidateModuleCommonData(ModuleInfoExtended module) =>
            ValidateModuleCommonDataEx(module).Select(x => x.ToLegacy());

        /// <summary>
        /// Validates a module's dependency declarations
        /// </summary>
        /// <param name="modules">All available modules (in any order)</param>
        /// <param name="targetModule">The module whose dependencies are being validated</param>
        /// <returns>Any errors that were detected during inspection of dependencies</returns>
        public static IEnumerable<ModuleIssue> ValidateModuleDependenciesDeclarations(
            IReadOnlyList<ModuleInfoExtended> modules, 
            ModuleInfoExtended targetModule) =>
            ValidateModuleDependenciesDeclarationsEx(modules, targetModule).Select(x => x.ToLegacy());

        /// <summary>
        /// Validates module dependencies
        /// </summary>
        /// <param name="modules">All available modules (in any order)</param>
        /// <param name="targetModule">The module whose dependencies are being validated</param>
        /// <param name="visitedModules">Set of modules already validated to prevent cycles</param>
        /// <param name="isSelected">Function that determines if a module is selected</param>
        /// <param name="isValid">Function that determines if a module is valid</param>
        /// <returns>Any errors that were detected during inspection of dependencies</returns>
        public static IEnumerable<ModuleIssue> ValidateModuleDependencies(
            IReadOnlyList<ModuleInfoExtended> modules,
            ModuleInfoExtended targetModule,
            HashSet<ModuleInfoExtended> visitedModules,
            Func<ModuleInfoExtended, bool> isSelected,
            Func<ModuleInfoExtended, bool> isValid) =>
            ValidateModuleDependenciesEx(modules, targetModule, visitedModules, isSelected, isValid)
                .Select(x => x.ToLegacy());

        /// <summary>
        /// Validates whether the load order is correctly sorted
        /// </summary>
        /// <param name="modules">All available modules in they order they are expected to be loaded in</param>
        /// <param name="targetModule">Assumed that it's present in <paramref name="modules"/></param>
        /// <returns>Any errors that were detected during inspection</returns>
        public static IEnumerable<ModuleIssue> ValidateLoadOrder(
            IReadOnlyList<ModuleInfoExtended> modules, 
            ModuleInfoExtended targetModule)
        {
            foreach (var issue in ValidateModuleCommonData(targetModule))
                yield return issue;

            var visited = new HashSet<ModuleInfoExtended>();
            foreach (var issue in ValidateLoadOrderEx(modules, targetModule, visited)
                         .Select(x => x.ToLegacy()))
                yield return issue;
        }

        /// <summary>
        /// Validates whether the load order is correctly sorted
        /// </summary>
        /// <param name="modules">All available modules in they order they are expected to be loaded in</param>
        /// <param name="targetModule">Assumed that it's present in <paramref name="modules"/></param>
        /// <param name="visitedModules">Used to track that we traverse each module only once</param>
        /// <returns>Any errors that were detected during inspection</returns>
        public static IEnumerable<ModuleIssue> ValidateLoadOrder(
            IReadOnlyList<ModuleInfoExtended> modules, 
            ModuleInfoExtended targetModule, 
            HashSet<ModuleInfoExtended> visitedModules) =>
            ValidateLoadOrderEx(modules, targetModule, visitedModules).Select(x => x.ToLegacy());
        #endregion

        /// <summary>
        /// Validates a module using the new variant-based issue system
        /// </summary>
        /// <param name="modules">All available modules (in any order)</param>
        /// <param name="targetModule">The module to validate</param>
        /// <param name="isSelected">Function that determines if a module is enabled</param>
        /// <returns>Any errors that were detected during inspection</returns>
        public static IEnumerable<ModuleIssueV2> ValidateModuleEx(
            IReadOnlyList<ModuleInfoExtended> modules,
            ModuleInfoExtended targetModule,
            Func<ModuleInfoExtended, bool> isSelected)
        {
            var visited = new HashSet<ModuleInfoExtended>();
            foreach (var issue in ValidateModuleEx(modules, targetModule, visited, isSelected, x => !ValidateModuleEx(modules, x, isSelected).Any()))
            {
                yield return issue;
            }
        }

        /// <summary>
        /// Validates a module using the new variant-based issue system
        /// </summary>
        /// <param name="modules">All available modules (in any order)</param>
        /// <param name="targetModule">The module to validate</param>
        /// <param name="isSelected">Function that determines if a module is enabled</param>
        /// <param name="isValid">Function that determines if a module is valid</param>
        /// <param name="validateDependencies">Set this to true to also report errors in the target module's dependencies. e.g. Missing dependencies of dependencies.</param>
        /// <returns>Any errors that were detected during inspection</returns>
        public static IEnumerable<ModuleIssueV2> ValidateModuleEx(
            IReadOnlyList<ModuleInfoExtended> modules,
            ModuleInfoExtended targetModule,
            Func<ModuleInfoExtended, bool> isSelected,
            Func<ModuleInfoExtended, bool> isValid,
            bool validateDependencies = true)
        {
            var visited = new HashSet<ModuleInfoExtended>();
            foreach (var issue in ValidateModuleEx(modules, targetModule, visited, isSelected, isValid, validateDependencies))
            {
                yield return issue;
            }
        }

        /// <summary>
        /// Internal validation method that handles the core validation logic
        /// </summary>
        /// <param name="modules">All available modules (in any order)</param>
        /// <param name="targetModule">The module to validate</param>
        /// <param name="visitedModules">Set of modules already validated to prevent cycles</param>
        /// <param name="isSelected">Function that determines if a module is enabled</param>
        /// <param name="isValid">Function that determines if a module is valid</param>
        /// <param name="validateDependencies">Set this to true to also report errors in the target module's dependencies. e.g. Missing dependencies of dependencies.</param>
        /// <returns>Any errors that were detected during inspection</returns>
        public static IEnumerable<ModuleIssueV2> ValidateModuleEx(
            IReadOnlyList<ModuleInfoExtended> modules,
            ModuleInfoExtended targetModule,
            HashSet<ModuleInfoExtended> visitedModules,
            Func<ModuleInfoExtended, bool> isSelected,
            Func<ModuleInfoExtended, bool> isValid,
            bool validateDependencies = true)
        {
            foreach (var issue in ValidateModuleCommonDataEx(targetModule))
                yield return issue;
            
            foreach (var issue in ValidateModuleDependenciesDeclarationsEx(modules, targetModule))
                yield return issue;
            
            foreach (var issue in ValidateModuleDependenciesEx(modules, targetModule, visitedModules, isSelected, isValid, validateDependencies))
                yield return issue;
        }

        /// <summary>
        /// Validates a module's common data using the new variant-based issue system
        /// </summary>
        /// <param name="module">The module whose common data is being validated</param>
        /// <returns>Any errors that were detected during inspection</returns>
        public static IEnumerable<ModuleIssueV2> ValidateModuleCommonDataEx(ModuleInfoExtended module)
        {
            if (string.IsNullOrWhiteSpace(module.Id))
                yield return new ModuleMissingIdIssue(module);

            if (string.IsNullOrWhiteSpace(module.Name))
                yield return new ModuleMissingNameIssue(module);

            foreach (var dependentModule in module.DependentModules.Where(x => x is not null))
            {
                if (dependentModule is null)
                {
                    yield return new ModuleDependencyNullIssue(module);
                    break;
                }
                if (string.IsNullOrWhiteSpace(dependentModule.Id))
                    yield return new ModuleDependencyMissingIdIssue(module);
            }

            foreach (var dependentModuleMetadata in module.DependentModuleMetadatas.Where(x => x is not null))
            {
                if (dependentModuleMetadata is null)
                {
                    yield return new ModuleDependencyNullIssue(module);
                    break;
                }
                if (string.IsNullOrWhiteSpace(dependentModuleMetadata.Id))
                    yield return new ModuleDependencyMissingIdIssue(module);
            }
        }
        
        /// <summary>
        /// Validates module dependencies declarations using the new variant-based issue system
        /// </summary>
        /// <param name="modules">All available modules (in any order)</param>
        /// <param name="targetModule">The module whose dependency declarations are being validated</param>
        /// <returns>Any errors that were detected during inspection</returns>
        public static IEnumerable<ModuleIssueV2> ValidateModuleDependenciesDeclarationsEx(
            IReadOnlyList<ModuleInfoExtended> modules,
            ModuleInfoExtended targetModule)
        {
            // Any Incompatible module is depended on
            // TODO: Will a null Id break things?
            foreach (var moduleId in targetModule.DependenciesToLoadDistinct()
                         .Select(x => x.Id)
                         .Intersect(targetModule.DependenciesIncompatiblesDistinct().Select(x => x.Id)))
            {
                yield return new ModuleDependencyConflictDependentAndIncompatibleIssue(targetModule, moduleId);
            }

            // Check raw metadata too
            foreach (var dependency in targetModule.DependentModuleMetadatas
                         .Where(x => x is not null)
                         .Where(x => x.IsIncompatible && x.LoadType != LoadType.None))
            {
                yield return new ModuleDependencyConflictDependentAndIncompatibleIssue(targetModule, dependency.Id);
            }

            // LoadBeforeThis conflicts with LoadAfterThis
            foreach (var module in targetModule.DependenciesLoadBeforeThisDistinct())
            {
                if (targetModule.DependenciesLoadAfterThisDistinct()
                        .FirstOrDefault(x => string.Equals(x.Id, module.Id, StringComparison.Ordinal)) is { } metadata)
                {
                    yield return new ModuleDependencyConflictLoadBeforeAndAfterIssue(targetModule, metadata);
                }
            }

            // Circular dependency detection
            foreach (var module in targetModule.DependenciesToLoadDistinct().Where(x => x.LoadType != LoadType.None))
            {
                var moduleInfo = modules.FirstOrDefault(x => string.Equals(x.Id, module.Id, StringComparison.Ordinal));
                if (moduleInfo?.DependenciesToLoadDistinct()
                        .Where(x => x.LoadType != LoadType.None)
                        .FirstOrDefault(x => string.Equals(x.Id, targetModule.Id, StringComparison.Ordinal)) is { } metadata)
                {
                    if (metadata.LoadType != module.LoadType) 
                        continue;

                    // Find the full module with given ID.
                    var fullModule = modules.First(x => moduleInfo.Id == x.Id);
                    yield return new ModuleDependencyConflictCircularIssue(targetModule, fullModule);
                }
            }
        }

        /// <summary>
        /// Validates module dependencies using the new variant-based issue system
        /// </summary>
        /// <param name="modules">All available modules (in any order)</param>
        /// <param name="targetModule">The module whose dependencies are being validated</param>
        /// <param name="visitedModules">Set of modules already validated to prevent cycles</param>
        /// <param name="isSelected">Function that determines if a module is enabled</param>
        /// <param name="isValid">Function that determines if a module is valid</param>
        /// <param name="validateDependencies">Set this to true to also report errors in the target module's dependencies. e.g. Missing dependencies of dependencies.</param>
        /// <returns>Any errors that were detected during inspection</returns>
        private static IEnumerable<ModuleIssueV2> ValidateModuleDependenciesEx(
            IReadOnlyList<ModuleInfoExtended> modules,
            ModuleInfoExtended targetModule,
            HashSet<ModuleInfoExtended> visitedModules,
            Func<ModuleInfoExtended, bool> isSelected,
            Func<ModuleInfoExtended, bool> isValid,
            bool validateDependencies = true)
        {
            // Check that all dependencies are present
            foreach (var metadata in targetModule.DependenciesToLoadDistinct())
            {
                // Ignore the check if the Id is incorrect
                if (string.IsNullOrWhiteSpace(metadata.Id)) continue;

                // Ignore the check for Optional
                if (metadata.IsOptional) continue;

                if (!modules.Any(x => string.Equals(x.Id, metadata.Id, StringComparison.Ordinal)))
                {
                    // For BLSE, there is a special case, 
                    if (metadata.Id is "BLSE.LoadingInterceptor" or "BLSE.AssemblyResolver")
                        yield return new ModuleMissingBLSEDependencyIssue(targetModule, metadata);
                    else if (metadata.Version != ApplicationVersion.Empty)
                        yield return new ModuleMissingExactVersionDependencyIssue(targetModule, metadata);
                    else if (metadata.VersionRange != ApplicationVersionRange.Empty)
                        yield return new ModuleMissingVersionRangeDependencyIssue(targetModule, metadata);
                    else
                        yield return new ModuleMissingUnversionedDependencyIssue(targetModule, metadata);
                    yield break;
                }
            }

            // Check that the dependencies themselves have all dependencies present
            if (validateDependencies)
            {
                var opts = new ModuleSorterOptions { SkipOptionals = true, SkipExternalDependencies = true };
                var dependencies = GetDependencies(modules, targetModule, visitedModules, opts).ToArray();
                foreach (var dependency in dependencies)
                {
                    if (targetModule.DependenciesAllDistinct().FirstOrDefault(x => 
                            string.Equals(x.Id, dependency.Id, StringComparison.Ordinal)) is { } metadata)
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
                        if (modules.FirstOrDefault(x => string.Equals(x.Id, dependency.Id, StringComparison.Ordinal)) is not { } dependencyModuleInfo)
                        {
                            yield return new ModuleDependencyMissingDependenciesIssue(targetModule, dependency.Id);
                            continue;
                        }

                        // Check dependency correctness
                        if (!isValid(dependencyModuleInfo))
                            yield return new ModuleDependencyValidationIssue(targetModule, dependency.Id);
                    }
                }
            }

            // Check that the dependencies have the minimum required version set by DependedModuleMetadatas
            foreach (var metadata in targetModule.DependenciesToLoadDistinct())
            {
                // Ignore the check for empty versions
                if (metadata.Version == ApplicationVersion.Empty && metadata.VersionRange == ApplicationVersionRange.Empty) 
                    continue;

                // Dependency is loaded
                if (modules.FirstOrDefault(x => string.Equals(x.Id, metadata.Id, StringComparison.Ordinal)) is not { } metadataModule) 
                    continue;

                if (metadata.Version != ApplicationVersion.Empty)
                {
                    // dependedModuleMetadata.Version > dependedModule.Version
                    if (!metadata.IsOptional && (ApplicationVersionComparer.CompareStandard(metadata.Version, metadataModule.Version) > 0))
                    {
                        yield return new ModuleVersionTooLowIssue(
                            targetModule,
                            metadataModule,
                            metadata.Version);
                        continue;
                    }
                }

                if (metadata.VersionRange != ApplicationVersionRange.Empty && !metadata.IsOptional)
                {
                    // dependedModuleMetadata.Version > dependedModule.VersionRange.Min
                    // dependedModuleMetadata.Version < dependedModule.VersionRange.Max
                    if (ApplicationVersionComparer.CompareStandard(metadata.VersionRange.Min, metadataModule.Version) > 0)
                    {
                        yield return new ModuleVersionMismatchLessThanRangeIssue(
                            targetModule,
                            metadataModule,
                            metadata.VersionRange);
                        continue;
                    }
                    if (ApplicationVersionComparer.CompareStandard(metadata.VersionRange.Max, metadataModule.Version) < 0)
                    {
                        yield return new ModuleVersionMismatchGreaterThanRangeIssue(
                            targetModule,
                            metadataModule,
                            metadata.VersionRange);
                        continue;
                    }
                }
            }

            // Do not load this mod if an incompatible mod is selected
            foreach (var metadata in targetModule.DependenciesIncompatiblesDistinct())
            {
                // Dependency is loaded
                if (modules.FirstOrDefault(x => string.Equals(x.Id, metadata.Id, StringComparison.Ordinal)) is not { } metadataModule || 
                    !isSelected(metadataModule)) continue;

                // If the incompatible mod is selected, this mod should be disabled
                if (isSelected(metadataModule))
                    yield return new ModuleIncompatibleIssue(targetModule, metadataModule);
            }

            // If another mod declared incompatibility and is selected, disable this
            if (validateDependencies)
            {
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
                            yield return new ModuleIncompatibleIssue(targetModule, module);
                    }
                }
            }
        }
      
        /// <summary>
        /// Validates whether the load order is correctly sorted using the new variant-based issue system
        /// </summary>
        /// <param name="modules">All available modules in they order they are expected to be loaded in</param>
        /// <param name="targetModule">The module whose load order is being validated</param>
        /// <returns>Any errors that were detected during inspection</returns>
        public static IEnumerable<ModuleIssueV2> ValidateLoadOrderEx(
            IReadOnlyList<ModuleInfoExtended> modules, 
            ModuleInfoExtended targetModule)
        {
            foreach (var issue in ValidateModuleCommonDataEx(targetModule))
                yield return issue;

            var visited = new HashSet<ModuleInfoExtended>();
            foreach (var issue in ValidateLoadOrderEx(modules, targetModule, visited))
                yield return issue;
        }

        /// <summary>
        /// Validates whether the load order is correctly sorted using the new variant-based issue system
        /// </summary>
        /// <param name="modules">All available modules in they order they are expected to be loaded in</param>
        /// <param name="targetModule">The module whose load order is being validated</param>
        /// <param name="visitedModules">Set of modules already validated to prevent cycles</param>
        /// <returns>Any errors that were detected during inspection</returns>
        /// <remarks>
        ///     In this case 'load order' means the set of enabled mods.
        /// </remarks>
        public static IEnumerable<ModuleIssueV2> ValidateLoadOrderEx(
            IReadOnlyList<ModuleInfoExtended> modules,
            ModuleInfoExtended targetModule,
            HashSet<ModuleInfoExtended> visitedModules)
        {
            var targetModuleIdx = CollectionsExtensions.IndexOf(modules, targetModule);
            if (targetModuleIdx == -1)
            {
                yield return new ModuleMissingIssue(
                    targetModule, 
                    new ApplicationVersionRange(targetModule.Version, targetModule.Version));
                yield break;
            }

            // Check that all dependencies are present
            foreach (var metadata in targetModule.DependenciesToLoad().DistinctBy(x => x.Id))
            {
                var metadataIdx = CollectionsExtensions.IndexOf(modules, x => 
                    string.Equals(x.Id, metadata.Id, StringComparison.Ordinal));
                
                if (metadataIdx == -1)
                {
                    // For BLSE, there is a special case, 
                    if (metadata.Id is "BLSE.LoadingInterceptor" or "BLSE.AssemblyResolver")
                        yield return new ModuleMissingBLSEDependencyIssue(targetModule, metadata);
                
                    // If the dependency lacks an Id, it's not valid
                    else if (!string.IsNullOrWhiteSpace(metadata.Id) && !metadata.IsOptional)
                    {
                        if (metadata.Version != ApplicationVersion.Empty)
                            yield return new ModuleMissingExactVersionDependencyIssue(targetModule, metadata);
                        else if (metadata.VersionRange != ApplicationVersionRange.Empty)
                            yield return new ModuleMissingVersionRangeDependencyIssue(targetModule, metadata);
                        else
                            yield return new ModuleMissingUnversionedDependencyIssue(targetModule, metadata);
                    }
                    continue;
                }

                // TODO(sewer): Return full module here later. Right now I don't have a good way to test some assertions.
                if (metadata.LoadType == LoadType.LoadBeforeThis && metadataIdx > targetModuleIdx)
                {
                    yield return new ModuleDependencyNotLoadedBeforeIssue(targetModule, metadata);
                }

                if (metadata.LoadType == LoadType.LoadAfterThis && metadataIdx < targetModuleIdx)
                {
                    yield return new ModuleDependencyNotLoadedAfterIssue(targetModule, metadata);
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
            if (!visitedModules.Add(targetModule)) return;

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
            if (!visitedModules.Add(targetModule)) return;

            setSelected(targetModule, false);

            var opt = new ModuleSorterOptions { SkipOptionals = true, SkipExternalDependencies = true };

            // Vanilla check
            // Deselect all modules that depend on this module if they are selected
            foreach (var module in modules)
            {
                var dependencies = GetDependencies(modules, module, opt);
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
                    setDisabled(module, getDisabled(module) & !AreDependenciesPresent(modules, module));
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
#nullable restore
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning restore
#endif
}