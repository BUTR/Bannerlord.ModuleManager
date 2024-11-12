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

namespace Bannerlord.ModuleManager;

using System.Collections.Generic;
using System.Linq;

#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
    internal
#else
public
# endif
    static class ModuleInfoExtendedExtensions
{
    public static IEnumerable<DependentModuleMetadata> DependenciesAllDistinct(this ModuleInfoExtended module) => DependenciesAll(module).DistinctBy(x => x.Id);
    public static IEnumerable<DependentModuleMetadata> DependenciesAll(this ModuleInfoExtended module)
    {
        foreach (var metadata in module.DependentModuleMetadatas.Where(x => x is not null))
        {
            yield return metadata;
        }
        foreach (var metadata in module.DependentModules.Where(x => x is not null))
        {
            yield return new DependentModuleMetadata
            {
                Id = metadata.Id,
                LoadType = LoadType.LoadBeforeThis,
                IsOptional = metadata.IsOptional,
                Version = metadata.Version
            };
        }
        foreach (var metadata in module.ModulesToLoadAfterThis.Where(x => x is not null))
        {
            yield return new DependentModuleMetadata
            {
                Id = metadata.Id,
                LoadType = LoadType.LoadAfterThis,
                IsOptional = metadata.IsOptional,
                Version = metadata.Version
            };
        }
        foreach (var metadata in module.IncompatibleModules.Where(x => x is not null))
        {
            yield return new DependentModuleMetadata
            {
                Id = metadata.Id,
                IsIncompatible = true,
                IsOptional = metadata.IsOptional,
                Version = metadata.Version
            };
        }
    }
        
    public static IEnumerable<DependentModuleMetadata> DependenciesToLoadDistinct(this ModuleInfoExtended module) => DependenciesToLoad(module).DistinctBy(x => x.Id);
    public static IEnumerable<DependentModuleMetadata> DependenciesToLoad(this ModuleInfoExtended module)
    {
        foreach (var metadata in module.DependentModuleMetadatas.Where(x => x is not null).Where(x => !x.IsIncompatible))
        {
            yield return metadata;
        }
        foreach (var metadata in module.DependentModules.Where(x => x is not null))
        {
            yield return new DependentModuleMetadata
            {
                Id = metadata.Id,
                LoadType = LoadType.LoadBeforeThis,
                IsOptional = metadata.IsOptional,
                Version = metadata.Version
            };
        }
        foreach (var metadata in module.ModulesToLoadAfterThis.Where(x => x is not null))
        {
            yield return new DependentModuleMetadata
            {
                Id = metadata.Id,
                LoadType = LoadType.LoadAfterThis,
                IsOptional = metadata.IsOptional,
                Version = metadata.Version
            };
        }
    }
        
    public static IEnumerable<DependentModuleMetadata> DependenciesLoadBeforeThisDistinct(this ModuleInfoExtended module) => DependenciesLoadBeforeThis(module).DistinctBy(x => x.Id);
    public static IEnumerable<DependentModuleMetadata> DependenciesLoadBeforeThis(this ModuleInfoExtended module)
    {
        foreach (var metadata in module.DependentModuleMetadatas.Where(x => x is not null).Where(x => x.LoadType == LoadType.LoadBeforeThis))
        {
            yield return metadata;
        }
        foreach (var metadata in module.DependentModules.Where(x => x is not null))
        {
            yield return new DependentModuleMetadata
            {
                Id = metadata.Id,
                LoadType = LoadType.LoadBeforeThis,
                IsOptional = metadata.IsOptional,
                Version = metadata.Version
            };
        }
    }
        
    public static IEnumerable<DependentModuleMetadata> DependenciesLoadAfterThisDistinct(this ModuleInfoExtended module) => DependenciesLoadAfterThis(module).DistinctBy(x => x.Id);
    public static IEnumerable<DependentModuleMetadata> DependenciesLoadAfterThis(this ModuleInfoExtended module)
    {
        foreach (var metadata in module.DependentModuleMetadatas.Where(x => x is not null).Where(x => x.LoadType == LoadType.LoadAfterThis))
        {
            yield return metadata;
        }
        foreach (var metadata in module.ModulesToLoadAfterThis.Where(x => x is not null))
        {
            yield return new DependentModuleMetadata
            {
                Id = metadata.Id,
                LoadType = LoadType.LoadAfterThis,
                IsOptional = metadata.IsOptional,
                Version = metadata.Version
            };
        }
    }
        
    public static IEnumerable<DependentModuleMetadata> DependenciesIncompatiblesDistinct(this ModuleInfoExtended module) => DependenciesIncompatibles(module).DistinctBy(x => x.Id);
    public static IEnumerable<DependentModuleMetadata> DependenciesIncompatibles(this ModuleInfoExtended module)
    {
        foreach (var metadata in module.DependentModuleMetadatas.Where(x => x is not null).Where(x => x.IsIncompatible))
        {
            yield return metadata;
        }
        foreach (var metadata in module.IncompatibleModules.Where(x => x is not null))
        {
            yield return new DependentModuleMetadata
            {
                Id = metadata.Id,
                IsIncompatible = true,
                IsOptional = metadata.IsOptional,
                Version = metadata.Version
            };
        }
    }
}

#nullable restore
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning restore
#endif