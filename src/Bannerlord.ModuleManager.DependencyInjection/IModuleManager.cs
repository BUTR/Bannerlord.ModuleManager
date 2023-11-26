using System.Collections.Generic;

namespace Bannerlord.ModuleManager.DependencyInjection;

public interface IModuleManager
{
    IList<ModuleInfoExtended> Sort(IReadOnlyCollection<ModuleInfoExtended> source);
    IList<ModuleInfoExtended> Sort(IReadOnlyCollection<ModuleInfoExtended> source, ModuleSorterOptions options);
    
    bool AreDependenciesPresent(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleInfoExtended module);

    IList<ModuleInfoExtended> GetDependencies(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleInfoExtended module);
    IList<ModuleInfoExtended> GetDependencies(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleInfoExtended module, ModuleSorterOptions options);

    IList<ModuleIssue> ValidateModule(IReadOnlyList<ModuleInfoExtended> modules, ModuleInfoExtended targetModule, IModuleManagerModuleViewModel provider);
    IList<ModuleIssue> ValidateLoadOrder(IReadOnlyList<ModuleInfoExtended> modules, ModuleInfoExtended targetModule);

    void EnableModule(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleInfoExtended targetModule, IModuleManagerModuleViewModel provider);
    void DisableModule(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleInfoExtended targetModule, IModuleManagerModuleViewModel provider);

    ModuleInfoExtended? GetModuleInfo(string xml);

    SubModuleInfoExtended? GetSubModuleInfo(string xml);

    ApplicationVersion ParseApplicationVersion(string value);
    int CompareVersions(ApplicationVersion x, ApplicationVersion y);
    
    IList<DependentModuleMetadata> GetDependenciesAll(ModuleInfoExtended moduleInfoExtended);
    IList<DependentModuleMetadata> GetDependenciesLoadBeforeThis(ModuleInfoExtended moduleInfoExtended);
    IList<DependentModuleMetadata> GetDependenciesLoadAfterThis(ModuleInfoExtended moduleInfoExtended);
    IList<DependentModuleMetadata> GetDependenciesIncompatibles(ModuleInfoExtended moduleInfoExtended);
}