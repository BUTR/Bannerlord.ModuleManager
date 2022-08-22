using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Xml;

namespace Bannerlord.ModuleManager.Native
{
    public static class BLManager
    {
        private static readonly ApplicationVersionComparer _applicationVersionComparer = new();
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
        private static readonly SourceGenerationContext _customSourceGenerationContext = new(_options);


        [UnmanagedCallersOnly(EntryPoint = "sort")]
        public static IntPtr Sort(IntPtr p_source_json)
        {
            try
            {
                var sourceJson = Marshal.PtrToStringAnsi(p_source_json);
                var source = JsonSerializer.Deserialize<ModuleInfoExtended[]>(sourceJson, _customSourceGenerationContext.ModuleInfoExtendedArray);

                var sorted = ModuleSorter.Sort(source).ToArray();
                
                var sortedJson = JsonSerializer.Serialize<ModuleInfoExtended[]>(sorted, _customSourceGenerationContext.ModuleInfoExtendedArray);
                return Marshal.StringToHGlobalAnsi(sortedJson);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "sort_with_options")]
        public static IntPtr SortWithOptions(IntPtr p_source_json, IntPtr p_options_json)
        {
            try
            {
                var sourceJson = Marshal.PtrToStringAnsi(p_source_json);
                var source = JsonSerializer.Deserialize<ModuleInfoExtended[]>(sourceJson, _customSourceGenerationContext.ModuleInfoExtendedArray);

                var optionsJson = Marshal.PtrToStringAnsi(p_options_json);
                var options = JsonSerializer.Deserialize<ModuleSorterOptions>(optionsJson, _customSourceGenerationContext.ModuleSorterOptions);

                var sorted = ModuleSorter.Sort(source, options).ToArray();

                var sortedJson = JsonSerializer.Serialize<ModuleInfoExtended[]>(sorted, _customSourceGenerationContext.ModuleInfoExtendedArray);
                return Marshal.StringToHGlobalAnsi(sortedJson);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "are_all_dependencies_of_module_present")]
        public static bool AreAllDependenciesOfModulePresent(IntPtr p_source_json, IntPtr p_module_json)
        {
            try
            {
                var sourceJson = Marshal.PtrToStringAnsi(p_source_json);
                var source = JsonSerializer.Deserialize<ModuleInfoExtended[]>(sourceJson, _customSourceGenerationContext.ModuleInfoExtendedArray);

                var moduleJson = Marshal.PtrToStringAnsi(p_module_json);
                var module = JsonSerializer.Deserialize<ModuleInfoExtended>(moduleJson, _customSourceGenerationContext.ModuleInfoExtended);

                return ModuleUtilities.AreDependenciesPresent(source, module);
            }
            catch
            {
                return false;
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "get_dependent_modules_of")]
        public static IntPtr GetDependentModulesOf(IntPtr p_source_json, IntPtr p_module_json)
        {
            try
            {
                var sourceJson = Marshal.PtrToStringAnsi(p_source_json);
                var source = JsonSerializer.Deserialize<ModuleInfoExtended[]>(sourceJson, _customSourceGenerationContext.ModuleInfoExtendedArray);

                var moduleJson = Marshal.PtrToStringAnsi(p_module_json);
                var module = JsonSerializer.Deserialize<ModuleInfoExtended>(moduleJson, _customSourceGenerationContext.ModuleInfoExtended);

                var gependentModules = ModuleUtilities.GetDependencies(source, module).ToArray();

                var sortedJson = JsonSerializer.Serialize<ModuleInfoExtended[]>(gependentModules, _customSourceGenerationContext.ModuleInfoExtendedArray);
                return Marshal.StringToHGlobalAnsi(sortedJson);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "get_dependent_modules_of_with_options")]
        public static IntPtr GetDependentModulesOfWithOptions(IntPtr p_source_json, IntPtr p_module_json, IntPtr p_options_json)
        {
            try
            {
                var sourceJson = Marshal.PtrToStringAnsi(p_source_json);
                var source = JsonSerializer.Deserialize<ModuleInfoExtended[]>(sourceJson, _customSourceGenerationContext.ModuleInfoExtendedArray);

                var moduleJson = Marshal.PtrToStringAnsi(p_module_json);
                var module = JsonSerializer.Deserialize<ModuleInfoExtended>(moduleJson, _customSourceGenerationContext.ModuleInfoExtended);

                var optionsJson = Marshal.PtrToStringAnsi(p_options_json);
                var options = JsonSerializer.Deserialize<ModuleSorterOptions>(optionsJson, _customSourceGenerationContext.ModuleSorterOptions);

                var gependentModules = ModuleUtilities.GetDependencies(source, module, options).ToArray();

                var sortedJson = JsonSerializer.Serialize<ModuleInfoExtended[]>(gependentModules, _customSourceGenerationContext.ModuleInfoExtendedArray);
                return Marshal.StringToHGlobalAnsi(sortedJson);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }


        private delegate bool IsSelected(IntPtr p_uuid, IntPtr p_module_id);

        [UnmanagedCallersOnly(EntryPoint = "validate_module")]
        public static IntPtr ValidateModule(IntPtr p_uuid, IntPtr p_modules_json, IntPtr p_target_module_json, IntPtr p_is_selected)
        {
            try
            {
                var uuid = Marshal.PtrToStringAnsi(p_uuid);

                var modulesJson = Marshal.PtrToStringAnsi(p_modules_json);
                var modules = JsonSerializer.Deserialize<ModuleInfoExtended[]>(modulesJson, _customSourceGenerationContext.ModuleInfoExtendedArray);

                var targetModuleJson = Marshal.PtrToStringAnsi(p_target_module_json);
                var targetModule = JsonSerializer.Deserialize<ModuleInfoExtended>(targetModuleJson, _customSourceGenerationContext.ModuleInfoExtended);

                var isSelected = Marshal.GetDelegateForFunctionPointer<IsSelected>(p_is_selected);

                var issues = ModuleUtilities.ValidateModule(modules, targetModule,
                    module =>
                    {
                        var pUuid = Marshal.StringToHGlobalAnsi(uuid);
                        var pModuleId = Marshal.StringToHGlobalAnsi(module.Id);
                        return isSelected(pUuid, pModuleId);
                    }).ToArray();
                
                var issuesJson = JsonSerializer.Serialize<ModuleIssue[]>(issues, _customSourceGenerationContext.ModuleIssueArray);
                return Marshal.StringToHGlobalAnsi(issuesJson);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "validate_module_dependencies_declarations")]
        public static IntPtr ValidateModuleDependenciesDeclarations(IntPtr p_target_module_json)
        {
            try
            {
                var targetModuleJson = Marshal.PtrToStringAnsi(p_target_module_json);
                var targetModule = JsonSerializer.Deserialize<ModuleInfoExtended>(targetModuleJson, _customSourceGenerationContext.ModuleInfoExtended);

                var moduleIssues = ModuleUtilities.ValidateModuleDependenciesDeclarations(targetModule).ToArray();

                var moduleIssuesJson = JsonSerializer.Serialize<ModuleIssue[]>(moduleIssues, _customSourceGenerationContext.ModuleIssueArray);
                return Marshal.StringToHGlobalAnsi(moduleIssuesJson);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }


        private delegate bool GetSelected(IntPtr p_uuid, IntPtr p_module_id);
        private delegate void SetSelected(IntPtr p_uuid, IntPtr p_module_id, bool value);
        private delegate bool GetDisabled(IntPtr p_uuid, IntPtr p_module_id);
        private delegate void SetDisabled(IntPtr p_uuid, IntPtr p_module_id, bool value);

        [UnmanagedCallersOnly(EntryPoint = "enable_module")]
        public static IntPtr EnableModule(IntPtr p_uuid, IntPtr p_module_json, IntPtr p_target_module_json, IntPtr p_get_selected, IntPtr p_set_selected, IntPtr p_get_disabled, IntPtr p_set_disabled)
        {
            try
            {
                var uuid = Marshal.PtrToStringAnsi(p_uuid);

                var modulesJson = Marshal.PtrToStringAnsi(p_module_json);
                var modules = JsonSerializer.Deserialize<ModuleInfoExtended[]>(modulesJson, _customSourceGenerationContext.ModuleInfoExtendedArray);

                var targetModuleJson = Marshal.PtrToStringAnsi(p_target_module_json);
                var targetModule = JsonSerializer.Deserialize<ModuleInfoExtended>(targetModuleJson, _customSourceGenerationContext.ModuleInfoExtended);

                var getSelected = Marshal.GetDelegateForFunctionPointer<GetSelected>(p_get_selected);
                var setSelected = Marshal.GetDelegateForFunctionPointer<SetSelected>(p_set_selected);
                var getDisabled = Marshal.GetDelegateForFunctionPointer<GetDisabled>(p_get_disabled);
                var setDisabled = Marshal.GetDelegateForFunctionPointer<SetDisabled>(p_set_disabled);

                var issues = ModuleUtilities.EnableModule(modules, targetModule,
                    module =>
                    {
                        var pUuid = Marshal.StringToHGlobalAnsi(uuid);
                        var pModuleId = Marshal.StringToHGlobalAnsi(module.Id);
                        return getSelected(pUuid, pModuleId);
                    },
                    (module, value) =>
                    {
                        var pUuid = Marshal.StringToHGlobalAnsi(uuid);
                        var pModuleId = Marshal.StringToHGlobalAnsi(module.Id);
                        setSelected(pUuid, pModuleId, value);
                    },
                    module =>
                    {
                        var pUuid = Marshal.StringToHGlobalAnsi(uuid);
                        var pModuleId = Marshal.StringToHGlobalAnsi(module.Id);
                        return getDisabled(pUuid, pModuleId);
                    },
                    (module, value) =>
                    {
                        var pUuid = Marshal.StringToHGlobalAnsi(uuid);
                        var pModuleId = Marshal.StringToHGlobalAnsi(module.Id);
                        setDisabled(pUuid, pModuleId, value);
                    }).ToArray();
                
                var issuesJson = JsonSerializer.Serialize<ModuleIssue[]>(issues, _customSourceGenerationContext.ModuleIssueArray);
                return Marshal.StringToHGlobalAnsi(issuesJson);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "disable_module")]
        public static IntPtr DisableModule(IntPtr p_uuid, IntPtr p_module_json, IntPtr p_target_module_json, IntPtr p_get_selected, IntPtr p_set_selected, IntPtr p_get_disabled, IntPtr p_set_disabled)
        {
            try
            {
                var uuid = Marshal.PtrToStringAnsi(p_uuid);

                var modulesJson = Marshal.PtrToStringAnsi(p_module_json);
                var modules = JsonSerializer.Deserialize<ModuleInfoExtended[]>(modulesJson, _customSourceGenerationContext.ModuleInfoExtendedArray);

                var targetModuleJson = Marshal.PtrToStringAnsi(p_target_module_json);
                var targetModule = JsonSerializer.Deserialize<ModuleInfoExtended>(targetModuleJson, _customSourceGenerationContext.ModuleInfoExtended);

                var getSelected = Marshal.GetDelegateForFunctionPointer<GetSelected>(p_get_selected);
                var setSelected = Marshal.GetDelegateForFunctionPointer<SetSelected>(p_set_selected);
                var getDisabled = Marshal.GetDelegateForFunctionPointer<GetDisabled>(p_get_disabled);
                var setDisabled = Marshal.GetDelegateForFunctionPointer<SetDisabled>(p_set_disabled);

                var issues = ModuleUtilities.DisableModule(modules, targetModule,
                    module =>
                    {
                        var pUuid = Marshal.StringToHGlobalAnsi(uuid);
                        var pModuleId = Marshal.StringToHGlobalAnsi(module.Id);
                        return getSelected(pUuid, pModuleId);
                    },
                    (module, value) =>
                    {
                        var pUuid = Marshal.StringToHGlobalAnsi(uuid);
                        var pModuleId = Marshal.StringToHGlobalAnsi(module.Id);
                        setSelected(pUuid, pModuleId, value);
                    },
                    module =>
                    {
                        var pUuid = Marshal.StringToHGlobalAnsi(uuid);
                        var pModuleId = Marshal.StringToHGlobalAnsi(module.Id);
                        return getDisabled(pUuid, pModuleId);
                    },
                    (module, value) =>
                    {
                        var pUuid = Marshal.StringToHGlobalAnsi(uuid);
                        var pModuleId = Marshal.StringToHGlobalAnsi(module.Id);
                        setDisabled(pUuid, pModuleId, value);
                    }).ToArray();
                
                var issuesJson = JsonSerializer.Serialize<ModuleIssue[]>(issues, _customSourceGenerationContext.ModuleIssueArray);
                return Marshal.StringToHGlobalAnsi(issuesJson);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "get_module_info")]
        public static IntPtr GetModuleInfo(IntPtr p_xml_content)
        {
            try
            {
                var xmlContent = Marshal.PtrToStringAnsi(p_xml_content);
                var xml = Utils.UnescapeString(xmlContent);

                var doc = new XmlDocument();
                doc.LoadXml(xml);

                var module = ModuleInfoExtended.FromXml(doc);

                var moduleJson = JsonSerializer.Serialize<ModuleInfoExtended>(module, _customSourceGenerationContext.ModuleInfoExtended);
                return Marshal.StringToHGlobalAnsi(moduleJson);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "get_sub_module_info")]
        public static IntPtr GetSubModuleInfo(IntPtr p_xml_content)
        {
            try
            {
                var xmlContent = Marshal.PtrToStringAnsi(p_xml_content);
                var xml = Utils.UnescapeString(xmlContent);

                var doc = new XmlDocument();
                doc.LoadXml(xml);

                var module = SubModuleInfoExtended.FromXml(doc);

                var moduleJson = JsonSerializer.Serialize<SubModuleInfoExtended>(module, _customSourceGenerationContext.SubModuleInfoExtended);
                return Marshal.StringToHGlobalAnsi(moduleJson);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "compare_versions")]
        public static int CompareVersions(IntPtr p_x_json, IntPtr p_y_json)
        {
            try
            {
                var xJson = Marshal.PtrToStringAnsi(p_x_json);
                var x = JsonSerializer.Deserialize<ApplicationVersion>(xJson, _customSourceGenerationContext.ApplicationVersion);

                var yJson = Marshal.PtrToStringAnsi(p_y_json);
                var y = JsonSerializer.Deserialize<ApplicationVersion>(yJson, _customSourceGenerationContext.ApplicationVersion);

                return _applicationVersionComparer.Compare(x, y);
            }
            catch
            {
                return -2;
            }
        }
    }
}