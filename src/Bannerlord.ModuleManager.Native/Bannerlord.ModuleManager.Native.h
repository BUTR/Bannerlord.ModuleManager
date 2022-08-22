#ifndef SRC_BLMANAGER_BINDINGS_H_
#define SRC_BLMANAGER_BINDINGS_H_

namespace Bannerlord {
    namespace ModuleManager {
        namespace Native {

        #ifdef __cplusplus
            extern "C" {
        #endif
                // We work with ANSI strings. Escape any non ASII symbol (> 127) as a unicode codepoint (\u0000)
                const char* sort(const char* p_source_json);
                const char* sort_with_options(const char* p_source_json, const char* p_options_json);

                bool are_all_dependencies_of_module_present(const char* p_source_json, const char* p_module_json);

                const char* get_dependent_modules_of(const char* p_source_json, const char* p_module_json);
                const char* get_dependent_modules_of_with_options(const char* p_source_json, const char* p_module_json, const char* p_options_json);

                const char* validate_module(const char* p_uuid, const char* p_modules_json, const char* p_target_module_json, bool (*p_is_selected)(const char*, const char*));
                const char* validate_module_dependencies_declarations(const char* p_target_module_json);

                const char* enable_module(const char* p_uuid, const char* p_modules_json, const char* p_target_module_json, bool (*p_get_selected)(const char*, const char*), void (*p_set_selected)(const char*, const char*, bool), bool (*p_get_disabled)(const char*, const char*), void (*p_set_disabled)(const char*, const char*, bool));
                const char* disable_module(const char* p_uuid, const char* p_modules_json, const char* p_target_module_json, bool (*p_get_selected)(const char*, const char*), void (*p_set_selected)(const char*, const char*, bool), bool (*p_get_disabled)(const char*, const char*), void (*p_set_disabled)(const char*, const char*, bool));

                const char* get_module_info(const char* p_xml_content);
                const char* get_sub_module_info(const char* p_xml_content);

                int compare_versions(const char* p_x_json, const char* p_y_json);
        #ifdef __cplusplus
            }
        #endif

        }
    }
}

#endif