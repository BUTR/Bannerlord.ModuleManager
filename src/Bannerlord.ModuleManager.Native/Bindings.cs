using BUTR.NativeAOT.Shared;

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
    public static unsafe class Bindings
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate return_value_bool* N_IsSelected(void* p_owner, param_string* p_module_id);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate return_value_bool* N_GetSelected(void* p_owner, param_string* p_module_id);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate return_value_void* N_SetSelected(void* p_owner, param_string* p_module_id, [MarshalAs(UnmanagedType.U1)] bool value);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate return_value_bool* N_GetDisabled(void* p_owner, param_string* p_module_id);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate return_value_void* N_SetDisabled(void* p_owner, param_string* p_module_id, [MarshalAs(UnmanagedType.U1)] bool value);

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
        public static return_value_json* Sort(param_json* p_source)
        {
            try
            {
                var source = Utils.DeserializeJson<ModuleInfoExtended[]>(p_source, _customSourceGenerationContext.ModuleInfoExtendedArray);

                var result = ModuleSorter.Sort(source).ToArray();

                return return_value_json.AsValue<ModuleInfoExtended[]>(result, _customSourceGenerationContext.ModuleInfoExtendedArray);
            }
            catch (Exception e)
            {
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "sort_with_options")]
        public static return_value_json* SortWithOptions(param_json* p_source, param_json* p_options)
        {
            try
            {
                var source = Utils.DeserializeJson<ModuleInfoExtended[]>(p_source, _customSourceGenerationContext.ModuleInfoExtendedArray);
                var options = Utils.DeserializeJson<ModuleSorterOptions>(p_options, _customSourceGenerationContext.ModuleSorterOptions);

                var result = ModuleSorter.Sort(source, options).ToArray();

                return return_value_json.AsValue<ModuleInfoExtended[]>(result, _customSourceGenerationContext.ModuleInfoExtendedArray);
            }
            catch (Exception e)
            {
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "are_all_dependencies_of_module_present")]
        public static return_value_bool* AreAllDependenciesOfModulePresent(param_json* p_source, param_json* p_module)
        {
            try
            {
                var source = Utils.DeserializeJson<ModuleInfoExtended[]>(p_source, _customSourceGenerationContext.ModuleInfoExtendedArray);
                var module = Utils.DeserializeJson<ModuleInfoExtended>(p_module, _customSourceGenerationContext.ModuleInfoExtended);

                var result = ModuleUtilities.AreDependenciesPresent(source, module);

                return return_value_bool.AsValue(result);
            }
            catch (Exception e)
            {
                return return_value_bool.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "get_dependent_modules_of")]
        public static return_value_json* GetDependentModulesOf(param_json* p_source, param_json* p_module)
        {
            try
            {
                var source = Utils.DeserializeJson<ModuleInfoExtended[]>(p_source, _customSourceGenerationContext.ModuleInfoExtendedArray);
                var module = Utils.DeserializeJson<ModuleInfoExtended>(p_module, _customSourceGenerationContext.ModuleInfoExtended);

                var result = ModuleUtilities.GetDependencies(source, module).ToArray();

                return return_value_json.AsValue<ModuleInfoExtended[]>(result, _customSourceGenerationContext.ModuleInfoExtendedArray);
            }
            catch (Exception e)
            {
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "get_dependent_modules_of_with_options")]
        public static return_value_json* GetDependentModulesOfWithOptions(param_json* p_source, param_json* p_module, param_json* p_options)
        {
            try
            {
                var source = Utils.DeserializeJson<ModuleInfoExtended[]>(p_source, _customSourceGenerationContext.ModuleInfoExtendedArray);
                var module = Utils.DeserializeJson<ModuleInfoExtended>(p_module, _customSourceGenerationContext.ModuleInfoExtended);
                var options = Utils.DeserializeJson<ModuleSorterOptions>(p_options, _customSourceGenerationContext.ModuleSorterOptions);

                var result = ModuleUtilities.GetDependencies(source, module, options).ToArray();

                return return_value_json.AsValue<ModuleInfoExtended[]>(result, _customSourceGenerationContext.ModuleInfoExtendedArray);
            }
            catch (Exception e)
            {
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "validate_module")]
        public static return_value_json* ValidateModule(
            void* p_owner,
            param_json* p_modules,
            param_json* p_target_module,
            delegate* unmanaged[Cdecl]<return_value_bool*, void*, param_string*> p_is_selected)
        {
            try
            {
                var modules = Utils.DeserializeJson<ModuleInfoExtended[]>(p_modules, _customSourceGenerationContext.ModuleInfoExtendedArray);
                var targetModule = Utils.DeserializeJson<ModuleInfoExtended>(p_target_module, _customSourceGenerationContext.ModuleInfoExtended);

                var isSelected = Marshal.GetDelegateForFunctionPointer<N_IsSelected>(new IntPtr(p_is_selected));

                var result = ModuleUtilities.ValidateModule(modules, targetModule, module =>
                {
                    fixed (char* pModuleId = module.Id)
                    {
                        using var result = SafeStructMallocHandle.Create(isSelected(p_owner, (param_string*) pModuleId));
                        return result.ValueAsBool();
                    }
                }).ToArray();

                return return_value_json.AsValue<ModuleIssue[]>(result, _customSourceGenerationContext.ModuleIssueArray);
            }
            catch (Exception e)
            {
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "validate_load_order")]
        public static return_value_json* ValidateLoadOrder(param_json* p_modules, param_json* p_target_module)
        {
            try
            {
                var modules = Utils.DeserializeJson<ModuleInfoExtended[]>(p_modules, _customSourceGenerationContext.ModuleInfoExtendedArray);
                var targetModule = Utils.DeserializeJson<ModuleInfoExtended>(p_target_module, _customSourceGenerationContext.ModuleInfoExtended);

                var result = ModuleUtilities.ValidateLoadOrder(modules, targetModule).ToArray();

                return return_value_json.AsValue<ModuleIssue[]>(result, _customSourceGenerationContext.ModuleIssueArray);
            }
            catch (Exception e)
            {
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "enable_module")]
        public static return_value_void* EnableModule(
            void* p_owner,
            param_json* p_module,
            param_json* p_target_module,
            delegate* unmanaged[Cdecl]<return_value_bool*, void*, param_string*> p_get_selected,
            delegate* unmanaged[Cdecl]<return_value_void*, void*, param_string*, bool> p_set_selected,
            delegate* unmanaged[Cdecl]<return_value_bool*, void*, param_string*> p_get_disabled,
            delegate* unmanaged[Cdecl]<return_value_void*, void*, param_string*, bool> p_set_disabled)
        {
            try
            {
                var modules = Utils.DeserializeJson<ModuleInfoExtended[]>(p_module, _customSourceGenerationContext.ModuleInfoExtendedArray);
                var targetModule = Utils.DeserializeJson<ModuleInfoExtended>(p_target_module, _customSourceGenerationContext.ModuleInfoExtended);

                var getSelected = Marshal.GetDelegateForFunctionPointer<N_GetSelected>(new IntPtr(p_get_selected));
                var setSelected = Marshal.GetDelegateForFunctionPointer<N_SetSelected>(new IntPtr(p_set_selected));
                var getDisabled = Marshal.GetDelegateForFunctionPointer<N_GetDisabled>(new IntPtr(p_get_disabled));
                var setDisabled = Marshal.GetDelegateForFunctionPointer<N_SetDisabled>(new IntPtr(p_set_disabled));

                ModuleUtilities.EnableModule(modules, targetModule, module =>
                {
                    fixed (char* pModuleId = module.Id)
                    {
                        using var result = SafeStructMallocHandle.Create(getSelected(p_owner, (param_string*) pModuleId));
                        return result.ValueAsBool();
                    }
                }, (module, value) =>
                {
                    fixed (char* pModuleId = module.Id)
                    {
                        var result = SafeStructMallocHandle.Create(setSelected(p_owner, (param_string*) pModuleId, value));
                        result.ValueAsVoid();
                    }
                }, module =>
                {
                    fixed (char* pModuleId = module.Id)
                    {
                        using var result = SafeStructMallocHandle.Create(getDisabled(p_owner, (param_string*) pModuleId));
                        return result.ValueAsBool();
                    }
                }, (module, value) =>
                {
                    fixed (char* pModuleId = module.Id)
                    {
                        var result = SafeStructMallocHandle.Create(setDisabled(p_owner, (param_string*) pModuleId, value));
                        result.ValueAsVoid();
                    }
                });

                return return_value_void.AsValue();
            }
            catch (Exception e)
            {
                return return_value_void.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "disable_module")]
        public static return_value_void* DisableModule(
            void* p_owner,
            param_json* p_module,
            param_json* p_target_module,
            delegate* unmanaged[Cdecl]<return_value_bool*, void*, param_string*> p_get_selected,
            delegate* unmanaged[Cdecl]<return_value_void*, void*, param_string*, bool> p_set_selected,
            delegate* unmanaged[Cdecl]<return_value_bool*, void*, param_string*> p_get_disabled,
            delegate* unmanaged[Cdecl]<return_value_void*, void*, param_string*, bool> p_set_disabled)
        {
            try
            {
                var modules = Utils.DeserializeJson<ModuleInfoExtended[]>(p_module, _customSourceGenerationContext.ModuleInfoExtendedArray);
                var targetModule = Utils.DeserializeJson<ModuleInfoExtended>(p_target_module, _customSourceGenerationContext.ModuleInfoExtended);

                var getSelected = Marshal.GetDelegateForFunctionPointer<N_GetSelected>(new IntPtr(p_get_selected));
                var setSelected = Marshal.GetDelegateForFunctionPointer<N_SetSelected>(new IntPtr(p_set_selected));
                var getDisabled = Marshal.GetDelegateForFunctionPointer<N_GetDisabled>(new IntPtr(p_get_disabled));
                var setDisabled = Marshal.GetDelegateForFunctionPointer<N_SetDisabled>(new IntPtr(p_set_disabled));

                ModuleUtilities.DisableModule(modules, targetModule, module =>
                {
                    fixed (char* pModuleId = module.Id)
                    {
                        using var result = SafeStructMallocHandle.Create(getSelected(p_owner, (param_string*) pModuleId));
                        return result.ValueAsBool();
                    }
                }, (module, value) =>
                {
                    fixed (char* pModuleId = module.Id)
                    {
                        var result = SafeStructMallocHandle.Create(setSelected(p_owner, (param_string*) pModuleId, value));
                        result.ValueAsVoid();
                    }
                }, module =>
                {
                    fixed (char* pModuleId = module.Id)
                    {
                        using var result = SafeStructMallocHandle.Create(getDisabled(p_owner, (param_string*) pModuleId));
                        return result.ValueAsBool();
                    }
                }, (module, value) =>
                {
                    fixed (char* pModuleId = module.Id)
                    {
                        var result = SafeStructMallocHandle.Create(setDisabled(p_owner, (param_string*) pModuleId, value));
                        result.ValueAsVoid();
                    }
                });

                return return_value_void.AsValue();
            }
            catch (Exception e)
            {
                return return_value_void.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "get_module_info")]
        public static return_value_json* GetModuleInfo(param_string* p_xml_content)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(new string(param_string.ToSpan(p_xml_content)));

                var result = ModuleInfoExtended.FromXml(doc);

                return return_value_json.AsValue<ModuleInfoExtended>(result, _customSourceGenerationContext.ModuleInfoExtended);
            }
            catch (Exception e)
            {
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "get_sub_module_info")]
        public static return_value_json* GetSubModuleInfo(param_string* p_xml_content)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(new string(param_string.ToSpan(p_xml_content)));

                var result = SubModuleInfoExtended.FromXml(doc);

                return return_value_json.AsValue<SubModuleInfoExtended>(result, _customSourceGenerationContext.SubModuleInfoExtended);
            }
            catch (Exception e)
            {
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "compare_versions")]
        public static return_value_int32* CompareVersions(param_json* p_x, param_json* p_y)
        {
            try
            {
                var x = Utils.DeserializeJson<ApplicationVersion>(p_x, _customSourceGenerationContext.ApplicationVersion);
                var y = Utils.DeserializeJson<ApplicationVersion>(p_y, _customSourceGenerationContext.ApplicationVersion);

                var result = _applicationVersionComparer.Compare(x, y);

                return return_value_int32.AsValue(result);
            }
            catch (Exception e)
            {
                return return_value_int32.AsError(Utils.Copy(e.ToString()));
            }
        }
    }
}