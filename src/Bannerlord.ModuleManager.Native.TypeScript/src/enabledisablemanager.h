#ifndef BLMANAGER_ENABLEDISABLEMANAGER_GUARD 
#define BLMANAGER_ENABLEDISABLEMANAGER_GUARD

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

                class EnableDisableManager {
                    private:
                        static std::unordered_map<std::string, EnableDisableManager*> _map;
                        const std::string _uuid;
                            
                        const Env _env;
                        const Object _source;
                        const Object _targetModule;
                        const Function _getSelected;
                        const Function _setSelected;
                        const Function _getDisabled;
                        const Function _setDisabled;

                        static bool getSelected(const char* uuidRaw, const char* moduleId) {
                            const auto uuid = (std::string) uuidRaw;
                            const auto enableDisableManager = _map[uuid];

                            return (bool) enableDisableManager->_getSelected( { String::New(enableDisableManager->_env, moduleId) }).As<Boolean>();
                        }
                        static void setSelected(const char* uuidRaw, const char* moduleId, bool value) {
                            const auto uuid = (std::string) uuidRaw;
                            const auto enableDisableManager = _map[uuid];

                            enableDisableManager->_setSelected( { String::New(enableDisableManager->_env, moduleId), Boolean::New(enableDisableManager->_env, value) });
                        }
                        static bool getDisabled(const char* uuidRaw, const char* moduleId) {
                            const auto uuid = (std::string) uuidRaw;
                            const auto enableDisableManager = _map[uuid];

                            return (bool) enableDisableManager->_getDisabled( { String::New(enableDisableManager->_env, moduleId) }).As<Boolean>();
                        }
                        static void setDisabled(const char* uuidRaw, const char* moduleId, bool value) {
                            const auto uuid = (std::string) uuidRaw;
                            const auto enableDisableManager = _map[uuid];

                            enableDisableManager->_setDisabled( { String::New(enableDisableManager->_env, moduleId), Boolean::New(enableDisableManager->_env, value) });
                        }

                    public:
                        EnableDisableManager(Env env, Object source, Object targetModule, Function getSelected, Function setSelected, Function getDisabled, Function setDisabled) :
                            _env(env), _source(source), _targetModule(targetModule), _getSelected(getSelected), _setSelected(setSelected), _getDisabled(getDisabled), _setDisabled(setDisabled), _uuid(uuid::generate_uuid_v4()) {
                            _map.insert({ _uuid, this });
                        }

                        const Object EnableModule() {
                            const auto source       = (std::string) JSONStringify(_env, _source);
                            const auto targetModule = (std::string) JSONStringify(_env, _targetModule);

                            const auto result = Bannerlord::ModuleManager::Native::enable_module(escapeString(_uuid).c_str(), escapeString(source).c_str(), escapeString(targetModule).c_str(), &getSelected, &setSelected, &getDisabled, &setDisabled);
                            return JSONParse(_env, String::New(_env, unescapeString(result)));
                        }
                        const Object DisableModule() {
                            const auto source       = (std::string) JSONStringify(_env, _source);
                            const auto targetModule = (std::string) JSONStringify(_env, _targetModule);

                            const auto result = Bannerlord::ModuleManager::Native::disable_module(escapeString(_uuid).c_str(), escapeString(source).c_str(), escapeString(targetModule).c_str(), &getSelected, &setSelected, &getDisabled, &setDisabled);
                            return JSONParse(_env, String::New(_env, unescapeString(result)));
                        }

                        ~EnableDisableManager() {
                            _map.erase(_uuid);
                        }
                };

                std::unordered_map<std::string, EnableDisableManager*> EnableDisableManager::_map = [] {
                    std::unordered_map<std::string, EnableDisableManager*> map = { };
                    return map;
                }();

            }
        }
    }
}
#endif