using BUTR.NativeAOT.Shared;

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml;

namespace Bannerlord.ModuleManager.Native
{
    public static unsafe partial class Bindings
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate return_value_bool* N_IsSelected(param_ptr* p_owner, param_string* p_module_id);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate return_value_bool* N_GetSelected(param_ptr* p_owner, param_string* p_module_id);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate return_value_void* N_SetSelected(param_ptr* p_owner, param_string* p_module_id, param_bool value);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate return_value_bool* N_GetDisabled(param_ptr* p_owner, param_string* p_module_id);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate return_value_void* N_SetDisabled(param_ptr* p_owner, param_string* p_module_id, param_bool value);

        private static readonly ApplicationVersionComparer _applicationVersionComparer = new();

        [UnmanagedCallersOnly(EntryPoint = "bmm_sort", CallConvs = new [] { typeof(CallConvCdecl) })]
        public static return_value_json* Sort(param_json* p_source)
        {
            Logger.LogInput(p_source);
            try
            {
                var source = Utils.DeserializeJson(p_source, CustomSourceGenerationContext.ModuleInfoExtendedArray);

                var result = ModuleSorter.Sort(source).ToArray();

                Logger.LogOutputManaged(result);
                return return_value_json.AsValue(result, CustomSourceGenerationContext.ModuleInfoExtendedArray, false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsError(Utils.Copy(e.ToString(), false), false);
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "bmm_sort_with_options", CallConvs = new [] { typeof(CallConvCdecl) })]
        public static return_value_json* SortWithOptions(param_json* p_source, param_json* p_options)
        {
            Logger.LogInput(p_source, p_options);
            try
            {
                var source = Utils.DeserializeJson(p_source, CustomSourceGenerationContext.ModuleInfoExtendedArray);
                var options = Utils.DeserializeJson(p_options, CustomSourceGenerationContext.ModuleSorterOptions);

                var result = ModuleSorter.Sort(source, options).ToArray();

                Logger.LogOutputManaged(result);
                return return_value_json.AsValue(result, CustomSourceGenerationContext.ModuleInfoExtendedArray, false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsError(Utils.Copy(e.ToString(), false), false);
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "bmm_are_all_dependencies_of_module_present", CallConvs = new [] { typeof(CallConvCdecl) })]
        public static return_value_bool* AreAllDependenciesOfModulePresent(param_json* p_source, param_json* p_module)
        {
            Logger.LogInput(p_source, p_module);
            try
            {
                var source = Utils.DeserializeJson(p_source, CustomSourceGenerationContext.ModuleInfoExtendedArray);
                var module = Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

                var result = ModuleUtilities.AreDependenciesPresent(source, module);

                Logger.LogOutputPrimitive(result);
                return return_value_bool.AsValue(result, false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_bool.AsError(Utils.Copy(e.ToString(), false), false);
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependent_modules_of", CallConvs = new [] { typeof(CallConvCdecl) })]
        public static return_value_json* GetDependentModulesOf(param_json* p_source, param_json* p_module)
        {
            Logger.LogInput(p_source, p_module);
            try
            {
                var source = Utils.DeserializeJson(p_source, CustomSourceGenerationContext.ModuleInfoExtendedArray);
                var module = Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

                var result = ModuleUtilities.GetDependencies(source, module).ToArray();

                Logger.LogOutputManaged(result);
                return return_value_json.AsValue(result, CustomSourceGenerationContext.ModuleInfoExtendedArray, false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsError(Utils.Copy(e.ToString(), false), false);
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependent_modules_of_with_options", CallConvs = new [] { typeof(CallConvCdecl) })]
        public static return_value_json* GetDependentModulesOfWithOptions(param_json* p_source, param_json* p_module, param_json* p_options)
        {
            Logger.LogInput(p_source, p_module, p_options);
            try
            {
                var source = Utils.DeserializeJson(p_source, CustomSourceGenerationContext.ModuleInfoExtendedArray);
                var module = Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtended);
                var options = Utils.DeserializeJson(p_options, CustomSourceGenerationContext.ModuleSorterOptions);

                var result = ModuleUtilities.GetDependencies(source, module, options).ToArray();

                Logger.LogOutputManaged(result);
                return return_value_json.AsValue(result, CustomSourceGenerationContext.ModuleInfoExtendedArray, false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsError(Utils.Copy(e.ToString(), false), false);
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "bmm_validate_module", CallConvs = new [] { typeof(CallConvCdecl) })]
        public static return_value_json* ValidateModule(param_ptr* p_owner, param_json* p_modules, param_json* p_target_module,
            delegate* unmanaged[Cdecl]<return_value_bool*, param_ptr*, param_string*> p_is_selected)
        {
            Logger.LogInput(p_modules, p_target_module);
            try
            {
                var modules = Utils.DeserializeJson(p_modules, CustomSourceGenerationContext.ModuleInfoExtendedArray);
                var targetModule = Utils.DeserializeJson(p_target_module, CustomSourceGenerationContext.ModuleInfoExtended);

                var isSelected = Marshal.GetDelegateForFunctionPointer<N_IsSelected>(new IntPtr(p_is_selected));

                var result = ModuleUtilities.ValidateModule(modules, targetModule, module =>
                {
                    Logger.LogInput();
                    fixed (char* pModuleId = module.Id)
                    {
                        Logger.LogInputChar(pModuleId);
                        
                        using var resultStr = SafeStructMallocHandle.Create(isSelected(p_owner, (param_string*) pModuleId), true);
                        var result = resultStr.ValueAsBool();
                        Logger.LogOutputPrimitive(result);
                        return result;
                    }
                }).ToArray();

                Logger.LogOutputManaged(result);
                return return_value_json.AsValue(result, CustomSourceGenerationContext.ModuleIssueArray, false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsError(Utils.Copy(e.ToString(), false), false);
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "bmm_validate_load_order", CallConvs = new [] { typeof(CallConvCdecl) })]
        public static return_value_json* ValidateLoadOrder(param_json* p_modules, param_json* p_target_module)
        {
            Logger.LogInput(p_modules, p_target_module);
            try
            {
                var modules = Utils.DeserializeJson(p_modules, CustomSourceGenerationContext.ModuleInfoExtendedArray);
                var targetModule = Utils.DeserializeJson(p_target_module, CustomSourceGenerationContext.ModuleInfoExtended);

                var result = ModuleUtilities.ValidateLoadOrder(modules, targetModule).ToArray();

                Logger.LogOutputManaged(result);
                return return_value_json.AsValue(result, CustomSourceGenerationContext.ModuleIssueArray, false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsError(Utils.Copy(e.ToString(), false), false);
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "bmm_enable_module", CallConvs = new [] { typeof(CallConvCdecl) })]
        public static return_value_void* EnableModule(param_ptr* p_owner, param_json* p_module, param_json* p_target_module,
            delegate* unmanaged[Cdecl]<return_value_bool*, param_ptr*, param_string*> p_get_selected,
            delegate* unmanaged[Cdecl]<return_value_void*, param_ptr*, param_string*, param_bool> p_set_selected,
            delegate* unmanaged[Cdecl]<return_value_bool*, param_ptr*, param_string*> p_get_disabled,
            delegate* unmanaged[Cdecl]<return_value_void*, param_ptr*, param_string*, param_bool> p_set_disabled)
        {
            Logger.LogInput(p_module, p_target_module);
            try
            {
                var modules = Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtendedArray);
                var targetModule = Utils.DeserializeJson(p_target_module, CustomSourceGenerationContext.ModuleInfoExtended);

                var getSelected = Marshal.GetDelegateForFunctionPointer<N_GetSelected>(new IntPtr(p_get_selected));
                var setSelected = Marshal.GetDelegateForFunctionPointer<N_SetSelected>(new IntPtr(p_set_selected));
                var getDisabled = Marshal.GetDelegateForFunctionPointer<N_GetDisabled>(new IntPtr(p_get_disabled));
                var setDisabled = Marshal.GetDelegateForFunctionPointer<N_SetDisabled>(new IntPtr(p_set_disabled));

                ModuleUtilities.EnableModule(modules, targetModule, module =>
                {
                    Logger.LogInput();
                    fixed (char* pModuleId = module.Id)
                    {
                        Logger.LogInputChar(pModuleId);

                        using var resultStr = SafeStructMallocHandle.Create(getSelected(p_owner, (param_string*) pModuleId), true);
                        var result = resultStr.ValueAsBool();
                        Logger.LogOutputPrimitive(result);
                        return result;
                    }
                }, (module, value) =>
                {
                    Logger.LogInput();
                    fixed (char* pModuleId = module.Id)
                    {
                        Logger.LogInputChar(pModuleId);

                        using var resultStr = SafeStructMallocHandle.Create(setSelected(p_owner, (param_string*) pModuleId, value), true);
                        resultStr.ValueAsVoid();
                        Logger.LogOutput();
                    }
                }, module =>
                {
                    Logger.LogInput();
                    fixed (char* pModuleId = module.Id)
                    {
                        Logger.LogInputChar(pModuleId);

                        using var resultStr = SafeStructMallocHandle.Create(getDisabled(p_owner, (param_string*) pModuleId), true);
                        var result = resultStr.ValueAsBool();
                        Logger.LogOutputPrimitive(result);
                        return result;
                    }
                }, (module, value) =>
                {
                    Logger.LogInput();
                    fixed (char* pModuleId = module.Id)
                    {
                        Logger.LogInputChar(pModuleId);

                        using var resultStr = SafeStructMallocHandle.Create(setDisabled(p_owner, (param_string*) pModuleId, value), true);
                        resultStr.ValueAsVoid();
                        Logger.LogOutput();
                    }
                });

                Logger.LogOutput();
                return return_value_void.AsValue(false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_void.AsError(Utils.Copy(e.ToString(), false), false);
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "bmm_disable_module", CallConvs = new [] { typeof(CallConvCdecl) })]
        public static return_value_void* DisableModule(param_ptr* p_owner, param_json* p_module, param_json* p_target_module,
            delegate* unmanaged[Cdecl]<return_value_bool*, param_ptr*, param_string*> p_get_selected,
            delegate* unmanaged[Cdecl]<return_value_void*, param_ptr*, param_string*, param_bool> p_set_selected,
            delegate* unmanaged[Cdecl]<return_value_bool*, param_ptr*, param_string*> p_get_disabled,
            delegate* unmanaged[Cdecl]<return_value_void*, param_ptr*, param_string*, param_ptr> p_set_disabled)
        {
            Logger.LogInput(p_module, p_target_module);
            try
            {
                var modules = Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtendedArray);
                var targetModule = Utils.DeserializeJson(p_target_module, CustomSourceGenerationContext.ModuleInfoExtended);

                var getSelected = Marshal.GetDelegateForFunctionPointer<N_GetSelected>(new IntPtr(p_get_selected));
                var setSelected = Marshal.GetDelegateForFunctionPointer<N_SetSelected>(new IntPtr(p_set_selected));
                var getDisabled = Marshal.GetDelegateForFunctionPointer<N_GetDisabled>(new IntPtr(p_get_disabled));
                var setDisabled = Marshal.GetDelegateForFunctionPointer<N_SetDisabled>(new IntPtr(p_set_disabled));

                ModuleUtilities.DisableModule(modules, targetModule, module =>
                {
                    Logger.LogInput();
                    fixed (char* pModuleId = module.Id)
                    {
                        Logger.LogInputChar(pModuleId);

                        using var resultStr = SafeStructMallocHandle.Create(getSelected(p_owner, (param_string*) pModuleId), true);
                        var result = resultStr.ValueAsBool();
                        Logger.LogOutputPrimitive(result);
                        return result;
                    }
                }, (module, value) =>
                {
                    Logger.LogInput();
                    fixed (char* pModuleId = module.Id)
                    {
                        Logger.LogInputChar(pModuleId);

                        using var resultStr = SafeStructMallocHandle.Create(setSelected(p_owner, (param_string*) pModuleId, value), true);
                        resultStr.ValueAsVoid();
                        Logger.LogOutput();
                    }
                }, module =>
                {
                    Logger.LogInput();
                    fixed (char* pModuleId = module.Id)
                    {
                        Logger.LogInputChar(pModuleId);

                        using var resultStr = SafeStructMallocHandle.Create(getDisabled(p_owner, (param_string*) pModuleId), true);
                        var result = resultStr.ValueAsBool();
                        Logger.LogOutputPrimitive(result);
                        return result;
                    }
                }, (module, value) =>
                {
                    Logger.LogInput();
                    fixed (char* pModuleId = module.Id)
                    {
                        Logger.LogInputChar(pModuleId);

                        using var resultStr = SafeStructMallocHandle.Create(setDisabled(p_owner, (param_string*) pModuleId, value), true);
                        resultStr.ValueAsVoid();
                        Logger.LogOutput();
                    }
                });

                Logger.LogOutput();
                return return_value_void.AsValue(false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_void.AsError(Utils.Copy(e.ToString(), false), false);
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "bmm_get_module_info", CallConvs = new [] { typeof(CallConvCdecl) })]
        public static return_value_json* GetModuleInfo(param_string* p_xml_content)
        {
            Logger.LogInput(p_xml_content);
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(new string(param_string.ToSpan(p_xml_content)));

                var result = ModuleInfoExtended.FromXml(doc);
                if (result is null)
                {
                    var e = new InvalidOperationException("Invalid xml content!");
                    Logger.LogException(e);
                    return return_value_json.AsError(Utils.Copy(e.ToString(), false), false);
                }

                Logger.LogOutputManaged(result);
                return return_value_json.AsValue(result, CustomSourceGenerationContext.ModuleInfoExtended, false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsError(Utils.Copy(e.ToString(), false), false);
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "bmm_get_sub_module_info", CallConvs = new [] { typeof(CallConvCdecl) })]
        public static return_value_json* GetSubModuleInfo(param_string* p_xml_content)
        {
            Logger.LogInput(p_xml_content);
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(new string(param_string.ToSpan(p_xml_content)));

                var result = SubModuleInfoExtended.FromXml(doc);
                if (result is null)
                {
                    var e = new InvalidOperationException("Invalid xml content!");
                    Logger.LogException(e);
                    return return_value_json.AsError(Utils.Copy(e.ToString(), false), false);
                }

                return return_value_json.AsValue(result, CustomSourceGenerationContext.SubModuleInfoExtended, false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsError(Utils.Copy(e.ToString(), false), false);
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "bmm_compare_versions", CallConvs = new [] { typeof(CallConvCdecl) })]
        public static return_value_int32* CompareVersions(param_json* p_x, param_json* p_y)
        {
            Logger.LogInput(p_x, p_y);
            try
            {
                var x = Utils.DeserializeJson(p_x, CustomSourceGenerationContext.ApplicationVersion);
                var y = Utils.DeserializeJson(p_y, CustomSourceGenerationContext.ApplicationVersion);

                var result = _applicationVersionComparer.Compare(x, y);

                Logger.LogOutputPrimitive(result);
                return return_value_int32.AsValue(result, false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_int32.AsError(Utils.Copy(e.ToString(), false), false);
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependencies_all", CallConvs = new [] { typeof(CallConvCdecl) })]
        public static return_value_json* GetDependenciesAll(param_json* p_module)
        {
            Logger.LogInput(p_module);
            try
            {
                var module = Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

                var result = module.DependenciesAllDistinct().ToArray();

                Logger.LogOutputManaged(result);
                return return_value_json.AsValue(result, CustomSourceGenerationContext.DependentModuleMetadataArray, false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsError(Utils.Copy(e.ToString(), false), false);
            }
        }
        [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependencies_load_before_this", CallConvs = new [] { typeof(CallConvCdecl) })]
        public static return_value_json* GetDependenciesLoadBeforeThis(param_json* p_module)
        {
            Logger.LogInput(p_module);
            try
            {
                var module = Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

                var result = module.DependenciesLoadBeforeThisDistinct().ToArray();

                Logger.LogOutputManaged(result);
                return return_value_json.AsValue(result, CustomSourceGenerationContext.DependentModuleMetadataArray, false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsError(Utils.Copy(e.ToString(), false), false);
            }
        }
        [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependencies_load_after_this", CallConvs = new [] { typeof(CallConvCdecl) })]
        public static return_value_json* GetDependenciesLoadAfterThis(param_json* p_module)
        {
            Logger.LogInput(p_module);
            try
            {
                var module = Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

                var result = module.DependenciesLoadAfterThisDistinct().ToArray();

                Logger.LogOutputManaged(result);
                return return_value_json.AsValue(result, CustomSourceGenerationContext.DependentModuleMetadataArray, false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsError(Utils.Copy(e.ToString(), false), false);
            }
        }
        [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependencies_incompatibles", CallConvs = new [] { typeof(CallConvCdecl) })]
        public static return_value_json* GetDependenciesIncompatibles(param_json* p_module)
        {
            Logger.LogInput(p_module);
            try
            {
                var module = Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

                var result = module.DependenciesIncompatiblesDistinct().ToArray();

                Logger.LogOutputManaged(result);
                return return_value_json.AsValue(result, CustomSourceGenerationContext.DependentModuleMetadataArray, false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsError(Utils.Copy(e.ToString(), false), false);
            }
        }
    }
}