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
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A module list prepared for <see cref="ModuleUtilities.GetDependencies(ModuleDependencyIndex, ModuleInfoExtended)"/>:
    /// modules by ID and the modules that must load after each ID, so finding a module's direct dependencies is a lookup
    /// instead of a pass over the whole list. Build one per list and reuse it for every module of the list, like a sort does.
    /// It doesn't follow changes to the list, build a new one then.
    /// </summary>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
    internal
#else
    public
# endif
        sealed class ModuleDependencyIndex
    {
        private readonly bool _skipOptionals;
        private readonly bool _skipExternalDependencies;

        // The first module with each ID, the one a FirstOrDefault over the list finds
        private readonly Dictionary<string, ModuleInfoExtended> _modulesById = new(StringComparer.Ordinal);

        // For each ID, the modules that declare LoadAfterThis for it, in the order of the list
        private readonly Dictionary<string, List<ModuleInfoExtended>> _modulesLoadingAfter = new(StringComparer.Ordinal);

        // Each module's distinct LoadBeforeThis dependencies, computed once
        private readonly Dictionary<ModuleInfoExtended, DependentModuleMetadata[]> _loadBeforeThis = new(ReferenceComparer.Instance);

        public IReadOnlyCollection<ModuleInfoExtended> Modules { get; }

        public ModuleDependencyIndex(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleSorterOptions options)
        {
            Modules = modules;
            _skipOptionals = options.SkipOptionals;
            _skipExternalDependencies = options.SkipExternalDependencies;

            foreach (var module in modules)
            {
                if (!_modulesById.ContainsKey(module.Id))
                    _modulesById.Add(module.Id, module);

                if (_skipExternalDependencies)
                    continue;

                foreach (var metadata in module.DependenciesLoadAfterThisDistinct())
                {
                    if (metadata.IsOptional && _skipOptionals)
                        continue;

                    if (!_modulesLoadingAfter.TryGetValue(metadata.Id, out var loadingAfter))
                    {
                        loadingAfter = new List<ModuleInfoExtended>();
                        _modulesLoadingAfter.Add(metadata.Id, loadingAfter);
                    }
                    loadingAfter.Add(module);
                }
            }
        }

        /// <summary>
        /// The modules of the list that must load before <paramref name="module"/>: the ones it depends on, then the ones
        /// that declare they load after it
        /// </summary>
        public IEnumerable<ModuleInfoExtended> GetDirectDependencies(ModuleInfoExtended module)
        {
            foreach (var metadata in GetLoadBeforeThis(module))
            {
                if (metadata.IsOptional && _skipOptionals)
                    continue;

                // A required one that's missing makes the list invalid, it's skipped like an optional one
                if (_modulesById.TryGetValue(metadata.Id, out var moduleInfo))
                    yield return moduleInfo;
            }

            if (!_skipExternalDependencies && _modulesLoadingAfter.TryGetValue(module.Id, out var loadingAfter))
            {
                foreach (var moduleInfo in loadingAfter)
                    yield return moduleInfo;
            }
        }

        private DependentModuleMetadata[] GetLoadBeforeThis(ModuleInfoExtended module)
        {
            if (!_loadBeforeThis.TryGetValue(module, out var metadatas))
            {
                metadatas = module.DependenciesLoadBeforeThisDistinct().ToArray();
                _loadBeforeThis.Add(module, metadatas);
            }
            return metadatas;
        }

        // Records compare by value, a cache only needs the same instance
        private sealed class ReferenceComparer : IEqualityComparer<ModuleInfoExtended>
        {
            public static readonly ReferenceComparer Instance = new();

            public bool Equals(ModuleInfoExtended? x, ModuleInfoExtended? y) => ReferenceEquals(x, y);

            public int GetHashCode(ModuleInfoExtended obj) => RuntimeHelpers.GetHashCode(obj);
        }
    }
#nullable restore
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning restore
#endif
}
