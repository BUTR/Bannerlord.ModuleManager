using DotNetJS;

using Microsoft.JSInterop;

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml;

[assembly: JSNamespace("Bannerlord.ModuleManager.WASM", "Bannerlord.ModuleManager")]

namespace Bannerlord.ModuleManager.WASM
{
    public static class Program
    {
        static Program()
        {
            JS.Runtime.ConfigureJson(options => options.Converters.Add(new JsonStringEnumConverter()));
        }

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
            ModuleUtilities.AreDependenciesPresent(source, module);


        [JSInvokable]
        public static ModuleInfoExtended[] GetDependentModulesOf(ModuleInfoExtended[] source, ModuleInfoExtended module) =>
            ModuleUtilities.GetDependencies(source, module).ToArray();

        [JSInvokable]
        public static ModuleInfoExtended[] GetDependentModulesOfWithOptions(ModuleInfoExtended[] source, ModuleInfoExtended module, ModuleSorterOptions options) =>
            ModuleUtilities.GetDependencies(source, module, options).ToArray();


        [JSInvokable]
        public static ModuleIssue[] ValidateModule(ModuleInfoExtended[] modules, ModuleInfoExtended targetModule, IJSUnmarshalledObjectReference manager) =>
            ModuleUtilities.ValidateModule(modules, targetModule, module => manager.Invoke<bool>("isSelected", module.Id)).ToArray();

        [JSInvokable]
        public static ModuleIssue[] ValidateLoadOrder(ModuleInfoExtended[] modules, ModuleInfoExtended targetModule) =>
            ModuleUtilities.ValidateLoadOrder(modules, targetModule).ToArray();


        [JSInvokable]
        public static void EnableModule(ModuleInfoExtended[] modules, ModuleInfoExtended targetModule, IJSUnmarshalledObjectReference manager) =>
            ModuleUtilities.EnableModule(modules, targetModule,
                module => manager.Invoke<bool>("getSelected", module.Id), (module, value) => manager.InvokeVoid("setSelected", module.Id, value),
                module => manager.Invoke<bool>("getDisabled", module.Id), (module, value) => manager.InvokeVoid("setDisabled", module.Id, value));

        [JSInvokable]
        public static void DisableModule(ModuleInfoExtended[] modules, ModuleInfoExtended targetModule, IJSUnmarshalledObjectReference manager) =>
            ModuleUtilities.DisableModule(modules, targetModule,
                module => manager.Invoke<bool>("getSelected", module.Id), (module, value) => manager.InvokeVoid("setSelected", module.Id, value),
                module => manager.Invoke<bool>("getDisabled", module.Id), (module, value) => manager.InvokeVoid("setDisabled", module.Id, value));


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