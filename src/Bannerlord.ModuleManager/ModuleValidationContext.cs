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

    /// <summary>
    /// The module list of one validation, indexed: the validation looks modules up by ID and asks the same modules for
    /// their dependencies many times, a pass over the list each time made it O(n^2) and more per module.
    /// It also remembers which modules are valid when the validation of a module validates its dependencies
    /// </summary>
    internal sealed class ModuleValidationContext
    {
        private readonly ModuleIdDictionary<ModuleInfoExtended> _modulesById = new();
        private readonly Dictionary<ModuleInfoExtended, Metadatas> _metadatas = new(ModuleReferenceComparer.Instance);
        // True or false once known, null while the module is being validated
        private readonly Dictionary<ModuleInfoExtended, bool?> _validity = new(ModuleReferenceComparer.Instance);
        private ModuleIdDictionary<List<ModuleInfoExtended>>? _declaringIncompatibility;
        private ModuleDependencyIndex? _requiredDependencies;

        public IReadOnlyList<ModuleInfoExtended> Modules { get; }

        public Func<ModuleInfoExtended, bool> IsSelected { get; }

        public ModuleValidationContext(IReadOnlyList<ModuleInfoExtended> modules, Func<ModuleInfoExtended, bool>? isSelected)
        {
            Modules = modules;
            IsSelected = isSelected ?? (_ => throw new InvalidOperationException("The validation has no isSelected"));
            foreach (var module in modules)
                _modulesById.AddIfMissing(module.Id, module);
        }

        /// <summary>
        /// The first module with the ID, like FirstOrDefault over the list
        /// </summary>
        public ModuleInfoExtended? GetModule(string? id) => _modulesById.TryGetValue(id, out var module) ? module : null;

        public bool ContainsModule(string? id) => _modulesById.ContainsKey(id);

        /// <summary>
        /// The modules that declare the ID incompatible, in the order of the list
        /// </summary>
        public IReadOnlyList<ModuleInfoExtended> GetModulesDeclaringIncompatible(string? id)
        {
            if (_declaringIncompatibility is null)
            {
                _declaringIncompatibility = new ModuleIdDictionary<List<ModuleInfoExtended>>();
                foreach (var module in Modules)
                {
                    foreach (var metadata in GetIncompatibles(module))
                    {
                        if (!_declaringIncompatibility.TryGetValue(metadata.Id, out var declaring))
                        {
                            declaring = new List<ModuleInfoExtended>();
                            _declaringIncompatibility.Add(metadata.Id, declaring);
                        }
                        declaring.Add(module);
                    }
                }
            }
            return _declaringIncompatibility.TryGetValue(id, out var modules) ? modules : Array.Empty<ModuleInfoExtended>();
        }

        /// <summary>
        /// For GetDependencies with the required dependencies only, what the validation asks for
        /// </summary>
        public ModuleDependencyIndex RequiredDependencies =>
            _requiredDependencies ??= new ModuleDependencyIndex(Modules, new ModuleSorterOptions { SkipOptionals = true, SkipExternalDependencies = true });

        public DependentModuleMetadata[] GetAll(ModuleInfoExtended module) => GetMetadatas(module).All ??= module.DependenciesAllDistinct().ToArray();
        public DependentModuleMetadata[] GetToLoad(ModuleInfoExtended module) => GetMetadatas(module).ToLoad ??= module.DependenciesToLoadDistinct().ToArray();
        public DependentModuleMetadata[] GetLoadBeforeThis(ModuleInfoExtended module) => GetMetadatas(module).LoadBeforeThis ??= module.DependenciesLoadBeforeThisDistinct().ToArray();
        public DependentModuleMetadata[] GetLoadAfterThis(ModuleInfoExtended module) => GetMetadatas(module).LoadAfterThis ??= module.DependenciesLoadAfterThisDistinct().ToArray();
        public DependentModuleMetadata[] GetIncompatibles(ModuleInfoExtended module) => GetMetadatas(module).Incompatibles ??= module.DependenciesIncompatiblesDistinct().ToArray();

        private Metadatas GetMetadatas(ModuleInfoExtended module)
        {
            if (!_metadatas.TryGetValue(module, out var metadatas))
            {
                metadatas = new Metadatas();
                _metadatas.Add(module, metadatas);
            }
            return metadatas;
        }

        /// <summary>
        /// Whether the module has no issues, validated like the module that depends on it, once per module.
        /// A module reached again while it's being validated is in a dependency cycle and isn't valid: the validation
        /// of a cycle of three or more modules recursed until the stack overflowed before
        /// </summary>
        public bool IsValid(ModuleInfoExtended module)
        {
            if (_validity.TryGetValue(module, out var validity))
                return validity ?? false;

            _validity[module] = null;
            var isValid = !ModuleUtilities.ValidateModuleEx(this, module, new HashSet<ModuleInfoExtended>(), IsValid, true).Any();
            _validity[module] = isValid;
            return isValid;
        }

        private sealed class Metadatas
        {
            public DependentModuleMetadata[]? All;
            public DependentModuleMetadata[]? ToLoad;
            public DependentModuleMetadata[]? LoadBeforeThis;
            public DependentModuleMetadata[]? LoadAfterThis;
            public DependentModuleMetadata[]? Incompatibles;
        }
    }
#nullable restore
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning restore
#endif
}
