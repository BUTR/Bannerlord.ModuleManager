using System;
using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.ModuleManager.Tests
{
    /// <summary>
    /// ModuleUtilities' GetDependencies, EnableModule and DisableModule before the ModuleDependencyIndex, unchanged,
    /// and ModuleSorter.Sort on top of them: what the indexed ones must match
    /// </summary>
    internal static class ReferenceModuleUtilities
    {
        // ModuleSorter's comparer, it's internal to the library
        private static IComparer<string> NewAlphanumComparatorFast() => (IComparer<string>) Activator.CreateInstance(
            typeof(ModuleSorter).Assembly.GetType("Bannerlord.ModuleManager.AlphanumComparatorFast", true)!, true)!;

        public static IList<ModuleInfoExtended> Sort(IReadOnlyCollection<ModuleInfoExtended> source)
        {
            var correctModules = source
                .Where(x => ModuleUtilities.AreDependenciesPresent(source, x))
                .OrderByDescending(x => x.IsOfficial)
                .ThenByDescending(mim => mim.Id, NewAlphanumComparatorFast())
                .ToArray();

            return ModuleSorter.TopologySort(correctModules, module => GetDependencies(correctModules, module));
        }
        public static IList<ModuleInfoExtended> Sort(IReadOnlyCollection<ModuleInfoExtended> source, ModuleSorterOptions options)
        {
            var correctModules = source
                .Where(x => ModuleUtilities.AreDependenciesPresent(source, x))
                .OrderByDescending(x => x.IsOfficial)
                .ThenByDescending(mim => mim.Id, NewAlphanumComparatorFast())
                .ToArray();

            return ModuleSorter.TopologySort(correctModules, module => GetDependencies(correctModules, module, options));
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
                    setDisabled(module, getDisabled(module) | !ModuleUtilities.AreDependenciesPresent(modules, module));
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
