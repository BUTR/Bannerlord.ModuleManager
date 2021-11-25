﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.ModuleManager
{
    public static class ModuleSorter
    {
        public static IList<ModuleInfoExtended> Sort(ICollection<ModuleInfoExtended> source)
        {
            var correctModules = source
                .Where(x => AreAllDependenciesOfModulePresent(source, x))
                .OrderByDescending(mim => mim.Id, new AlphanumComparatorFast())
                .ToArray();

            return TopologySort(correctModules, module => GetDependentModulesOf(correctModules, module));
        }

        public static IList<T> TopologySort<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
        {
            var list = new List<T>();
            var visited = new HashSet<T>();
            foreach (var item in source)
            {
                Visit(item, getDependencies, list, visited);
            }
            return list;
        }

        public static void Visit<T>(T item, Func<T, IEnumerable<T>> getDependencies, ICollection<T> sorted, HashSet<T> visited)
        {
            if (visited.Contains(item))
            {
                return;
            }

            visited.Add(item);
            if (getDependencies(item) is { } enumerable)
            {
                foreach (var item2 in enumerable)
                {
                    Visit(item2, getDependencies, sorted, visited);
                }
            }
            sorted.Add(item);
        }

        public static bool AreAllDependenciesOfModulePresent(ICollection<ModuleInfoExtended> source, ModuleInfoExtended info)
        {
            foreach (var dependentModule in info.DependentModules)
            {
                if (source.All(m => m.Id != dependentModule.Id))
                {
                    return false;
                }
            }
            foreach (var dependentModuleMetadata in info.DependentModuleMetadatas)
            {
                if (dependentModuleMetadata.IsIncompatible)
                {
                    if (source.Any(m => m.Id == dependentModuleMetadata.Id))
                    {
                        return false;
                    }
                }
                if (dependentModuleMetadata.IsOptional)
                {
                    continue;
                }
                if (dependentModuleMetadata.LoadType != LoadType.LoadBeforeThis)
                {
                    continue;
                }

                if (source.All(m => m.Id != dependentModuleMetadata.Id))
                {
                    return false;
                }
            }
            return true;
        }

        public static IEnumerable<ModuleInfoExtended> GetDependentModulesOf(IEnumerable<ModuleInfoExtended> source, ModuleInfoExtended module, bool skipExternalDependencies = false)
        {
            var visited = new HashSet<ModuleInfoExtended>();
            return GetDependentModulesOf(source, module, visited, skipExternalDependencies);
        }

        public static IEnumerable<ModuleInfoExtended> GetDependentModulesOf(IEnumerable<ModuleInfoExtended> source, ModuleInfoExtended module, HashSet<ModuleInfoExtended> visited, bool skipExternalDependencies = false)
        {
            var dependencies = new List<ModuleInfoExtended>();
            Visit(module, x => GetDependentModulesOfInternal(source, x, skipExternalDependencies), dependencies, visited);
            return dependencies;
        }


        private static IEnumerable<ModuleInfoExtended> GetDependentModulesOfInternal(IEnumerable<ModuleInfoExtended> source, ModuleInfoExtended module, bool skipExternalDependencies = false)
        {
            var sourceList = source.ToList();

            foreach (var dependentModule in module.DependentModules)
            {
                if (sourceList.Find(i => i.Id == dependentModule.Id) is { } moduleInfo)
                {
                    yield return moduleInfo;
                }
            }

            foreach (var dependentModuleMetadata in module.DependentModuleMetadatas)
            {
                if (dependentModuleMetadata.LoadType != LoadType.LoadBeforeThis)
                {
                    continue;
                }

                var moduleInfo = sourceList.Find(i => i.Id == dependentModuleMetadata.Id);
                if (!dependentModuleMetadata.IsOptional && moduleInfo is null)
                {
                    // We should not hit this place. If we do, the module list is invalid
                }
                else if (moduleInfo is not null)
                {
                    yield return moduleInfo;
                }
            }

            if (!skipExternalDependencies)
            {
                foreach (var moduleInfo in sourceList)
                {
                    foreach (var dependentModuleMetadata in moduleInfo.DependentModuleMetadatas)
                    {
                        if (dependentModuleMetadata.LoadType != LoadType.LoadAfterThis)
                        {
                            continue;
                        }

                        if (dependentModuleMetadata.Id != module.Id)
                        {
                            continue;
                        }

                        yield return moduleInfo;
                    }
                }
            }
        }
    }
}