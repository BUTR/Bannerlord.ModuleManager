#ifndef BLMANAGER_WRAPPER_GUARD 
#define BLMANAGER_WRAPPER_GUARD

#include <iostream>
#include <codecvt>
#include <string>
#include <locale>
#include <napi.h>
#include "Bannerlord.ModuleManager.Native.h"
#include "utils.h"
#include "validationmanager.h"
#include "enabledisablemanager.h"

using namespace Napi;
using namespace Bannerlord::ModuleManager::Native::Utils;

namespace Bannerlord {
    namespace ModuleManager {
        namespace Native {
            namespace Wrapper {

                const Object sortWrapped(const CallbackInfo& info) {
                    const auto env    = info.Env();
                    const auto source = JSONStringify(env, info[0].As<Object>()).Utf8Value();
                    const auto result = Bannerlord::ModuleManager::Native::sort(escapeString(source).c_str());
                    return JSONParse(env, String::New(env, unescapeString(result)));
                }
                const Object sortWithOptionsWrapped(const CallbackInfo& info) {
                    const auto env    = info.Env();
                    const auto source  = JSONStringify(env, info[0].As<Object>()).Utf8Value();
                    const auto options = JSONStringify(env, info[1].As<Object>()).Utf8Value();
                    const auto result  = Bannerlord::ModuleManager::Native::sort_with_options(escapeString(source).c_str(), escapeString(options).c_str());
                    return JSONParse(env, String::New(env, unescapeString(result)));
                }

                const Boolean areAllDependenciesOfModulePresentWrapper(const CallbackInfo& info) {
                    const auto env    = info.Env();
                    const auto source = JSONStringify(env, info[0].As<Object>()).Utf8Value();
                    const auto module = JSONStringify(env, info[1].As<Object>()).Utf8Value();
                    const auto result = Bannerlord::ModuleManager::Native::are_all_dependencies_of_module_present(escapeString(source).c_str(), escapeString(module).c_str());
                    return Boolean::New(env, result);
                }

                const Object getDependentModulesOfWrapped(const CallbackInfo& info) {
                    const auto env    = info.Env();
                    const auto source = JSONStringify(env, info[0].As<Object>()).Utf8Value();
                    const auto module = JSONStringify(env, info[1].As<Object>()).Utf8Value();
                    const auto result = Bannerlord::ModuleManager::Native::get_dependent_modules_of(escapeString(source).c_str(), escapeString(module).c_str());
                    return JSONParse(env, String::New(env, unescapeString(result)));
                }
                const Object getDependentModulesOfWithOptionsWrapped(const CallbackInfo& info) {
                    const auto env     = info.Env();
                    const auto source  = JSONStringify(env, info[0].As<Object>()).Utf8Value();
                    const auto module  = JSONStringify(env, info[1].As<Object>()).Utf8Value();
                    const auto options = JSONStringify(env, info[2].As<Object>()).Utf8Value();
                    const auto result  = Bannerlord::ModuleManager::Native::get_dependent_modules_of_with_options(escapeString(source).c_str(), escapeString(module).c_str(), escapeString(options).c_str());
                    return JSONParse(env, String::New(env, unescapeString(result)));
                }

                const Object validateModuleWrapped(const CallbackInfo& info) {
                    const auto env           = info.Env();
                    const auto source        = info[0].As<Object>();
                    const auto targetModule  = info[1].As<Object>();
                    const auto manager       = info[2].As<Object>();
                    const auto isSelected    = manager.Get("isSelected").As<Function>();
                    auto validationManager   = ValidationManager{env, source, targetModule, isSelected};
                    return validationManager.ValidateModule();
                }

                const Object validateModuleDependenciesDeclarationsWrapped(const CallbackInfo& info) {
                    const auto env          = info.Env();
                    const auto targetModule = JSONStringify(env, info[0].As<Object>()).Utf8Value();
                    const auto result       = Bannerlord::ModuleManager::Native::validate_module_dependencies_declarations(escapeString(targetModule).c_str());
                    return JSONParse(env, String::New(env, unescapeString(result)));
                }

                const Object enableModuleWrapped(const CallbackInfo& info) {
                    const auto env           = info.Env();
                    const auto source        = info[0].As<Object>();
                    const auto targetModule  = info[1].As<Object>();
                    const auto manager       = info[2].As<Object>();
                    const auto getSelected   = manager.Get("getSelected").As<Function>();
                    const auto setSelected   = manager.Get("setSelected").As<Function>();
                    const auto getDisabled   = manager.Get("getDisabled").As<Function>();
                    const auto setDisabled   = manager.Get("setDisabled").As<Function>();
                    auto enableDisableManager = EnableDisableManager{ env, source, targetModule, getSelected, setSelected, getDisabled, setDisabled};
                    return enableDisableManager.EnableModule();
                }
                const Object disableModuleWrapped(const CallbackInfo& info) {
                    const auto env           = info.Env();
                    const auto source        = info[0].As<Object>();
                    const auto targetModule  = info[1].As<Object>();
                    const auto manager       = info[2].As<Object>();
                    const auto getSelected   = manager.Get("getSelected").As<Function>();
                    const auto setSelected   = manager.Get("setSelected").As<Function>();
                    const auto getDisabled   = manager.Get("getDisabled").As<Function>();
                    const auto setDisabled   = manager.Get("setDisabled").As<Function>();
                    auto enableDisableManager = EnableDisableManager{ env, source, targetModule, getSelected, setSelected, getDisabled, setDisabled};
                    return enableDisableManager.DisableModule();
                }

                const Object getModuleInfoWrapped(const CallbackInfo& info) {
                    const auto env    = info.Env();
                    const auto source = info[0].As<String>().Utf8Value();
                    const auto str = escapeString(source.c_str());
                    const auto result = Bannerlord::ModuleManager::Native::get_module_info(str.c_str());
                    return JSONParse(env, String::New(env, unescapeString(result)));
                }
                const Object getSubModuleInfoWrapped(const CallbackInfo& info) {
                    const auto env    = info.Env();
                    const auto source = info[0].As<String>().Utf8Value();
                    const auto result = Bannerlord::ModuleManager::Native::get_sub_module_info(escapeString(source).c_str());
                    return JSONParse(env, String::New(env, unescapeString(result)));
                }

                const Number compareVersionsWrapped(const CallbackInfo& info) {
                    const auto env    = info.Env();
                    const auto x      = JSONStringify(env, info[0].As<Object>()).Utf8Value();
                    const auto y      = JSONStringify(env, info[1].As<Object>()).Utf8Value();
                    const auto result = Bannerlord::ModuleManager::Native::compare_versions(escapeString(x).c_str(), escapeString(y).c_str());
                    return Number::New(env, result);
                }

                const Object Init(Env env, Object exports) {
                    exports.Set("sort", Function::New(env, sortWrapped));
                    exports.Set("sortWithOptions", Function::New(env, sortWithOptionsWrapped));

                    exports.Set("areAllDependenciesOfModulePresent", Function::New(env, areAllDependenciesOfModulePresentWrapper));

                    exports.Set("getDependentModulesOf", Function::New(env, getDependentModulesOfWrapped));
                    exports.Set("getDependentModulesOfWithOptions", Function::New(env, getDependentModulesOfWithOptionsWrapped));

                    exports.Set("validateModule", Function::New(env, validateModuleWrapped));
                    exports.Set("validateModuleDependenciesDeclarations", Function::New(env, validateModuleDependenciesDeclarationsWrapped));

                    exports.Set("enableModule", Function::New(env, enableModuleWrapped));
                    exports.Set("disableModule", Function::New(env, disableModuleWrapped));
                    
                    exports.Set("getModuleInfo", Function::New(env, getModuleInfoWrapped));
                    exports.Set("getSubModuleInfo", Function::New(env, getSubModuleInfoWrapped));
                    
                    exports.Set("compareVersions", Function::New(env, compareVersionsWrapped));
                    
                    return exports;
                }

            }
        }
    }
}
#endif