#ifndef BMM_BLMANAGER_GUARD_HPP_
#define BMM_BLMANAGER_GUARD_HPP_

#include "utils.hpp"
#include <codecvt>

using namespace Napi;
using namespace Utils;
using namespace Bannerlord::ModuleManager::Native;

namespace Bannerlord::ModuleManager
{

    struct ValidationData
    {
        const Napi::Env Env;
        const Function FIsSelected;
    };
    static return_value_bool *const isSelected(const param_ptr *const p_owner, const param_string *const p_module_id) noexcept
    {
        try
        {
            const auto data = static_cast<const ValidationData *const>(p_owner);
            const auto env = data->Env;

            const auto moduleId = String::New(env, p_module_id);

            return Create(return_value_bool{nullptr, data->FIsSelected({moduleId}).As<Boolean>()});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_bool{Copy(conv.from_bytes(e.what())), false});
        }
    }

    struct EnableDisableData
    {
        const Napi::Env Env;
        const Function FGetSelected;
        const Function FSetSelected;
        const Function FGetDisabled;
        const Function FSetDisabled;
    };
    static return_value_bool *const getSelected(const param_ptr *const p_owner, const param_string *const p_module_id) noexcept
    {
        try
        {
            const auto data = static_cast<const EnableDisableData *const>(p_owner);
            const auto env = data->Env;

            const auto moduleId = String::New(env, p_module_id);

            return Create(return_value_bool{nullptr, data->FGetSelected({moduleId}).As<Boolean>()});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_bool{Copy(conv.from_bytes(e.what())), false});
        }
    }
    static return_value_void *const setSelected(const param_ptr *const p_owner, const param_string *const p_module_id, param_bool value_raw) noexcept
    {
        try
        {
            const auto data = static_cast<const EnableDisableData *const>(p_owner);
            const auto env = data->Env;

            const auto moduleId = String::New(env, p_module_id);
            const auto value = Boolean::New(env, value_raw == 1);

            data->FSetSelected({moduleId, value});
            return Create(return_value_void{nullptr});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
    }
    static return_value_bool *const getDisabled(const param_ptr *const p_owner, const param_string *const p_module_id) noexcept
    {
        try
        {
            const auto data = static_cast<const EnableDisableData *const>(p_owner);
            const auto env = data->Env;

            const auto moduleId = String::New(env, Copy(p_module_id));

            return Create(return_value_bool{nullptr, data->FGetDisabled({moduleId}).As<Boolean>()});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_bool{Copy(conv.from_bytes(e.what())), false});
        }
    }
    static return_value_void *const setDisabled(const param_ptr *const p_owner, const param_string *const p_module_id, param_bool value_raw) noexcept
    {
        try
        {
            const auto data = static_cast<const EnableDisableData *const>(p_owner);
            const auto env = data->Env;

            const auto moduleId = String::New(env, p_module_id);
            const auto value = Boolean::New(env, value_raw == 1);

            data->FSetDisabled({moduleId, value});
            return Create(return_value_void{nullptr});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
    }

    Value Sort(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto source = JSONStringify(env, info[0].As<Object>());

        const auto sourceCopy = CopyWithFree(source.Utf16Value());

        const auto result = bmm_sort(sourceCopy.get());
        return ThrowOrReturnJson(env, result);
    }
    Value SortWithOptions(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto source = JSONStringify(env, info[0].As<Object>());
        const auto options = JSONStringify(env, info[1].As<Object>());

        const auto sourceCopy = CopyWithFree(source.Utf16Value());
        const auto optionsCopy = CopyWithFree(options.Utf16Value());

        const auto result = bmm_sort_with_options(sourceCopy.get(), optionsCopy.get());
        return ThrowOrReturnJson(env, result);
    }

    Value AreAllDependenciesOfModulePresent(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto source = JSONStringify(env, info[0].As<Object>());
        const auto module = JSONStringify(env, info[1].As<Object>());

        const auto sourceCopy = CopyWithFree(source.Utf16Value());
        const auto moduleCopy = CopyWithFree(module.Utf16Value());

        const auto result = bmm_are_all_dependencies_of_module_present(sourceCopy.get(), moduleCopy.get());
        return ThrowOrReturnBoolean(env, result);
    }

    Value GetDependentModulesOf(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto source = JSONStringify(env, info[0].As<Object>());
        const auto module = JSONStringify(env, info[1].As<Object>());

        const auto sourceCopy = CopyWithFree(source.Utf16Value());
        const auto moduleCopy = CopyWithFree(module.Utf16Value());

        const auto result = bmm_get_dependent_modules_of(sourceCopy.get(), moduleCopy.get());
        return ThrowOrReturnJson(env, result);
    }
    Value GetDependentModulesOfWithOptions(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto source = JSONStringify(env, info[0].As<Object>());
        const auto module = JSONStringify(env, info[1].As<Object>());
        const auto options = JSONStringify(env, info[2].As<Object>());

        const auto sourceCopy = CopyWithFree(source.Utf16Value());
        const auto moduleCopy = CopyWithFree(module.Utf16Value());
        const auto optionsCopy = CopyWithFree(options.Utf16Value());

        const auto result = bmm_get_dependent_modules_of_with_options(sourceCopy.get(), moduleCopy.get(), optionsCopy.get());
        return ThrowOrReturnJson(env, result);
    }

    Value ValidateLoadOrder(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto source = JSONStringify(env, info[0].As<Object>());
        const auto targetModule = JSONStringify(env, info[1].As<Object>());

        const auto sourceCopy = CopyWithFree(source.Utf16Value());
        const auto targetModuleCopy = CopyWithFree(targetModule.Utf16Value());

        const auto result = bmm_validate_load_order(sourceCopy.get(), targetModuleCopy.get());
        return ThrowOrReturnJson(env, result);
    }

    Value ValidateModule(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto source = JSONStringify(env, info[0].As<Object>());
        const auto targetModule = JSONStringify(env, info[1].As<Object>());
        const auto manager = info[2].As<Object>();

        const auto fIsSelected = manager.Get("isSelected").As<Function>();

        const auto sourceCopy = CopyWithFree(source.Utf16Value());
        const auto targetModuleCopy = CopyWithFree(targetModule.Utf16Value());

        auto data = ValidationData{env, fIsSelected};

        const auto result = bmm_validate_module(static_cast<void *const>(&data), sourceCopy.get(), targetModuleCopy.get(), isSelected);
        return ThrowOrReturnJson(env, result);
    }

    void EnableModule(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto source = JSONStringify(env, info[0].As<Object>());
        const auto targetModule = JSONStringify(env, info[1].As<Object>());
        const auto manager = info[2].As<Object>();

        const auto fGetSelected = manager.Get("getSelected").As<Function>();
        const auto fSetSelected = manager.Get("setSelected").As<Function>();
        const auto fGetDisabled = manager.Get("getDisabled").As<Function>();
        const auto fSetDisabled = manager.Get("setDisabled").As<Function>();

        const auto sourceCopy = CopyWithFree(source.Utf16Value());
        const auto targetModuleCopy = CopyWithFree(targetModule.Utf16Value());

        auto data = EnableDisableData{env, fGetSelected, fSetSelected, fGetDisabled, fSetDisabled};

        const auto result = bmm_enable_module(static_cast<void *const>(&data), sourceCopy.get(), targetModuleCopy.get(), getSelected, setSelected, getDisabled, setDisabled);
        ThrowOrReturn(env, result);
    }
    void DisableModule(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto source = JSONStringify(env, info[0].As<Object>());
        const auto targetModule = JSONStringify(env, info[1].As<Object>());
        const auto manager = info[2].As<Object>();

        const auto FGetSelected = manager.Get("getSelected").As<Function>();
        const auto FSetSelected = manager.Get("setSelected").As<Function>();
        const auto FGetDisabled = manager.Get("getDisabled").As<Function>();
        const auto FSetDisabled = manager.Get("setDisabled").As<Function>();

        const auto sourceCopy = CopyWithFree(source.Utf16Value());
        const auto targetModuleCopy = CopyWithFree(targetModule.Utf16Value());

        auto data = EnableDisableData{env, FGetSelected, FSetSelected, FGetDisabled, FSetDisabled};

        const auto result = bmm_disable_module(static_cast<void *const>(&data), sourceCopy.get(), targetModuleCopy.get(), getSelected, setSelected, getDisabled, setDisabled);
        ThrowOrReturn(env, result);
    }

    Value GetModuleInfo(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto source = info[0].As<String>();

        const auto sourceCopy = CopyWithFree(source.Utf16Value());

        const auto result = bmm_get_module_info(sourceCopy.get());
        return ThrowOrReturnJson(env, result);
    }
    Value GetSubModuleInfo(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto source = info[0].As<String>();

        const auto sourceCopy = CopyWithFree(source.Utf16Value());

        const auto result = bmm_get_sub_module_info(sourceCopy.get());
        return ThrowOrReturnJson(env, result);
    }

    Value CompareVersions(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto x = JSONStringify(env, info[0].As<Object>());
        const auto y = JSONStringify(env, info[1].As<Object>());

        const auto xCopy = CopyWithFree(x.Utf16Value());
        const auto yCopy = CopyWithFree(y.Utf16Value());

        const auto result = bmm_compare_versions(xCopy.get(), yCopy.get());
        return ThrowOrReturnInt32(env, result);
    }

    Value GetDependenciesAll(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto module = JSONStringify(env, info[0].As<Object>());

        const auto moduleCopy = CopyWithFree(module.Utf16Value());

        const auto result = bmm_get_dependencies_all(moduleCopy.get());
        return ThrowOrReturnJson(env, result);
    }
    Value GetDependenciesToLoadBeforeThis(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto module = JSONStringify(env, info[0].As<Object>());

        const auto moduleCopy = CopyWithFree(module.Utf16Value());

        const auto result = bmm_get_dependencies_load_before_this(moduleCopy.get());
        return ThrowOrReturnJson(env, result);
    }
    Value GetDependenciesToLoadAfterThis(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto module = JSONStringify(env, info[0].As<Object>());

        const auto moduleCopy = CopyWithFree(module.Utf16Value());

        const auto result = bmm_get_dependencies_load_after_this(moduleCopy.get());
        return ThrowOrReturnJson(env, result);
    }
    Value GetDependenciesIncompatibles(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto module = JSONStringify(env, info[0].As<Object>());

        const auto moduleCopy = CopyWithFree(module.Utf16Value());

        const auto result = bmm_get_dependencies_incompatibles(moduleCopy.get());
        return ThrowOrReturnJson(env, result);
    }

    Object Init(const Env env, const Object exports)
    {
        exports.Set("sort", Function::New(env, Sort));
        exports.Set("sortWithOptions", Function::New(env, SortWithOptions));

        exports.Set("areAllDependenciesOfModulePresent", Function::New(env, AreAllDependenciesOfModulePresent));

        exports.Set("getDependentModulesOf", Function::New(env, GetDependentModulesOf));
        exports.Set("getDependentModulesOfWithOptions", Function::New(env, GetDependentModulesOfWithOptions));

        exports.Set("validateModule", Function::New(env, ValidateModule));
        exports.Set("validateLoadOrder", Function::New(env, ValidateLoadOrder));

        exports.Set("enableModule", Function::New(env, EnableModule));
        exports.Set("disableModule", Function::New(env, DisableModule));

        exports.Set("getModuleInfo", Function::New(env, GetModuleInfo));
        exports.Set("getSubModuleInfo", Function::New(env, GetSubModuleInfo));

        exports.Set("compareVersions", Function::New(env, CompareVersions));

        exports.Set("getDependenciesAll", Function::New(env, GetDependenciesAll));
        exports.Set("getDependenciesToLoadBeforeThis", Function::New(env, GetDependenciesToLoadBeforeThis));
        exports.Set("getDependenciesToLoadAfterThis", Function::New(env, GetDependenciesToLoadAfterThis));
        exports.Set("getDependenciesIncompatibles", Function::New(env, GetDependenciesIncompatibles));

        return exports;
    }

}
#endif