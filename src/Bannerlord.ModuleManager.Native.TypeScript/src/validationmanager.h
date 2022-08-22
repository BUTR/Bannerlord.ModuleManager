#ifndef BLMANAGER_VALIDATIONMANAGER_GUARD 
#define BLMANAGER_VALIDATIONMANAGER_GUARD

#include <unordered_map>
#include <string>
#include <napi.h>
#include "uuid.h"
#include "utils.h"
#include "Bannerlord.ModuleManager.Native.h"

using namespace Napi;
using namespace Bannerlord::ModuleManager::Native::Utils;

namespace Bannerlord {
    namespace ModuleManager {
        namespace Native {
            namespace Wrapper {

                class ValidationManager {
                    private:
                        static std::unordered_map<std::string, ValidationManager*> _map;
                        const std::string _uuid;
                        
                        const Env _env;
                        const Object _source;
                        const Object _targetModule;
                        const Function _isSelected;

                        static bool isSelected(const char* uuidRaw, const char* moduleId) {
                            const auto uuid = (std::string) uuidRaw;
                            const auto validationManager = _map[uuid];

                            return (bool) validationManager->_isSelected( { String::New(validationManager->_env, moduleId) }).As<Boolean>();
                        }

                    public:
                        ValidationManager(const Env env, const Object source, const Object targetModule, const Function isSelected) :
                            _env(env), _source(source), _targetModule(targetModule), _isSelected(isSelected), _uuid(uuid::generate_uuid_v4()) {
                            _map.insert({ _uuid, this });
                        }

                        const Object ValidateModule() {
                            const auto source       = (std::string) JSONStringify(_env, _source);
                            const auto targetModule = (std::string) JSONStringify(_env, _targetModule);

                            const auto result = Bannerlord::ModuleManager::Native::validate_module(escapeString(_uuid).c_str(), escapeString(source).c_str(), escapeString(targetModule).c_str(), &isSelected);
                            return JSONParse(_env, String::New(_env, unescapeString(result)));
                        }

                        ~ValidationManager() {
                            _map.erase(_uuid);
                        }
                };

                std::unordered_map<std::string, ValidationManager*> ValidationManager::_map = [] {
                    std::unordered_map<std::string, ValidationManager*> map = { };
                    return map;
                }();

            }
        }
    }
}
#endif