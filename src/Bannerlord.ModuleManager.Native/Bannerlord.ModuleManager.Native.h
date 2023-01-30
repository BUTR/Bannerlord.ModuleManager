#ifndef SRC_MODULEMANAGER_BINDINGS_H_
#define SRC_MODULEMANAGER_BINDINGS_H_

#include "Common.Native.h"

#define CALL_CONV __cdecl

#ifdef __cplusplus
using namespace Common;

namespace Bannerlord::ModuleManager {
    extern "C" {
#endif

        // All char16_t* parameters do not transfer ownership to the callee
        // All char16_t* returns pass their ownership to the callee

        return_value_json* CALL_CONV bmm_sort(const param_json* p_source);
        return_value_json* CALL_CONV bmm_sort_with_options(const param_json* p_source, const param_json* p_options);

        return_value_bool* CALL_CONV bmm_are_all_dependencies_of_module_present(const param_json* p_source, const param_json* p_module);

        return_value_json* CALL_CONV bmm_get_dependent_modules_of(const param_json* p_source, const param_json* p_module);
        return_value_json* CALL_CONV bmm_get_dependent_modules_of_with_options(const param_json* p_source, const param_json* p_module, const param_json* p_options);

        return_value_json* CALL_CONV bmm_validate_load_order(const param_json* p_source, const param_json* p_target_module);
        return_value_json* CALL_CONV bmm_validate_module(const void* p_owner, const param_json* p_modules, const param_json* p_target_module
            , return_value_bool* (CALL_CONV *p_is_selected)(const void* p_owner, const param_string* p_module_id));

        return_value_void* CALL_CONV bmm_enable_module(const void* p_owner, const param_json* p_modules, const param_json* p_target_module
            , return_value_bool* (CALL_CONV *p_get_selected)(const void* p_owner, const param_string* p_module_id)
            , return_value_void* (CALL_CONV *p_set_selected)(const void* p_owner, const param_string* p_module_id, param_bool value)
            , return_value_bool* (CALL_CONV *p_get_disabled)(const void* p_owner, const param_string* p_module_id)
            , return_value_void* (CALL_CONV *p_set_disabled)(const void* p_owner, const param_string* p_module_id, param_bool value));
        return_value_void* CALL_CONV bmm_disable_module(const void* p_owner, const param_json* p_modules, const param_json* p_target_module
            , return_value_bool* (CALL_CONV *p_get_selected)(const void* p_owner, const param_string* p_module_id)
            , return_value_void* (CALL_CONV *p_set_selected)(const void* p_owner, const param_string* p_module_id, param_bool value)
            , return_value_bool* (CALL_CONV *p_get_disabled)(const void* p_owner, const param_string* p_module_id)
            , return_value_void* (CALL_CONV *p_set_disabled)(const void* p_owner, const param_string* p_module_id, param_bool value));

        return_value_json* CALL_CONV bmm_get_module_info(const param_string* p_xml_content);
        return_value_json* CALL_CONV bmm_get_sub_module_info(const param_string* p_xml_content);

        return_value_int32* CALL_CONV bmm_compare_versions(const param_json* p_x, const param_json* p_y);

        return_value_json* CALL_CONV bmm_get_dependencies_all(const param_json* p_module);
        return_value_json* CALL_CONV bmm_get_dependencies_load_before_this(const param_json* p_module);
        return_value_json* CALL_CONV bmm_get_dependencies_load_after_this(const param_json* p_module);
        return_value_json* CALL_CONV bmm_get_dependencies_incompatibles(const param_json* p_module);


#ifdef __cplusplus
    }
}
#endif

#endif