using DotNetJS;

using Microsoft.JSInterop;

using System.Collections.Generic;
using System.Linq;
using System.Xml;

[assembly: JSNamespace("Bannerlord.ModuleManager.WASM", "Bannerlord.ModuleManager")]

namespace Bannerlord.ModuleManager.WASM
{
    public static class Program
    {
        private static readonly ApplicationVersionComparer _applicationVersionComparer = new();

        public static void Main() { }

        [JSInvokable]
        public static IList<ModuleInfoExtended> Sort(ModuleInfoExtended[] source) =>
            ModuleSorter.Sort(source);

        [JSInvokable]
        public static IList<ModuleInfoExtended> SortWithOptions(ModuleInfoExtended[] source, ModuleSorterOptions options) =>
            ModuleSorter.Sort(source, options);

        [JSInvokable]
        public static bool AreAllDependenciesOfModulePresent(ModuleInfoExtended[] source, ModuleInfoExtended module) =>
            ModuleSorter.AreAllDependenciesOfModulePresent(source, module);

        [JSInvokable]
        public static ModuleInfoExtended[] GetDependentModulesOf(ModuleInfoExtended[] source, ModuleInfoExtended module) =>
            ModuleSorter.GetDependentModulesOf(source, module).ToArray();

        [JSInvokable]
        public static ModuleInfoExtended[] GetDependentModulesOfWithOptions(ModuleInfoExtended[] source, ModuleInfoExtended module, ModuleSorterOptions options) =>
            ModuleSorter.GetDependentModulesOf(source, module, options).ToArray();

        //[JSInvokable]
        //public static ModuleInfoExtended[] GetDependentModulesOf(ModuleInfoExtended[] source, ModuleInfoExtended[] visited, ModuleInfoExtended module, bool skipExternalDependencies = false) =>
        //    ModuleSorter.GetDependentModulesOf(source, module, visited.ToHashSet(), skipExternalDependencies).ToArray();

        [JSInvokable]
        public static ModuleInfoExtended? GetModuleInfo(string xmlContent)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xmlContent);
                return ModuleInfoExtended.FromXml(doc);
            }
            catch { return null; }
        }

        [JSInvokable]
        public static SubModuleInfoExtended? GetSubModuleInfo(string xmlContent)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xmlContent);
                return SubModuleInfoExtended.FromXml(doc);
            }
            catch { return null; }
        }

        [JSInvokable]
        public static int CompareVersions(ApplicationVersion x, ApplicationVersion y) => _applicationVersionComparer.Compare(x, y);
    }
}