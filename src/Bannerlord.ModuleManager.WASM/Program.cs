using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.Unicode;
using System.Xml;

namespace Bannerlord.ModuleManager.WASM
{
    public static partial class Program
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            IncludeFields = false,
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin)
        };
        internal static readonly SourceGenerationContext CustomSourceGenerationContext = new(_options);
        public static void Main() { }

        private static string ToJson<T>(T value, JsonTypeInfo<T> typeInfo) => JsonSerializer.Serialize(value, typeInfo);
        private static T FromJson<T>(string value, JsonTypeInfo<T> typeInfo) => JsonSerializer.Deserialize(value, typeInfo);
        private static ModuleInfoExtended AsModuleInfoExtended(string value) => FromJson(value, CustomSourceGenerationContext.ModuleInfoExtended);
        private static ModuleInfoExtended[] AsModuleInfoExtendedArray(string value) => FromJson(value, CustomSourceGenerationContext.ModuleInfoExtendedArray);
        private static ModuleSorterOptions AsModuleSorterOptions(string value) => FromJson(value, CustomSourceGenerationContext.ModuleSorterOptions);
        private static ApplicationVersion AsApplicationVersion(string value) => FromJson(value, CustomSourceGenerationContext.ApplicationVersion);

        [JSExport]
        public static string Sort([JSMarshalAs<JSType.String>] string source)
        {
            return ToJson(ModuleSorter.Sort(AsModuleInfoExtendedArray(source)).ToArray(), CustomSourceGenerationContext.ModuleInfoExtendedArray);
        }


        [JSExport]
        [return: JSMarshalAs<JSType.String>]
        public static string SortWithOptions([JSMarshalAs<JSType.String>] string source, [JSMarshalAs<JSType.String>] string options) =>
            ToJson(ModuleSorter.Sort(AsModuleInfoExtendedArray(source), AsModuleSorterOptions(options)).ToArray(), CustomSourceGenerationContext.ModuleInfoExtendedArray);


        [JSExport]
        [return: JSMarshalAs<JSType.Boolean>]
        public static bool AreAllDependenciesOfModulePresent([JSMarshalAs<JSType.String>] string source, [JSMarshalAs<JSType.String>] string module) =>
            ModuleUtilities.AreDependenciesPresent(AsModuleInfoExtendedArray(source), AsModuleInfoExtended(module));


        [JSExport]
        [return: JSMarshalAs<JSType.String>]
        public static string GetDependentModulesOf([JSMarshalAs<JSType.String>] string source, [JSMarshalAs<JSType.String>] string module) =>
            ToJson(ModuleUtilities.GetDependencies(AsModuleInfoExtendedArray(source), AsModuleInfoExtended(module)).ToArray(), CustomSourceGenerationContext.ModuleInfoExtendedArray);

        [JSExport]
        [return: JSMarshalAs<JSType.String>]
        public static string GetDependentModulesOfWithOptions([JSMarshalAs<JSType.String>] string source, [JSMarshalAs<JSType.String>] string module, [JSMarshalAs<JSType.String>] string options) =>
            ToJson(ModuleUtilities.GetDependencies(AsModuleInfoExtendedArray(source), AsModuleInfoExtended(module), AsModuleSorterOptions(options)).ToArray(), CustomSourceGenerationContext.ModuleInfoExtendedArray);


        [JSExport]
        [return: JSMarshalAs<JSType.String>]
        public static string ValidateModule([JSMarshalAs<JSType.String>] string modules, [JSMarshalAs<JSType.String>] string targetModule, JSObject manager)
        {
            return ToJson(Array.Empty<ModuleIssue>(), CustomSourceGenerationContext.ModuleIssueArray);
            //return Array.Empty<ModuleIssue>();
            //return ModuleUtilities
            //    .ValidateModule(AsModuleInfoExtendedArray(modules), targetModule, module => manager.Invoke<bool>("isSelected", module.Id))
            //    .ToArray();
        }

        [JSExport]
        [return: JSMarshalAs<JSType.String>]
        public static string ValidateLoadOrder([JSMarshalAs<JSType.String>] string modules, [JSMarshalAs<JSType.String>] string targetModule) =>
            ToJson(ModuleUtilities.ValidateLoadOrder(AsModuleInfoExtendedArray(modules), AsModuleInfoExtended(targetModule)).ToArray(), CustomSourceGenerationContext.ModuleIssueArray);


        [JSExport]
        public static void EnableModule([JSMarshalAs<JSType.String>] string modules, [JSMarshalAs<JSType.String>] string targetModule, JSObject manager)
        {
            //ModuleUtilities.EnableModule(AsModuleInfoExtendedArray(modules), targetModule,
            //    module => manager.Invoke<bool>("getSelected", module.Id), (module, value) => manager.InvokeVoid("setSelected", module.Id, value),
            //    module => manager.Invoke<bool>("getDisabled", module.Id), (module, value) => manager.InvokeVoid("setDisabled", module.Id, value));
        }

        [JSExport]
        public static void DisableModule([JSMarshalAs<JSType.String>] string modules, [JSMarshalAs<JSType.String>] string targetModule, JSObject manager)
        {
            //ModuleUtilities.DisableModule(AsModuleInfoExtendedArray(modules), targetModule,
            //    module => manager.Invoke<bool>("getSelected", module.Id), (module, value) => manager.InvokeVoid("setSelected", module.Id, value),
            //    module => manager.Invoke<bool>("getDisabled", module.Id), (module, value) => manager.InvokeVoid("setDisabled", module.Id, value));
        }


        [JSExport]
        [return: JSMarshalAs<JSType.String>]
        public static string GetModuleInfo([JSMarshalAs<JSType.String>] string xmlContent)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xmlContent);
                return ToJson(ModuleInfoExtended.FromXml(doc), CustomSourceGenerationContext.ModuleInfoExtended);
            }
            catch { return null; }
        }

        [JSExport]
        [return: JSMarshalAs<JSType.String>]
        public static string GetSubModuleInfo([JSMarshalAs<JSType.String>] string xmlContent)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xmlContent);
                return ToJson(SubModuleInfoExtended.FromXml(doc), CustomSourceGenerationContext.SubModuleInfoExtended);
            }
            catch { return null; }
        }

        [JSExport]
        [return: JSMarshalAs<JSType.Number>]
        public static int CompareVersions([JSMarshalAs<JSType.String>] string x, [JSMarshalAs<JSType.String>] string y) =>
            ApplicationVersionComparer.CompareStandard(AsApplicationVersion(x), AsApplicationVersion(y));
    }
}