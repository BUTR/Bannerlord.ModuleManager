using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Xml;

namespace Bannerlord.ModuleManager.Native
{
    public static class Program
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
        public static IntPtr Sort(IntPtr pSourceJson)
        {
            try
            {
                var sourceJson = Marshal.PtrToStringAnsi(pSourceJson);
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

        [UnmanagedCallersOnly(EntryPoint = "sortWithOptions")]
        public static IntPtr SortWithOptions(IntPtr pSourceJson, IntPtr pOptionsJson)
        {
            try
            {
                var sourceJson = Marshal.PtrToStringAnsi(pSourceJson);
                var source = JsonSerializer.Deserialize<ModuleInfoExtended[]>(sourceJson, _customSourceGenerationContext.ModuleInfoExtendedArray);

                var optionsJson = Marshal.PtrToStringAnsi(pOptionsJson);
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


        [UnmanagedCallersOnly(EntryPoint = "areAllDependenciesOfModulePresent")]
        public static bool AreAllDependenciesOfModulePresent(IntPtr pSourceJson, IntPtr pModuleJson)
        {
            try
            {
                var sourceJson = Marshal.PtrToStringAnsi(pSourceJson);
                var source = JsonSerializer.Deserialize<ModuleInfoExtended[]>(sourceJson, _customSourceGenerationContext.ModuleInfoExtendedArray);

                var moduleJson = Marshal.PtrToStringAnsi(pModuleJson);
                var module = JsonSerializer.Deserialize<ModuleInfoExtended>(moduleJson, _customSourceGenerationContext.ModuleInfoExtended);

                return ModuleUtilities.AreDependenciesPresent(source, module);
            }
            catch
            {
                return false;
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "getDependentModulesOf")]
        public static IntPtr GetDependentModulesOf(IntPtr pSourceJson, IntPtr pModuleJson)
        {
            try
            {
                var sourceJson = Marshal.PtrToStringAnsi(pSourceJson);
                var source = JsonSerializer.Deserialize<ModuleInfoExtended[]>(sourceJson, _customSourceGenerationContext.ModuleInfoExtendedArray);

                var moduleJson = Marshal.PtrToStringAnsi(pModuleJson);
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

        [UnmanagedCallersOnly(EntryPoint = "getDependentModulesOfWithOptions")]
        public static IntPtr GetDependentModulesOfWithOptions(IntPtr pSourceJson, IntPtr pModuleJson, IntPtr pOptionsJson)
        {
            try
            {
                var sourceJson = Marshal.PtrToStringAnsi(pSourceJson);
                var source = JsonSerializer.Deserialize<ModuleInfoExtended[]>(sourceJson, _customSourceGenerationContext.ModuleInfoExtendedArray);

                var moduleJson = Marshal.PtrToStringAnsi(pModuleJson);
                var module = JsonSerializer.Deserialize<ModuleInfoExtended>(moduleJson, _customSourceGenerationContext.ModuleInfoExtended);

                var optionsJson = Marshal.PtrToStringAnsi(pOptionsJson);
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


        private delegate bool IsSelected(IntPtr pModuleId);

        [UnmanagedCallersOnly(EntryPoint = "validateModule")]
        public static IntPtr ValidateModule(IntPtr pModulesJson, IntPtr pTargetModuleJson, IntPtr pIsSelected)
        {
            try
            {
                var modulesJson = Marshal.PtrToStringAnsi(pModulesJson);
                var modules = JsonSerializer.Deserialize<ModuleInfoExtended[]>(modulesJson, _customSourceGenerationContext.ModuleInfoExtendedArray);

                var targetModuleJson = Marshal.PtrToStringAnsi(pTargetModuleJson);
                var targetModule = JsonSerializer.Deserialize<ModuleInfoExtended>(targetModuleJson, _customSourceGenerationContext.ModuleInfoExtended);

                var isSelected = Marshal.GetDelegateForFunctionPointer<IsSelected>(pIsSelected);

                var issues = ModuleUtilities.ValidateModule(modules, targetModule,
                    module =>
                    {
                        var pModuleId = Marshal.StringToHGlobalAnsi(module.Id);
                        return isSelected(pModuleId);
                    }).ToArray();
                
                var issuesJson = JsonSerializer.Serialize<ModuleIssue[]>(issues, _customSourceGenerationContext.ModuleIssueArray);
                return Marshal.StringToHGlobalAnsi(issuesJson);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "validateModuleDependenciesDeclarations")]
        public static IntPtr ValidateModuleDependenciesDeclarations(IntPtr pTargetModuleJson)
        {
            try
            {
                var targetModuleJson = Marshal.PtrToStringAnsi(pTargetModuleJson);
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


        private delegate bool GetSelected(IntPtr pModuleId);
        private delegate void SetSelected(IntPtr pModuleId, bool value);
        private delegate bool GetDisabled(IntPtr pModuleId);
        private delegate void SetDisabled(IntPtr pModuleId, bool value);

        [UnmanagedCallersOnly(EntryPoint = "enableModule")]
        public static IntPtr EnableModule(IntPtr pModulesJson, IntPtr pTargetModuleJson, IntPtr pGetSelected, IntPtr pSetSelected, IntPtr pGetDisabled, IntPtr pSetDisabled)
        {
            try
            {
                var modulesJson = Marshal.PtrToStringAnsi(pModulesJson);
                var modules = JsonSerializer.Deserialize<ModuleInfoExtended[]>(modulesJson, _customSourceGenerationContext.ModuleInfoExtendedArray);

                var targetModuleJson = Marshal.PtrToStringAnsi(pTargetModuleJson);
                var targetModule = JsonSerializer.Deserialize<ModuleInfoExtended>(targetModuleJson, _customSourceGenerationContext.ModuleInfoExtended);

                var getSelected = Marshal.GetDelegateForFunctionPointer<GetSelected>(pGetSelected);
                var setSelected = Marshal.GetDelegateForFunctionPointer<SetSelected>(pSetSelected);
                var getDisabled = Marshal.GetDelegateForFunctionPointer<GetDisabled>(pGetDisabled);
                var setDisabled = Marshal.GetDelegateForFunctionPointer<SetDisabled>(pSetDisabled);

                var issues = ModuleUtilities.EnableModule(modules, targetModule,
                    module =>
                    {
                        var pModuleId = Marshal.StringToHGlobalAnsi(module.Id);
                        return getSelected(pModuleId);
                    },
                    (module, value) =>
                    {
                        var pModuleId = Marshal.StringToHGlobalAnsi(module.Id);
                        setSelected(pModuleId, value);
                    },
                    module =>
                    {
                        var pModuleId = Marshal.StringToHGlobalAnsi(module.Id);
                        return getDisabled(pModuleId);
                    },
                    (module, value) =>
                    {
                        var pModuleId = Marshal.StringToHGlobalAnsi(module.Id);
                        setDisabled(pModuleId, value);
                    }).ToArray();
                
                var issuesJson = JsonSerializer.Serialize<ModuleIssue[]>(issues, _customSourceGenerationContext.ModuleIssueArray);
                return Marshal.StringToHGlobalAnsi(issuesJson);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "disableModule")]
        public static IntPtr DisableModule(IntPtr pModulesJson, IntPtr pTargetModuleJson, IntPtr pGetSelected, IntPtr pSetSelected, IntPtr pGetDisabled, IntPtr pSetDisabled)
        {
            try
            {
                var modulesJson = Marshal.PtrToStringAnsi(pModulesJson);
                var modules = JsonSerializer.Deserialize<ModuleInfoExtended[]>(modulesJson, _customSourceGenerationContext.ModuleInfoExtendedArray);

                var targetModuleJson = Marshal.PtrToStringAnsi(pTargetModuleJson);
                var targetModule = JsonSerializer.Deserialize<ModuleInfoExtended>(targetModuleJson, _customSourceGenerationContext.ModuleInfoExtended);

                var getSelected = Marshal.GetDelegateForFunctionPointer<GetSelected>(pGetSelected);
                var setSelected = Marshal.GetDelegateForFunctionPointer<SetSelected>(pSetSelected);
                var getDisabled = Marshal.GetDelegateForFunctionPointer<GetDisabled>(pGetDisabled);
                var setDisabled = Marshal.GetDelegateForFunctionPointer<SetDisabled>(pSetDisabled);

                var issues = ModuleUtilities.DisableModule(modules, targetModule,
                    module =>
                    {
                        var pModuleId = Marshal.StringToHGlobalAnsi(module.Id);
                        return getSelected(pModuleId);
                    },
                    (module, value) =>
                    {
                        var pModuleId = Marshal.StringToHGlobalAnsi(module.Id);
                        setSelected(pModuleId, value);
                    },
                    module =>
                    {
                        var pModuleId = Marshal.StringToHGlobalAnsi(module.Id);
                        return getDisabled(pModuleId);
                    },
                    (module, value) =>
                    {
                        var pModuleId = Marshal.StringToHGlobalAnsi(module.Id);
                        setDisabled(pModuleId, value);
                    }).ToArray();
                
                var issuesJson = JsonSerializer.Serialize<ModuleIssue[]>(issues, _customSourceGenerationContext.ModuleIssueArray);
                return Marshal.StringToHGlobalAnsi(issuesJson);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "getModuleInfo")]
        public static IntPtr GetModuleInfo(IntPtr pXmlContent)
        {
            try
            {
                var xmlContent = Marshal.PtrToStringAnsi(pXmlContent);

                var doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                var module = ModuleInfoExtended.FromXml(doc);

                var moduleJson = JsonSerializer.Serialize<ModuleInfoExtended>(module, _customSourceGenerationContext.ModuleInfoExtended);
                return Marshal.StringToHGlobalAnsi(moduleJson);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "getSubModuleInfo")]
        public static IntPtr GetSubModuleInfo(IntPtr pXmlContent)
        {
            try
            {
                var xmlContent = Marshal.PtrToStringAnsi(pXmlContent);

                var doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                var module = SubModuleInfoExtended.FromXml(doc);

                var moduleJson = JsonSerializer.Serialize<SubModuleInfoExtended>(module, _customSourceGenerationContext.SubModuleInfoExtended);
                return Marshal.StringToHGlobalAnsi(moduleJson);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "compareVersions")]
        public static int CompareVersions(IntPtr pXJson, IntPtr pYJson)
        {
            try
            {
                var xJson = Marshal.PtrToStringAnsi(pXJson);
                var x = JsonSerializer.Deserialize<ApplicationVersion>(xJson, _customSourceGenerationContext.ApplicationVersion);

                var yJson = Marshal.PtrToStringAnsi(pYJson);
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