using Microsoft.JSInterop;

using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Bannerlord.ModuleManager.WASM
{
    public static class Program
    {
        public static void Main() { }

        [JSInvokable]
        public static IList<ModuleInfoExtended> Sort(ModuleInfoExtended[] source) =>
            ModuleSorter.Sort(source);

        [JSInvokable]
        public static bool AreAllDependenciesOfModulePresent(ModuleInfoExtended[] source, ModuleInfoExtended module) =>
            ModuleSorter.AreAllDependenciesOfModulePresent(source, module);

        [JSInvokable]
        public static ModuleInfoExtended[] GetDependentModulesOf(ModuleInfoExtended[] source, ModuleInfoExtended module, bool skipExternalDependencies = false) =>
            ModuleSorter.GetDependentModulesOf(source, module, skipExternalDependencies).ToArray();

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
    }
}