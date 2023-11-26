using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Bannerlord.ModuleManager.DependencyInjection;

public class ModuleManager : IModuleManager
{
    public IList<ModuleInfoExtended> Sort(IReadOnlyCollection<ModuleInfoExtended> source) =>
        ModuleSorter.Sort(source);
    public IList<ModuleInfoExtended> Sort(IReadOnlyCollection<ModuleInfoExtended> source, ModuleSorterOptions options) =>
        ModuleSorter.Sort(source, options);

    public bool AreDependenciesPresent(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleInfoExtended module) =>
        ModuleUtilities.AreDependenciesPresent(modules, module);

    public IList<ModuleInfoExtended> GetDependencies(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleInfoExtended module) =>
        ModuleUtilities.GetDependencies(modules, module).ToList();
    public IList<ModuleInfoExtended> GetDependencies(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleInfoExtended module, ModuleSorterOptions options) =>
        ModuleUtilities.GetDependencies(modules, module, options).ToList();

    
    public IList<ModuleIssue> ValidateModule(IReadOnlyList<ModuleInfoExtended> modules, ModuleInfoExtended targetModule, IModuleManagerModuleViewModel provider) =>
        ModuleUtilities.ValidateModule(modules, targetModule, x => provider.GetSelected(x.Id)).ToList();

    public IList<ModuleIssue> ValidateLoadOrder(IReadOnlyList<ModuleInfoExtended> modules, ModuleInfoExtended targetModule) =>
        ModuleUtilities.ValidateLoadOrder(modules, targetModule).ToList();

    public void EnableModule(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleInfoExtended targetModule, IModuleManagerModuleViewModel provider) =>
        ModuleUtilities.EnableModule(modules, targetModule, x => provider.GetSelected(x.Id), (x, y) => provider.SetSelected(x.Id, y), x => provider.GetDisabled(x.Id), (x, y) => provider.SetDisabled(x.Id, y));

    public void DisableModule(IReadOnlyCollection<ModuleInfoExtended> modules, ModuleInfoExtended targetModule, IModuleManagerModuleViewModel provider) =>
        ModuleUtilities.DisableModule(modules, targetModule, x => provider.GetSelected(x.Id), (x, y) => provider.SetSelected(x.Id, y), x => provider.GetDisabled(x.Id), (x, y) => provider.SetDisabled(x.Id, y));
 
    public ModuleInfoExtended? GetModuleInfo(string xml)
    {
        try
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return ModuleInfoExtended.FromXml(doc);
        }
        catch { return null; }
    }

    public SubModuleInfoExtended? GetSubModuleInfo(string xml)
    {
        try
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return SubModuleInfoExtended.FromXml(doc);
        }
        catch { return null; }
    }

    public ApplicationVersion ParseApplicationVersion(string value) =>
        ApplicationVersion.TryParse(value, out var x) ? x : ApplicationVersion.Empty;

    public int CompareVersions(ApplicationVersion x, ApplicationVersion y) =>
        new ApplicationVersionComparer().Compare(x, y);

    public IList<DependentModuleMetadata> GetDependenciesAll(ModuleInfoExtended moduleInfoExtended) =>
        moduleInfoExtended.DependenciesAllDistinct().ToArray();

    public IList<DependentModuleMetadata> GetDependenciesLoadBeforeThis(ModuleInfoExtended moduleInfoExtended) =>
        moduleInfoExtended.DependenciesLoadBeforeThisDistinct().ToArray();

    public IList<DependentModuleMetadata> GetDependenciesLoadAfterThis(ModuleInfoExtended moduleInfoExtended) =>
        moduleInfoExtended.DependenciesLoadAfterThisDistinct().ToArray();

    public IList<DependentModuleMetadata> GetDependenciesIncompatibles(ModuleInfoExtended moduleInfoExtended) =>
        moduleInfoExtended.DependenciesIncompatiblesDistinct().ToArray();
}